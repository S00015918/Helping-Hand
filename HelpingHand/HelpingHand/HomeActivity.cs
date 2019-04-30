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
using HelpingHand.Adapter;
using XamarinFirebaseAuth;

namespace HelpingHand
{
    [Activity(Label = "Home", Theme = "@style/AppTheme")]
    public class HomeActivity : AppCompatActivity
    {
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        FirebaseAuth auth;
        GridView gridView;
        // Display titles of each item in the gridview
        string[] gridViewString = {
            "Dashboard", "Messages",
            "Profile", "Calendar",
            "Favourites", "Map",
            "Cancel Appointment", "Logout"
        };

        // also set an image for each of the items in the gridview
        int[] imageId = {
            Resource.Drawable.dashboard , Resource.Drawable.messages,
            Resource.Drawable.user, Resource.Drawable.calendar,
            Resource.Drawable.favourites, Resource.Drawable.map,
            Resource.Drawable.cancel, Resource.Drawable.logout
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Home";

            CustomGridViewAdapter adapter = new CustomGridViewAdapter(this, gridViewString, imageId);
            gridView = FindViewById<GridView>(Resource.Id.gridView_image);

            gridView.SetVerticalSpacing(1);
            gridView.SetHorizontalSpacing(1);
            gridView.Adapter = adapter;
            // depending on which item has been clicked - navigate to the corresponding item
            gridView.ItemClick += (s, e) =>
            {
                Toast.MakeText(this, gridViewString[e.Position], ToastLength.Long).Show();
                int clicked = e.Position;

                if (clicked == 0)
                {
                    StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                    Finish();
                }
                else if (clicked == 1)
                {
                    StartActivity(new Android.Content.Intent(this, typeof(MessageActivity)));
                    Finish();
                }
                else if (clicked == 2)
                {
                    StartActivity(new Android.Content.Intent(this, typeof(userProfile)));
                    Finish();
                }
                else if (clicked == 3)
                {
                    StartActivity(new Android.Content.Intent(this, typeof(userSchedule)));
                    Finish();
                }
                else if (clicked == 4)
                {
                    StartActivity(new Android.Content.Intent(this, typeof(userFavourites)));
                    Finish();
                }
                else if (clicked == 5)
                {
                    StartActivity(new Android.Content.Intent(this, typeof(MapActivity)));
                    Finish();
                }
                else if (clicked == 6)
                {
                    StartActivity(new Android.Content.Intent(this, typeof(CancelAppointment)));
                    Finish();
                }
                else if (clicked == 7)
                {
                    // Log out of the app
                    auth = FirebaseAuth.GetInstance(MainActivity.app);
                    FirebaseUser user = auth.CurrentUser;
                    auth.SignOut();
                    StartActivity(new Android.Content.Intent(this, typeof(MainActivity)));
                    Finish();
                }
            };
        }
    }
}