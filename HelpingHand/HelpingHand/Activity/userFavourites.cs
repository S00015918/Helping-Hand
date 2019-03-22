using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using HelpingHand.Adapter;
using HelpingHand.Model;
using Newtonsoft.Json;
using XamarinFirebaseAuth;

namespace HelpingHand
{
    [Activity(Label = "Favourites", Theme = "@style/AppTheme")]
    public class userFavourites : AppCompatActivity
    {
        private ListView list_data;
        List<Parent> list_parents = new List<Parent>();
        List<BabySitter> list_babySitters = new List<BabySitter>();
        FirebaseAuth auth;
        private FavouriteBabysitterAdapter babysitterAdapter;
        private ParentViewAdapter parentAdapter;

        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.myFavourites);
            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            var firebase = new FirebaseClient(FirebaseURL);

            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
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
            }

            if (users.Any((_) => _.Key == auth.CurrentUser.Uid))
            {
                // You are a parent
                var sitters = await firebase
                    .Child("babysitter")
                    .OnceAsync<BabySitter>();
                list_babySitters.Clear();
                babysitterAdapter = null;
                foreach (var item in sitters)
                {
                    BabySitter account = new BabySitter();
                    account.name = item.Object.name;
                    account.rating = item.Object.rating;

                    int rating = account.rating;
                    if (rating == 5)
                    {
                        list_babySitters.Add(account);
                        babysitterAdapter = new FavouriteBabysitterAdapter(this, list_babySitters);
                        babysitterAdapter.NotifyDataSetChanged();
                        list_data.Adapter = babysitterAdapter;
                    }
                }
            }
            else
            {
                // you are a babysitter
                var parents = await firebase
                    .Child("parent")
                    .OnceAsync<Parent>();
                list_babySitters.Clear();
                babysitterAdapter = null;
                foreach (var item in parents)
                {
                    Parent account = new Parent();
                    account.id = item.Key;
                    account.name = item.Object.name;
                    list_parents.Add(account);

                    parentAdapter = new ParentViewAdapter(this, list_parents);
                    parentAdapter.NotifyDataSetChanged();
                    list_data.Adapter = parentAdapter;
                }
            }

            list_data.Visibility = ViewStates.Visible;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_profile, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.menu_home)
            {
                StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                Finish();
            }
            //else if (id == Resource.Id.menu_save) // Update users details
            //{
            //    UpdateUser();
            //}

            return base.OnOptionsItemSelected(item);
        }

        public void getFavourites()
        {

            string babysitter = this.Intent.GetStringExtra("KEY");
            BabySitter userSitter = JsonConvert.DeserializeObject<BabySitter>(babysitter);

            if (userSitter.id != auth.CurrentUser.Uid)
            {
                string user = userSitter.name;
                int rating = userSitter.rating;
                list_babySitters.Add(userSitter);

                babysitterAdapter = new FavouriteBabysitterAdapter(this, list_babySitters);
                babysitterAdapter.NotifyDataSetChanged();
                list_data.Adapter = babysitterAdapter;
            }
            else
            {
                string _parent = this.Intent.GetStringExtra("KEY");
                Parent userParent = JsonConvert.DeserializeObject<Parent>(_parent);
                list_parents.Add(userParent);
            }
        }

        private void UpdateUser()
        {
            var firebase = new FirebaseClient(FirebaseURL);

            Toast.MakeText(this, "Details Updated.", ToastLength.Short).Show();
        }
    }
}