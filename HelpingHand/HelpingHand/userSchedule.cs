using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Firebase.Xamarin.Database;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using XamarinFirebaseAuth;
using Com.Syncfusion.Calendar;
using HelpingHand.Model;

namespace HelpingHand
{
    [Activity(Label = "Schedule", Theme = "@style/AppTheme")]
    public class userSchedule : AppCompatActivity
    {
        //RelativeLayout activity_schedule;
        FirebaseAuth auth;
        //private ListView list_data;
        List<Appointment> list_appointments = new List<Appointment>();

        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SfCalendar sfCalendar = new SfCalendar(this);
            SetContentView(sfCalendar);
            //SetContentView(Resource.Layout.schedule_view);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //SfCalendar calendar = new SfCalendar(this);
            List<DateTime> black_dates = new List<DateTime>();
            DateTime today = DateTime.Now.Date;
            for (int i = 0; i < 5; i++)
            {
                DateTime date = DateTime.Now.Date.AddDays(i - 5);
                black_dates.Add(date);
            }
            //sfCalendar.BlackoutDates = black_dates;
            //var calendarView = FindViewById<CalendarView>(Resource.Id.calendar);

            //calendarView.DateChange += (s, e) => {
            //    int day = e.DayOfMonth;
            //    int month = e.Month;
            //    int year = e.Year;
            //};
            var firebase = new FirebaseClient(FirebaseURL);
            var appointments = await firebase
                    .Child("appointment")
                    .OnceAsync<Appointment>();

            foreach (var item in appointments)
            {
                Appointment appointment = new Appointment();
                appointment.Date = item.Object.Date;
                appointment.Time = item.Object.Time;
                appointment.Address = item.Object.Address;
                appointment.City = item.Object.City;
                //list_parents.Add(account);
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