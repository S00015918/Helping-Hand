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
using Java.Text;
using Java.Util;
using Newtonsoft.Json;
using static Android.App.TimePickerDialog;
using static Android.Views.View;

namespace HelpingHand
{
    [Activity(Label = "Create Booking", Theme = "@style/AppTheme")]
    public class CreateAppointment : Activity, IOnClickListener, IOnTimeSetListener
    {
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;
        RelativeLayout activity_book_appointment;
        DatePicker datePicker;
        FloatingActionButton btnCreateApointment;
        string selectedStartTime, selectedEndTime;
        string selectedDate;
        private const int StartTimeDialog = 1;
        private const int EndTimeDialog = 2;
        private int hour = 7;
        private int minutes = 0;
        public string strStart, strEnd;
        public int selection;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // set the view
            SetContentView(Resource.Layout.bookAppointment);
            activity_book_appointment = FindViewById<RelativeLayout>(Resource.Id.activity_book_appointment);
            datePicker = FindViewById<DatePicker>(Resource.Id.datePicker);
            btnCreateApointment = FindViewById<FloatingActionButton>(Resource.Id.btnCreateAppointment);

            var btnChangeDate = FindViewById<Button>(Resource.Id.btnChange_date);
            var btnChangeTime = FindViewById<Button>(Resource.Id.btnChange_time);
            var btnChangeEndTime = FindViewById<Button>(Resource.Id.btnChange_Endtime);
            var txtDate = FindViewById<TextView>(Resource.Id.textViewDate);

            txtDate.Text = (getDate());
            btnChangeDate.Click += (s, e) =>
            {
                txtDate.Text = getDate();
            };

            btnChangeTime.Click += delegate
            {
                selection = 1;
                ShowDialog(StartTimeDialog);
                //txtStartTime.Text = getStartTime();
            };
            //timePicker.SetIs24HourView(Java.Lang.Boolean.True);

            btnChangeEndTime.Click += (s, e) =>
            {
                selection = 2;
                ShowDialog(EndTimeDialog);
                //txtEndTime.Text = getEndTime();
            };
            //endTimePicker.SetIs24HourView(Java.Lang.Boolean.True);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            btnCreateApointment.SetOnClickListener(this);
        }
        protected override Dialog OnCreateDialog(int id)
        {
            switch (id)
            {
                case StartTimeDialog:
                    {
                        TimePickerDialog startTime = new TimePickerDialog(this, this, hour, minutes, true);
                        return startTime;
                    }
                case EndTimeDialog:
                    {
                        TimePickerDialog EndTime = new TimePickerDialog(this, this, hour, minutes, true);
                        return EndTime;
                        //return new TimePickerDialog(this, this, hour, minutes, true);
                    }
                default:
                    break;
            }
            return null;
        }

        public void OnTimeSet(Android.Widget.TimePicker view, int hourOfDay, int minutesInHour)
        {
            if (StartTimeDialog == selection)
            {
                var txtStartTime = FindViewById<TextView>(Resource.Id.textViewTime);

                StringBuilder strTime = new StringBuilder();
                hour = hourOfDay;
                minutes = minutesInHour;

                SimpleDateFormat timeFormat = new SimpleDateFormat("hh:mm");
                //Date date = new Date(0, 0, 0, hour, minutes);
                string sHour = Convert.ToString(hour);
                string sMinutes = Convert.ToString(minutes);
                strStart = sHour + ":" + sMinutes;

                selectedStartTime = strStart.ToString();
                Toast.MakeText(this, "You have selected: " + strStart, ToastLength.Short).Show();
                getStartTime(strStart);
                strTime.Append(strStart);

                txtStartTime.Text = getStartTime(strStart);
            }
            else
            {
                var txtEndTime = FindViewById<TextView>(Resource.Id.textViewEndTime);

                StringBuilder strEndTime = new StringBuilder();

                hour = hourOfDay;
                minutes = minutesInHour;

                SimpleDateFormat timeFormat = new SimpleDateFormat("hh:mm");
                string eHour = Convert.ToString(hour);
                string eMinutes = Convert.ToString(minutes);
                strEnd = eHour + ":" + eMinutes;

                selectedEndTime = strEnd.ToString();
                Toast.MakeText(this, "You have selected: " + strEnd, ToastLength.Short).Show();
                getEndTime(strEnd);
                strEndTime.Append(strEnd);

                txtEndTime.Text = getEndTime(strEnd);
            }
        }

        private string getEndTime(string endTime)
        {
            StringBuilder strEndTime = new StringBuilder();
            strEndTime.Append(endTime);
            selectedEndTime = strEndTime.ToString();
            return strEndTime.ToString();
        }

        public string getStartTime(string startTime)
        {
            StringBuilder strTime = new StringBuilder();
            strTime.Append(startTime);
            selectedStartTime = strTime.ToString();
            return strTime.ToString();
        }

        public string getDate()
        {
            StringBuilder strCurrentDate = new StringBuilder();
            int month = datePicker.Month + 1;
            strCurrentDate.Append("Date : " + month + "/" + datePicker.DayOfMonth + "/" + datePicker.Year);
            int day = datePicker.DayOfMonth;
            int _month = month;
            int year = datePicker.Year;
            selectedDate = datePicker.DayOfMonth + "/" + month + "/" + datePicker.Year;
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
            appointment.Date = Convert.ToDateTime(selectedDate);
            appointment.startTime = selectedStartTime;
            appointment.endTime = selectedEndTime;
            appointment.userEmail = auth.CurrentUser.Email;
            appointment.babysitterEmail = userAppointment.email;
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