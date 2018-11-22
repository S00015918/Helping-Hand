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
using System.Collections;
using System.Collections.Generic;
using static Android.Views.View;

namespace XamarinFirebaseAuth
{
    [Activity(Label = "DashBoard" ,Theme = "@style/AppTheme")]
    public class DashBoard : AppCompatActivity, IOnCompleteListener
    {
        EditText input_new_password, input_name, input_email;
        private ListView list_data;
        private ArrayList filteredUsers;

        //private List<Parent> list_parents = new List<Parent>();
        List<BabySitter> list_babySitters = new List<BabySitter>();

        private ListViewAdapter adapter;
        //Parent selectedParent;
        BabySitter selectedBabysitter;

        RelativeLayout activity_dashboard;
        FirebaseAuth auth;

        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        //public void OnClick(View v)
        //{
        //    if (v.Id == Resource.Id.dashboard_btn_change_pass)
        //        ChangePassword(input_new_password.Text);
        //    else if (v.Id == Resource.Id.dashboard_btn_logout)
        //        LogoutUser();
        //}

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
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            FirebaseUser user = auth.CurrentUser;
            user.UpdatePassword(newPassword)
            .AddOnCompleteListener(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DashBoard);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //Add Toolbar
            //var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
            //toolbar.Title = "Welcome , " + auth.CurrentUser.Email;
            SupportActionBar.Title = auth.CurrentUser.Email;
      
            //View
            SearchView search = FindViewById<SearchView>(Resource.Id.searchview);

            activity_dashboard = FindViewById<RelativeLayout>(Resource.Id.activity_dashboard);

            search.SetQueryHint("Search");
            list_data = FindViewById<ListView>(Resource.Id.list_data);
            list_data.ItemClick += (s, e) =>
            {
                BabySitter account = list_babySitters[e.Position];
                selectedBabysitter = account;


                //input_name.Text = account.name;
                //input_email.Text = account.email;
            };


            LoadData();

            search.QueryTextChange += searchChange;
            
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
                .Child("babysitter")
                .OnceAsync<BabySitter>();
            list_babySitters.Clear();
            adapter = null;
            foreach (var item in items)
            {
                BabySitter account = new BabySitter();
                account.id = item.Key;
                account.name = item.Object.name;
                account.email = item.Object.email;
                list_babySitters.Add(account);
            }
            adapter = new ListViewAdapter(this, list_babySitters);
            adapter.NotifyDataSetChanged();
            list_data.Adapter = adapter;

            list_data.Visibility = ViewStates.Visible;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.menu_message)
            {
                StartActivity(new Android.Content.Intent(this, typeof(MessageActivity)));
                Finish();
            }
            else if (id == Resource.Id.menu_star) //favourites
            {
                //UpdateUser(selectedParent.id, input_name.Text, input_email.Text);
            }
            else if (id == Resource.Id.menu_user) //user profile
            {
                //DeleteUser(selectedParent.id);
            }
            return base.OnOptionsItemSelected(item);
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