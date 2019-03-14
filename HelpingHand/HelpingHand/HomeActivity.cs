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
using HelpingHand.Adapter;
using XamarinFirebaseAuth;

namespace HelpingHand
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class HomeActivity : AppCompatActivity
    {
        GridView gridView;
        string[] gridViewString = {
            "Dashboard", "Messages",
            "Profile", "Calendar",
            "Faviourites", "Map",
            "About us", "Logout"
        };

        int[] imageId = {
            Resource.Drawable.dashboard , Resource.Drawable.messages,
            Resource.Drawable.user_icon, Resource.Drawable.calendar,
            Resource.Drawable.star, Resource.Drawable.map,
            Resource.Drawable.info, Resource.Drawable.logout
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            //var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);
            //SupportActionBar.Title = "Grid View";

            CustomGridViewAdapter adapter = new CustomGridViewAdapter(this, gridViewString, imageId);
            gridView = FindViewById<GridView>(Resource.Id.gridView_image_text);
            gridView.Adapter = adapter;
            gridView.ItemClick += (s, e) =>
            {
                Toast.MakeText(this, gridViewString[e.Position], ToastLength.Long).Show();
                int clicked = e.Position;

                if (clicked == 0)
                {
                    StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                    Finish();
                }
            };
        }
    }
}