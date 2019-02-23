using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using HelpingHand;
using HelpingHand.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using static Android.Content.ClipData;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;

namespace XamarinFirebaseAuth
{
    [Activity(Label = "DashBoard" ,Theme = "@style/AppTheme")]
    public class DashBoard : AppCompatActivity, IOnCompleteListener
    {
        //EditText input_new_password, input_name, input_email;
        private ListView list_data;
        //private ArrayList filteredUsers;

        List<Parent> list_parents = new List<Parent>();
        List<BabySitter> list_babySitters = new List<BabySitter>();
        BabySitter babySitter;

        private ListViewAdapter babysitterAdapter;
        private ParentViewAdapter parentAdapter;
        //Parent selectedParent;
        //BabySitter selectedBabysitter;

        RelativeLayout activity_dashboard;
        FirebaseAuth auth;

        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

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
         
        protected async override void OnCreate(Bundle savedInstanceState)
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
            //SupportActionBar.Title = auth.CurrentUser.Email;;

            //View
            SearchView search = FindViewById<SearchView>(Resource.Id.searchview);

            activity_dashboard = FindViewById<RelativeLayout>(Resource.Id.activity_dashboard);

            var firebase = new FirebaseClient(FirebaseURL);

            search.SetQueryHint("Search");
            list_data = FindViewById<ListView>(Resource.Id.list_data);

            list_data.Visibility = ViewStates.Invisible;

            var users = await firebase
                    .Child("parent")
                    .OnceAsync<Parent>();
            list_babySitters.Clear();
            babysitterAdapter = null;
            foreach (var item in users)
            {
                Parent account = new Parent();
                account.id = item.Key;
                account.name = item.Object.name;
                account.phone = item.Object.phone;
                account.city = item.Object.city;
                account.address = item.Object.address;
                account.email = item.Object.email;
                account.eircode = item.Object.eircode;
                account.noOfKids = item.Object.noOfKids;
                list_parents.Add(account);
            }

            list_data.ItemClick += (s, e) =>
            {
                Parent selectedParent = list_parents[e.Position];

                var parentJson = JsonConvert.SerializeObject(selectedParent);

                var viewSelectedUser = new Intent(this, typeof(viewUser));
                viewSelectedUser.PutExtra("KEY", parentJson);
                StartActivity(viewSelectedUser);
            };

            if (users.Any((_) => _.Key == auth.CurrentUser.Uid))
            {

                // You are a parent
                //var items = await firebase
                //        .Child("babysitter")
                //        .OnceAsync<BabySitter>();

                var items = await firebase.Child("babysitter").OnceAsync<object>();
                list_babySitters.Clear();
                babysitterAdapter = null;

                foreach (var item in items)
                {
                    try
                    {
                        babySitter = JsonConvert.DeserializeObject<BabySitter>(item.Object.ToString());
                        if (babySitter.availability != null)
                        {
                            string[] values = babySitter.availability;
                        }
                        list_babySitters.Add(babySitter);
                    }
                    catch (Exception)
                    {
                        var babySitter = JsonConvert.DeserializeObject<BabySitter>(item.Object.ToString());
                        throw;
                    }
                }

                //foreach (var item in items)
                //{
                //BabySitter account = new BabySitter();
                //account.id = item.Key;
                //account.name = item.Object.name;
                //account.age = item.Object.age;
                //account.phone = item.Object.phone;
                //account.city = item.Object.city;
                //account.address = item.Object.address;
                //account.email = item.Object.email;
                //account.eircode = item.Object.eircode;
                //account.availability = item.Object.availability;
                //list_babySitters.Add(account);

                //}

                list_data.ItemClick += (s, e) =>
                {
                    BabySitter selectedBabysitter = list_babySitters[e.Position];

                    var babysitterJson = JsonConvert.SerializeObject(selectedBabysitter);

                    var viewSelectedUser = new Intent(this, typeof(viewUser));
                    viewSelectedUser.PutExtra("KEY", babysitterJson);
                    StartActivity(viewSelectedUser);
                };

                babysitterAdapter = new ListViewAdapter(this, list_babySitters);
                babysitterAdapter.NotifyDataSetChanged();
                list_data.Adapter = babysitterAdapter;

            }
            else
            {
                // you are a babysitter
                parentAdapter = new ParentViewAdapter(this, list_parents);
                parentAdapter.NotifyDataSetChanged();
                list_data.Adapter = parentAdapter;
            }

            list_data.Visibility = ViewStates.Visible;

            search.QueryTextChange += searchChange;          
        }

        private void searchChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            SearchView search = FindViewById<SearchView>(Resource.Id.searchview);
            search.QueryTextChange += (s, f) => babysitterAdapter.Filter.InvokeFilter(f.NewText);
            //search.QueryTextChange += (s, f) => parentAdapter.Filter.InvokeFilter(f.NewText);
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
            else if (id == Resource.Id.menu_calendar) //calendar
            {
                StartActivity(new Android.Content.Intent(this, typeof(userSchedule)));
                Finish();
            }
            else if (id == Resource.Id.menu_star) //favourites
            {
                StartActivity(new Android.Content.Intent(this, typeof(userFavourites)));
                Finish();
            }
            else if (id == Resource.Id.menu_user) //user profile
            {
                StartActivity(new Android.Content.Intent(this, typeof(userProfile)));
                Finish();
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