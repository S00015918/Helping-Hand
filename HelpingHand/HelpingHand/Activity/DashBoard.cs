﻿using Android.App;
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
using HelpingHand.Adapter;

namespace XamarinFirebaseAuth
{
    [Activity(Label = "DashBoard", Theme = "@style/AppTheme")]
    public class DashBoard : AppCompatActivity, IOnCompleteListener
    {
        TextView txtviewBabysitter, txtviewParent;
        private ListView list_data;

        List<Parent> list_parents = new List<Parent>();
        List<BabySitter> list_babySitters = new List<BabySitter>();
        BabySitter babySitter;

        private BabysitterViewAdapter babysitterAdapter;
        private ParentViewAdapter parentAdapter;
        RelativeLayout activity_dashboard;
        FirebaseAuth auth;
        bool parentUser;

        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        private void LogoutUser()
        {
            auth.SignOut();
            if (auth.CurrentUser == null)
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
            txtviewBabysitter = FindViewById<TextView>(Resource.Id.txtBabysitter);
            txtviewParent = FindViewById<TextView>(Resource.Id.txtParent);

            activity_dashboard = FindViewById<RelativeLayout>(Resource.Id.activity_dashboard);

            var firebase = new FirebaseClient(FirebaseURL);

            search.SetQueryHint("Search");
            list_data = FindViewById<ListView>(Resource.Id.list_data);

            txtviewBabysitter.Visibility = ViewStates.Invisible;
            txtviewParent.Visibility = ViewStates.Invisible;
            list_data.Visibility = ViewStates.Invisible;

            //get all parents from the database
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
                list_parents.Add(account);  // add all parents to a list
            }

            if (users.Any((_) => _.Key == auth.CurrentUser.Uid))
            {
                // You are a parent
                // current user is a parent
                parentUser = true;
                txtviewBabysitter.Visibility = ViewStates.Visible;
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
                            string values = babySitter.availability;
                            string[] availableTimes = values.Split(',');
                        }
                        list_babySitters.Add(babySitter);
                    }
                    catch (Exception)
                    {
                        var babySitter = JsonConvert.DeserializeObject<BabySitter>(item.Object.ToString());
                        throw;
                    }
                }

                // when an item in the listview is clicked get the correspoding details
                list_data.ItemClick += (s, e) =>
                {
                    BabySitter selectedBabysitter = list_babySitters[e.Position];

                    var babysitterJson = JsonConvert.SerializeObject(selectedBabysitter);

                    // store details in JSON about the selected babysitter and then navigate to the view user page
                    var viewSelectedUser = new Intent(this, typeof(viewUser));
                    viewSelectedUser.PutExtra("KEY", babysitterJson);
                    StartActivity(viewSelectedUser);
                };

                babysitterAdapter = new BabysitterViewAdapter(this, list_babySitters);
                babysitterAdapter.NotifyDataSetChanged();
                list_data.Adapter = babysitterAdapter;

            }
            else
            {
                // you are a babysitter
                // current user is a babysitter
                parentUser = false;
                txtviewParent.Visibility = ViewStates.Visible;
                parentAdapter = new ParentViewAdapter(this, list_parents);
                parentAdapter.NotifyDataSetChanged();
                list_data.Adapter = parentAdapter;

                list_data.ItemClick += (s, e) =>
                {
                    Parent selectedParent = list_parents[e.Position];

                    var parentJson = JsonConvert.SerializeObject(selectedParent);
                    // store details in JSON about the selected parent and then navigate to the view user page
                    var viewSelectedUser = new Intent(this, typeof(viewUser));
                    viewSelectedUser.PutExtra("KEY", parentJson);
                    StartActivity(viewSelectedUser);
                };
            }

            list_data.Visibility = ViewStates.Visible;

            search.QueryTextChange += searchChange;
        }

        private void searchChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            // search bar used to filter out babysitters or parents based on location, return results based on the search query
            SearchView search = FindViewById<SearchView>(Resource.Id.searchview);
            if (parentUser == true)
            {
                search.QueryTextChange += (s, f) => babysitterAdapter.Filter.InvokeFilter(f.NewText);
            }
            else
            {
                search.QueryTextChange += (s, f) => parentAdapter.Filter.InvokeFilter(f.NewText);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_messages, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.menu_home)
            {
                StartActivity(new Android.Content.Intent(this, typeof(HomeActivity)));
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