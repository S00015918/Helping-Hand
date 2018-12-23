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
using Firebase.Xamarin.Database;
using XamarinFirebaseAuth;

namespace HelpingHand
{
    [Activity(Label = "Favourites", Theme = "@style/AppTheme")]
    public class userFavourites : AppCompatActivity
    {
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.myFavourites);

            string stringFavourite = this.Intent.GetStringExtra("FAV");

            //BabySitter user = JsonConvert.DeserializeObject<BabySitter>(stringName);

            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
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
            else if (id == Resource.Id.menu_save) // Update users details
            {
                UpdateUser();
            }

            return base.OnOptionsItemSelected(item);
        }

        private async void UpdateUser()
        {
            var firebase = new FirebaseClient(FirebaseURL);

            Toast.MakeText(this, "Details Updated.", ToastLength.Short).Show();
        }
    }
}