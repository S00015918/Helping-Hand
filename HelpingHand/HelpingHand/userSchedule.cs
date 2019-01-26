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
using XamarinFirebaseAuth;

namespace HelpingHand
{
    [Activity(Label = "Schedule", Theme = "@style/AppTheme")]
    public class userSchedule : AppCompatActivity
    {
        //RelativeLayout activity_schedule;
        FirebaseAuth auth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.schedule_view);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //Add Toolbar
            //var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);

            var calendarView = FindViewById<CalendarView>(Resource.Id.calendar);

            calendarView.DateChange += (s, e) => {
                int day = e.DayOfMonth;
                int month = e.Month;
                int year = e.Year;
            };
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
                StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                Finish();
            }
            if (id == Resource.Id.menu_message) //messages
            {
                StartActivity(new Android.Content.Intent(this, typeof(MessageActivity)));
                Finish();
            }

            else if (id == Resource.Id.menu_user) //user profile
            {
                StartActivity(new Android.Content.Intent(this, typeof(userProfile)));
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}