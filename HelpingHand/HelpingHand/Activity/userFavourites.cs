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
using Firebase.Xamarin.Database.Query;
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
        List<Rating> list_ratings = new List<Rating>();
        List<BabySitter> list_babysitters = new List<BabySitter>();
        FirebaseAuth auth;
        Rating selectedUser;
        private FavouriteBabysitterAdapter ratedUserAdapter;
        private ParentViewAdapter parentAdapter;
        string ratedByEmail;

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

            var babysitterEmailList = new List<string>();
            var parentEmailList = new List<string>();

            var ratings = await firebase
                    .Child("rating").Child(auth.CurrentUser.Uid)
                    .OnceAsync<Rating>();
            list_ratings.Clear();

            foreach (var item in ratings)
            {
                Rating account = new Rating();
                account.ratedByEmail = item.Object.ratedByEmail;
                ratedByEmail = account.ratedByEmail;
                parentEmailList.Add(ratedByEmail);
                babysitterEmailList.Add(account.userRatedEmail);
            }

            if (parentEmailList.Contains(auth.CurrentUser.Email))
            {
                // Current user is a parent
                list_ratings.Clear();

                foreach (var item in ratings)
                {
                    Rating accountRated = new Rating();
                    accountRated.ratedByEmail = item.Object.ratedByEmail;
                    accountRated.ratedUsersName = item.Object.ratedUsersName;
                    accountRated.rating = item.Object.rating;
                    accountRated.userRatedEmail = item.Object.userRatedEmail;

                    int rating = accountRated.rating;
                    if (rating == 5)
                    {
                        list_ratings.Add(accountRated);
                        ratedUserAdapter = new FavouriteBabysitterAdapter(this, list_ratings);
                        ratedUserAdapter.NotifyDataSetChanged();
                        list_data.Adapter = ratedUserAdapter;
                    }
                }
            }
            if (babysitterEmailList.Contains(auth.CurrentUser.Email))
            {
                // current user is a babysitter
                var parents = await firebase
                    .Child("parent")
                    .OnceAsync<Parent>();
                list_ratings.Clear();
                ratedUserAdapter = null;
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
            list_data.ItemClick += (s, e) =>
            {
                selectedUser = list_ratings[e.Position];
                GetUsers();
            };
        }

        public async void GetUsers()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            var sitters = await firebase
                .Child("babysitter")
                .OnceAsync<BabySitter>();
            list_babysitters.Clear();
            foreach (var item in sitters)
            {
                BabySitter account = new BabySitter();
                account.email = item.Object.email;
                string email = account.email;

                if (selectedUser.userRatedEmail == email)
                {
                    BabySitter ratedAccount = new BabySitter();
                    ratedAccount.id = item.Object.id;
                    ratedAccount.name = item.Object.name;
                    ratedAccount.phone = item.Object.phone;
                    ratedAccount.city = item.Object.city;
                    ratedAccount.address = item.Object.address;
                    ratedAccount.email = item.Object.email;
                    ratedAccount.eircode = item.Object.eircode;
                    ratedAccount.age = item.Object.age;
                    ratedAccount.rate = item.Object.rate;
                    ratedAccount.availability = item.Object.availability;

                    var userJson = JsonConvert.SerializeObject(ratedAccount);

                    var viewSelectedUser = new Intent(this, typeof(viewUser));
                    viewSelectedUser.PutExtra("KEY", userJson);
                    StartActivity(viewSelectedUser);
                }
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
    }
}