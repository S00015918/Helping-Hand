using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using HelpingHand.Model;
using Newtonsoft.Json;
using static Android.Views.View;

namespace HelpingHand
{
    [Activity(Label = "Appointment Reciept", Theme = "@style/AppTheme")]
    public class AppointmentRecieptActivity : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        TextView appDate, appStart, appEnd, appCost, appBabysitter, appAddress;
        DateTime Date;
        string startTime, endTime, babysitter, Address, _date, parentEmail;
        decimal Cost;
        Button resendReciept;
        FirebaseAuth auth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Appointment_Confirmed);
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            appDate = FindViewById<TextView>(Resource.Id.appDate);
            appStart = FindViewById<TextView>(Resource.Id.appStart);
            appEnd = FindViewById<TextView>(Resource.Id.appEnd);
            appCost = FindViewById<TextView>(Resource.Id.appCost);
            appAddress = FindViewById<TextView>(Resource.Id.appAddress);
            appBabysitter = FindViewById<TextView>(Resource.Id.appBabysitter);
            resendReciept = FindViewById<Button>(Resource.Id.resend_reciept);

            string appointment = this.Intent.GetStringExtra("KEY");
            Appointment newAppointment = JsonConvert.DeserializeObject<Appointment>(appointment);

            Date = newAppointment.Date;
            _date = Date.ToString();
            string[] dateNotTime = _date.Split(' ');
            string justDate = dateNotTime[0];

            startTime = newAppointment.startTime;
            endTime = newAppointment.endTime;
            babysitter = newAppointment.Babysitter;
            Address = newAppointment.Address;
            Cost = newAppointment.cost;
            parentEmail = newAppointment.userEmail;

            appDate.Text = "Appointment date: " + justDate;
            appStart.Text = "Appointment starts: " + startTime;
            appEnd.Text = "Appointment ends: " + endTime;
            appBabysitter.Text = "Baysitter name: " + babysitter;
            appAddress.Text = "Appointment location: " + Address;
            appCost.Text = "Appointment cost: " + Cost.ToString();

            resendReciept.SetOnClickListener(this);
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

        public void OnComplete(Task task)
        {
           
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.resend_reciept)
            {
                ResendConfirmation(parentEmail);
            }
        }

        private void ResendConfirmation(string parentEmail)
        {
            Toast.MakeText(this, "Confirmation sent to: "+ parentEmail, ToastLength.Short).Show();
            auth.SendPasswordResetEmail(parentEmail).AddOnCompleteListener(this, this);
        }
    }
}