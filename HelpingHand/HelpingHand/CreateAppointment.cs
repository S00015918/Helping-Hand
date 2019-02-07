using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;

namespace HelpingHand
{
    [Activity(Label = "Create Booking", Theme = "@style/AppTheme")]
    public class CreateAppointment : Activity
    {
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;
        RelativeLayout activity_book_appointment;
        DatePicker datePicker;
        TimePicker timePicker;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // set the view
            SetContentView(Resource.Layout.bookAppointment);
            activity_book_appointment = FindViewById<RelativeLayout>(Resource.Id.activity_book_appointment);
            datePicker = FindViewById<DatePicker>(Resource.Id.datePicker);
            //timePicker = FindViewById<TimePicker>(Resource.Id.timePicker);

            var btnChangeDate = FindViewById<Button>(Resource.Id.btnChange_date);
            //var btnChangeTime = FindViewById<Button>(Resource.Id.btnChange_time);
            var txtDate = FindViewById<TextView>(Resource.Id.textViewDate);
            //var txtTime = FindViewById<TextView>(Resource.Id.textViewTime);

            txtDate.Text = (getDate());
            btnChangeDate.Click += (s, e) =>
            {
                txtDate.Text = getDate();
            };

            //btnChangeTime.Click += (s, e) =>
            //{
            //    txtTime.Text = getTime();
            //};
            //timePicker.SetIs24HourView(Java.Lang.Boolean.True);
            //txtTime.Text = getTime();

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);
        }

        private string getTime()
        {
            StringBuilder strTime = new StringBuilder();
            strTime.Append("Time: " + timePicker.Hour + ":" + timePicker.Minute);
            return strTime.ToString();
        }

        private string getDate()
        {
            StringBuilder strCurrentDate = new StringBuilder();
            int month = datePicker.Month + 1;
            strCurrentDate.Append("Date : " + month + "/" + datePicker.DayOfMonth + "/" + datePicker.Year);
            return strCurrentDate.ToString();
        }
    }
}