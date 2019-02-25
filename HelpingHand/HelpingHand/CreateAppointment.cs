using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using HelpingHand.Model;
using Newtonsoft.Json;
using static Android.Views.View;

namespace HelpingHand
{
    [Activity(Label = "Create Booking", Theme = "@style/AppTheme")]
    public class CreateAppointment : Activity, IOnClickListener
    {
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;
        RelativeLayout activity_book_appointment;
        DatePicker datePicker;
        TimePicker timePicker;
        FloatingActionButton btnCreateApointment;
        string selectedTime, selectedDate;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // set the view
            SetContentView(Resource.Layout.bookAppointment);
            activity_book_appointment = FindViewById<RelativeLayout>(Resource.Id.activity_book_appointment);
            datePicker = FindViewById<DatePicker>(Resource.Id.datePicker);
            timePicker = FindViewById<TimePicker>(Resource.Id.timePicker);
            btnCreateApointment = FindViewById<FloatingActionButton>(Resource.Id.btnCreateAppointment);

            var btnChangeDate = FindViewById<Button>(Resource.Id.btnChange_date);
            var btnChangeTime = FindViewById<Button>(Resource.Id.btnChange_time);
            var txtDate = FindViewById<TextView>(Resource.Id.textViewDate);
            var txtTime = FindViewById<TextView>(Resource.Id.textViewTime);

            txtDate.Text = (getDate());
            btnChangeDate.Click += (s, e) =>
            {
                txtDate.Text = getDate();
            };

            btnChangeTime.Click += (s, e) =>
            {
                txtTime.Text = getTime();
            };
            timePicker.SetIs24HourView(Java.Lang.Boolean.True);
            txtTime.Text = getTime();

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            btnCreateApointment.SetOnClickListener(this);
        }

        public string getTime()
        {
            StringBuilder strTime = new StringBuilder();
            strTime.Append("Time: " + timePicker.Hour + ":" + timePicker.Minute);
            selectedTime = strTime.ToString();
            return strTime.ToString();
        }

        public string getDate()
        {
            StringBuilder strCurrentDate = new StringBuilder();
            int month = datePicker.Month + 1;
            strCurrentDate.Append("Date : " + month + "/" + datePicker.DayOfMonth + "/" + datePicker.Year);
            selectedDate = strCurrentDate.ToString();
            return strCurrentDate.ToString();
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.btnCreateAppointment)
            {
                CreateNewAppointment();
                StartActivity(new Android.Content.Intent(this, typeof(userSchedule)));
                Finish();
            }
        }

        private async void CreateNewAppointment()
        {
            string babysitter = this.Intent.GetStringExtra("KEY");

            BabySitter userAppointment = JsonConvert.DeserializeObject<BabySitter>(babysitter);

            FirebaseUser user = auth.CurrentUser;
            string name = userAppointment.name;
            string city = userAppointment.city;
            string eircode = userAppointment.eircode;
            string address = userAppointment.address;

            Appointment appointment = new Appointment();
            appointment.Parent = user.DisplayName;
            appointment.Date = selectedDate;
            appointment.Time = selectedTime;
            //appointment.Color = TitleColor(gre)
            appointment.Babysitter = name;
            appointment.City = city;
            appointment.Address = address;
            appointment.Eircode = eircode;


            var firebase = new FirebaseClient(FirebaseURL);
            ////Add Item
            var item = await firebase.Child("appointment").PostAsync<Appointment>(appointment);
        }
    }
}