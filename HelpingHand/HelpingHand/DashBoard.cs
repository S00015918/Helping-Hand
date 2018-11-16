using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using HelpingHand;
using HelpingHand.Model;
using System;
using System.Collections.Generic;
using static Android.Views.View;

namespace XamarinFirebaseAuth
{
    [Activity(Label = "DashBoard" ,Theme = "@style/AppTheme")]
    public class DashBoard : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        EditText input_new_password, input_name, input_email;
        private ListView list_data;

        private List<Parent> list_parents = new List<Parent>();
        private ListViewAdapter adapter;
        Parent selectedParent;
        Button btnChangePass, btnLogout;
        RelativeLayout activity_dashboard;
        FirebaseAuth auth;

        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.dashboard_btn_change_pass)
                ChangePassword(input_new_password.Text);
            else if (v.Id == Resource.Id.dashboard_btn_logout)
                LogoutUser();
        }

        private void LogoutUser()
        {
            auth.SignOut();
            if(auth.CurrentUser == null)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
        }

        private void ChangePassword(string newPassword)
        {
            FirebaseUser user = auth.CurrentUser;
            user.UpdatePassword(newPassword)
            .AddOnCompleteListener(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DashBoard);

            //Add Toolbar

            //var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
            //toolbar.Title = "Welcome , " + auth.CurrentUser.Email;
            SupportActionBar.Title = "Welcome, " /*+ auth.CurrentUser.Email*/;
            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //SupportActionBar.SetHomeButtonEnabled(true);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //View
            btnChangePass = FindViewById<Button>(Resource.Id.dashboard_btn_change_pass);
            SearchView search = FindViewById<SearchView>(Resource.Id.searchview);
            btnLogout = FindViewById<Button>(Resource.Id.dashboard_btn_logout);
            input_new_password = FindViewById<EditText>(Resource.Id.dashboard_newpassword);
            activity_dashboard = FindViewById<RelativeLayout>(Resource.Id.activity_dashboard);

            search.SetQueryHint("Babysitter Search");
            list_data = FindViewById<ListView>(Resource.Id.list_data);
            list_data.ItemClick += (s, e) =>
            {
                Parent account = list_parents[e.Position];
                selectedParent = account;
                input_name.Text = account.name;
                input_email.Text = account.email;
            };

            LoadData();

            search.QueryTextChange += searchChange;
            btnChangePass.SetOnClickListener(this);
            btnLogout.SetOnClickListener(this);
            
        }

        private void searchChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            //Filter or Search
        }

        private async void LoadData()
        {

            list_data.Visibility = ViewStates.Invisible;
            var firebase = new FirebaseClient(FirebaseURL);
            var items = await firebase
                .Child("parent")
                .OnceAsync<Parent>();
            list_parents.Clear();
            adapter = null;
            foreach (var item in items)
            {
                Parent account = new Parent();
                account.id = item.Key;
                account.name = item.Object.name;
                account.email = item.Object.email;
                list_parents.Add(account);
            }
            adapter = new ListViewAdapter(this, list_parents);
            adapter.NotifyDataSetChanged();
            list_data.Adapter = adapter;

            list_data.Visibility = ViewStates.Visible;
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful == true)
            {
                Snackbar snackbar = Snackbar.Make(activity_dashboard, "Password has been Changed!", Snackbar.LengthShort);
                snackbar.Show();
            }
        }
    }
}