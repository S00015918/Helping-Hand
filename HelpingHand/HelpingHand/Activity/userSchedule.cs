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
using Newtonsoft.Json;

namespace HelpingHand
{
    [Activity(Label = "Appointments Schedule", Theme = "@style/AppTheme")]
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

            SetContentView(Resource.Layout.calendar);
            //calendar = FindViewById<SfCalendar>(Resource.Id.calendar);
            SfCalendar calendar = (SfCalendar)FindViewById(Resource.Id.sfCalendar);
            //SfCalendar calendar = new SfCalendar(this);
            Calendar minCalendar = Calendar.Instance;
            minCalendar.Set(2018, 12, 12);
            calendar.MinDate = minCalendar;           
            calendar.ViewMode = ViewMode.MonthView;
            //SetContentView(calendar);

            calendar.TransitionMode = TransitionMode.Card;
            calendar.ShowEventsInline = true;
            Calendar currentDate = Calendar.Instance;
            CalendarEventCollection eventCollection = new CalendarEventCollection();
            //SetContentView(Resource.Layout.schedule_view);
            MonthViewLabelSetting labelSettings = new MonthViewLabelSetting();
            labelSettings.DateFormat = "dd";
            labelSettings.DayLabelSize = 10;
            labelSettings.DayFormat = "EEE";
            labelSettings.DateLabelSize = 12;

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
                appointment.Eircode = item.Object.Eircode;
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

                    _event.Subject = appointment.startTime + " - " + appointment.endTime + ", " + appointment.Address + ", " + appointment.City + ", " + appointment.Eircode + ", Babysitters Name: " + appointment.Babysitter;
                    _event.Color = Android.Graphics.Color.LightBlue;

                    //starting date of event
                    Calendar startEventDate = Calendar.Instance;
                    startEventDate.Set(year, month, day, hour, minute);
                    _event.StartTime = startEventDate;

                    //ending date of event
                    Calendar endEventDate = Calendar.Instance;
                    endEventDate.Set(year, month, day, endHour, endMinute);
                    _event.EndTime = endEventDate;
                 
                    eventCollection.Add(_event);
                    calendar.DataSource = eventCollection;
                }
                if (appointment.babysitterEmail == auth.CurrentUser.Email)
                {
                    // you are logged in as babysitter
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

                    _event.Subject = appointment.startTime + " - " + appointment.endTime + ", " + appointment.Address + ", " + appointment.City + ", " + appointment.Eircode + ", " + appointment.userEmail;
                    _event.Color = Android.Graphics.Color.LightBlue;

                    //starting date of event
                    Calendar startEventDate = Calendar.Instance;
                    startEventDate.Set(year, month, day, hour, minute);
                    _event.StartTime = startEventDate;

                    //ending date of event
                    Calendar endEventDate = Calendar.Instance;
                    endEventDate.Set(year, month, day, endHour, endMinute);
                    _event.EndTime = endEventDate;

                    eventCollection.Add(_event);
                    calendar.DataSource = eventCollection;
                }
            }
            MonthViewSettings monthViewSettings = new MonthViewSettings();
            monthViewSettings.TodayTextColor.Equals("#1B79D6");
            //monthViewSettings.InlineBackgroundColor = Android.Graphics.Color.ParseColor("#cee4e5");
            monthViewSettings.DateSelectionColor = Android.Graphics.Color.ParseColor("#cee4e5");
            calendar.MonthViewSettings = monthViewSettings;

            calendar.AddDatesInPast();

            //calendar.InlineItemLoaded += Calendar_InlineItemLoaded;
            calendar.InlineItemTapped += Calendar_InlineItemTapped;
        }

        private void Calendar_InlineItemTapped(object sender, InlineItemTappedEventArgs e)
        {
            var appointment = e.InlineEvent;
            var userJson = JsonConvert.SerializeObject(appointment);

            Toast.MakeText(this, appointment.Subject + " - " + appointment.StartTime.Time.ToString(), ToastLength.Long).Show();

            var mapAddress = new Intent(this, typeof(MapWithMarkersActivity));
            mapAddress.PutExtra("KEY", Convert.ToString(userJson));
            StartActivity(mapAddress);
            //showMap();
        }

        private void Calendar_InlineItemLoaded(object sender, InlineItemLoadedEventArgs e)
        {
            var appointment = e.CalendarInlineEvent;
            Button button = new Button(this);
            button.Text = "Appointment : " + appointment.StartTime + " - " + appointment.EndTime + "&#10; - "
                + appointment.Subject;
            //button.SetBackgroundColor(Android.Graphics.Color.LightBlue);
            //button.SetTextColor(Android.Graphics.Color.LightGray);
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
                StartActivity(new Android.Content.Intent(this, typeof(HomeActivity)));
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}