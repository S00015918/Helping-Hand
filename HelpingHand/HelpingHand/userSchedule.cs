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
using Com.Syncfusion.Calendar.Enums;
using Java.Util;

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
            sfCalendar.ViewMode = ViewMode.MonthView;
            SetContentView(sfCalendar);

            sfCalendar.TransitionMode = TransitionMode.Card;
            sfCalendar.ShowEventsInline = true;
            Calendar currentDate = Calendar.Instance;
            //SetContentView(Resource.Layout.schedule_view);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var firebase = new FirebaseClient(FirebaseURL);

            var appointments = await firebase
                    .Child("appointment")
                    .OnceAsync<Appointment>();

            foreach (var item in appointments)
            {
                Appointment appointment = new Appointment();
                appointment.Babysitter = item.Object.Babysitter;
                appointment.userEmail = item.Object.userEmail;
                appointment.babysitterEmail = item.Object.babysitterEmail;
                appointment.Date = item.Object.Date;
                appointment.startTime = item.Object.startTime;
                appointment.endTime = item.Object.endTime;
                appointment.Address = item.Object.Address;
                appointment.City = item.Object.City;
                list_appointments.Add(appointment);

                //if (users.Any((_) => _.Key == auth.CurrentUser.Uid))
                if (appointment.userEmail == auth.CurrentUser.Email)
                {
                    // You are logged in as parent
                    CalendarInlineEvent _event = new CalendarInlineEvent();
                    DateTime start = appointment.Date;
                    int year = start.Year;
                    int month = start.Month - 1;
                    int day = start.Day;

                    string time = appointment.startTime;
                    var startTime = Array.ConvertAll(time.Split(':'), int.Parse).ToList();
                    int hour = startTime[0];
                    int minute = startTime[1];

                    string endTime = appointment.endTime;
                    var end = Array.ConvertAll(endTime.Split(':'), int.Parse).ToList();
                    int endHour = end[0];
                    int endMinute = end[1];

                    _event.Subject = appointment.Babysitter + ", " + appointment.startTime + ", " + appointment.Address;
                    _event.Color = Android.Graphics.Color.LightBlue;

                    //starting date of event
                    Calendar startEventDate = Calendar.Instance;
                    startEventDate.Set(year, month, day, hour, minute);
                    _event.StartTime = startEventDate;

                    //ending date of event
                    Calendar endEventDate = Calendar.Instance;
                    endEventDate.Set(year, month, day, endHour, endMinute);
                    _event.EndTime = endEventDate;

                    CalendarEventCollection eventCollection = new CalendarEventCollection();
                    eventCollection.Add(_event);
                    sfCalendar.DataSource = eventCollection;
                }
                if (appointment.babysitterEmail == auth.CurrentUser.Email)
                {
                    // you are logged in as babysitter
                    CalendarInlineEvent _event = new CalendarInlineEvent();
                    DateTime start = appointment.Date;
                    string time = appointment.startTime;
                    _event.Subject = appointment.Address + ", " + appointment.startTime + ", " + appointment.userEmail;
                    _event.StartTime.Equals(start).ToString();
                    _event.Color.Equals(ConsoleColor.DarkGreen);

                    CalendarEventCollection eventCollection = new CalendarEventCollection();
                    eventCollection.Add(_event);
                    sfCalendar.DataSource = eventCollection;
                }
            }
            MonthViewSettings monthViewSettings = new MonthViewSettings();
            monthViewSettings.TodayTextColor.Equals("#1B79D6");
            //monthViewSettings.InlineBackgroundColor = Android.Graphics.Color.ParseColor("#cee4e5");
            monthViewSettings.DateSelectionColor = Android.Graphics.Color.ParseColor("#cee4e5");
            sfCalendar.MonthViewSettings = monthViewSettings;

            sfCalendar.AddDatesInPast();

            //sfCalendar.InlineItemLoaded += Calendar_InlineItemLoaded;
        }

        private void Calendar_InlineItemLoaded(object sender, InlineItemLoadedEventArgs e)
        {
            var appointment = e.CalendarInlineEvent;
            Button button = new Button(this);
            button.Text = "Appointment :" + appointment.Subject;
            button.SetBackgroundColor(Android.Graphics.Color.LightBlue);
            button.SetTextColor(Android.Graphics.Color.LightGray);
            e.View = button;
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