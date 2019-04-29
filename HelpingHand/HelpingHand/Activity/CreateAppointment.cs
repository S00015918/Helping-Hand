using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
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
    public class CreateAppointment : AppCompatActivity, IOnClickListener, IOnTimeSetListener
    {
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;
        RelativeLayout activity_book_appointment;
        DatePicker datePicker;
        FloatingActionButton btnCreateApointment;
        string selectedStartTime, selectedEndTime, strStart, strEnd;
        string selectedDate, timeOfDay;
        private const int StartTimeDialog = 1;
        private const int EndTimeDialog = 2;
        private int hour = 7;
        private int minutes = 0;
        public int selection;
        decimal amountDue;
        DateTime validDate;
        List<Appointment> list_appointments = new List<Appointment>();
        bool refresh, confirmed;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // set the view
            SetContentView(Resource.Layout.bookAppointment);
            activity_book_appointment = FindViewById<RelativeLayout>(Resource.Id.activity_book_appointment);
            datePicker = FindViewById<DatePicker>(Resource.Id.datePicker);
            datePicker.MinDate= Java.Lang.JavaSystem.CurrentTimeMillis();

            btnCreateApointment = FindViewById<FloatingActionButton>(Resource.Id.btnCreateAppointment);

            var btnChangeDate = FindViewById<Button>(Resource.Id.btnChange_date);
            var btnChangeTime = FindViewById<Button>(Resource.Id.btnChange_time);
            var btnChangeEndTime = FindViewById<Button>(Resource.Id.btnChange_Endtime);
            var txtDate = FindViewById<TextView>(Resource.Id.textViewDate);

            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            txtDate.Text = (getDate());
            btnChangeDate.Click += (s, e) =>
            {
                btnCreateApointment.Enabled = true;
                txtDate.Text = getDate();
            };

            btnChangeTime.Click += delegate
            {
                btnCreateApointment.Enabled = true;
                selection = 1;
                ShowDialog(StartTimeDialog);
            };
            //timePicker.SetIs24HourView(Java.Lang.Boolean.True);

            btnChangeEndTime.Click += (s, e) =>
            {
                btnCreateApointment.Enabled = true;
                selection = 2;
                ShowDialog(EndTimeDialog);
            };
            //endTimePicker.SetIs24HourView(Java.Lang.Boolean.True);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            btnCreateApointment.SetOnClickListener(this);
        }

        private void Appointment_Confirm(decimal cost, DateTime date)
        {
            decimal fees = cost * 10 / 100;
            decimal totalAmount = fees + cost;
            string _amountDue = totalAmount.ToString();
            string _appointmentDate = date.ToString();

            Bundle passCost = new Bundle();
            Bundle passDate = new Bundle();
            passCost.PutString("Cost", _amountDue);
            passDate.PutString("Date", _appointmentDate);

            FragmentTransaction transcation = FragmentManager.BeginTransaction();
            AppointmentDialog confirmAppointment = new AppointmentDialog();
            confirmAppointment.setCost(passCost);
            confirmAppointment.setDate(passDate);
            confirmAppointment.Show(transcation, "Dialog Fragment");
            confirmAppointment.onComplete += ConfirmAppointment_onComplete;
        }

        private void ConfirmAppointment_onComplete(object sender, AppointmentConfirmed e)
        {
            confirmed = e.confirmed;
            if (confirmed == true)
            {
                string babysitter = this.Intent.GetStringExtra("KEY");
                BabySitter userAppointment = JsonConvert.DeserializeObject<BabySitter>(babysitter);
                decimal payRate = userAppointment.rate;

                FirebaseUser user = auth.CurrentUser;
                string name = userAppointment.name;
                string city = userAppointment.city;
                string eircode = userAppointment.eircode;
                string address = userAppointment.address;

                validDate = Convert.ToDateTime(selectedDate);
                DateTime start = Convert.ToDateTime(selectedStartTime);
                DateTime end = Convert.ToDateTime(selectedEndTime);

                int startHour = start.Hour;
                int endHour = end.Hour;
                int totalHours = endHour - startHour;
                amountDue = totalHours * payRate;

                Appointment appointment = new Appointment();
                appointment.Date = Convert.ToDateTime(selectedDate);
                appointment.startTime = selectedStartTime;
                appointment.endTime = selectedEndTime;
                appointment.userEmail = auth.CurrentUser.Email;
                appointment.babysitterEmail = userAppointment.email;
                appointment.Babysitter = name;
                appointment.City = city;
                appointment.Address = address;
                appointment.Eircode = eircode;
                appointment.cost = amountDue;

                var firebase = new FirebaseClient(FirebaseURL);
                var appointmentJson = JsonConvert.SerializeObject(appointment);

                var viewPaymentForm = new Intent(this, typeof(PaymentActivity));
                viewPaymentForm.PutExtra("KEY", appointmentJson);
                StartActivity(viewPaymentForm);
            }
            else
            { // please confirm appointment to progress
            }
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

        public async void getAppointments()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            var appointments = await firebase
                    .Child("appointment")
                    .OnceAsync<Appointment>();

            foreach (var item in appointments)
            {
                Appointment session = new Appointment();
                session.Date = item.Object.Date;
                session.startTime = item.Object.startTime;
                session.endTime = item.Object.endTime;
                list_appointments.Add(session);
            }
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
                getAvailabilty();
            }
        }

        public async void getAvailabilty()
        {
            DateTime dateSelected = Convert.ToDateTime(selectedDate);
            DateTime start = Convert.ToDateTime(selectedStartTime);
            DateTime end = Convert.ToDateTime(selectedEndTime);

            string babysitter = this.Intent.GetStringExtra("KEY");
            BabySitter userAppointment = JsonConvert.DeserializeObject<BabySitter>(babysitter);

            var firebase = new FirebaseClient(FirebaseURL);
            var appointments = await firebase
                    .Child("appointment")
                    .OnceAsync<Appointment>();

            foreach (var item in appointments)
            {
                Appointment session = new Appointment();
                session.Babysitter = item.Object.Babysitter;
                session.Date = item.Object.Date;
                session.startTime = item.Object.startTime;
                session.endTime = item.Object.endTime;
                list_appointments.Add(session);
                string BabysitterName = session.Babysitter.ToString().ToLower();

                if (BabysitterName == userAppointment.name)
                {
                    if (session.Date == dateSelected)
                    {
                        if (start >= Convert.ToDateTime(session.startTime) && start <= Convert.ToDateTime(session.endTime) 
                            || (end >= Convert.ToDateTime(session.startTime) && end <= Convert.ToDateTime(session.endTime)))
                        {
                            // Cannot book appointment because babysitter already has booking
                            Toast.MakeText(this, "Babysitter Unavailable", ToastLength.Short).Show();
                            refresh = true;
                        }
                    }
                }
            }
            if (refresh == true)
            {
                refreshActivity();
            }
            else
            {
                string availabilty = userAppointment.availability; // compare availabilty to book schedule

                var days = availabilty.Split(',');
                if (days == null)
                {
                    string y = Convert.ToString(days + "");
                }

                string[] schedule = days;

                var startHour = start.Hour.ToString();
                int _hour = int.Parse(startHour);

                if (_hour >= 7 && _hour < 12)
                {
                    timeOfDay = "Morning";
                }
                else if (_hour >= 12 && _hour < 16)
                { timeOfDay = "Afternoon"; }

                else if (_hour >= 16 && _hour < 20)
                { timeOfDay = "Evening"; }

                else if (_hour >= 20 && _hour < 24)
                { timeOfDay = "Night"; }

                else if (_hour >= 1 && _hour < 6)
                { timeOfDay = "Un-available"; }

                var today = dateSelected.DayOfWeek.ToString().TrimEnd();
                string todaysTime = today + " " + timeOfDay.ToString();

                var availabiltyList = new List<string>();
                foreach (var item in schedule)
                {
                    string newItem = item.Trim();
                    availabiltyList.Add(newItem);
                }

                if (availabiltyList.Contains(todaysTime))
                {
                    CreateNewAppointment();
                }
                else {
                    Toast.MakeText(this, "Please change appointment, Babysitter not available for selected time", ToastLength.Short).Show();
                }
                if (todaysTime.Contains("Un-available"))
                {
                    Toast.MakeText(this, "Please change appointment, Babysitter not available for selected time", ToastLength.Short).Show();
                }
            }
        }

        private void refreshActivity()
        {
            this.Recreate();
            //StartActivity(new Android.Content.Intent(this, typeof(CreateAppointment)));
            Finish();
        }

        private void CreateNewAppointment()
        {
            string babysitter = this.Intent.GetStringExtra("KEY");
            BabySitter userAppointment = JsonConvert.DeserializeObject<BabySitter>(babysitter);
            decimal payRate = userAppointment.rate;

            validDate = Convert.ToDateTime(selectedDate);
            DateTime start = Convert.ToDateTime(selectedStartTime);
            DateTime end = Convert.ToDateTime(selectedEndTime);

            int startHour = start.Hour;
            int endHour = end.Hour;
            int totalHours = endHour - startHour;
            amountDue = totalHours * payRate;

            if (end < start)
            {
                Toast.MakeText(this, "Please change Appointment end time.", ToastLength.Short).Show();
            }
            else
            {
                Appointment_Confirm(amountDue, validDate);
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
                StartActivity(new Android.Content.Intent(this, typeof(HomeActivity)));
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}