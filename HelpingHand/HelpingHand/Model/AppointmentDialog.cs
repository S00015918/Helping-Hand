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
using Firebase.Xamarin.Database;
using Newtonsoft.Json;

namespace HelpingHand.Model
{
    public class AppointmentConfirmed : EventArgs
    {
        public bool confirmed;

        public bool userConfirmed
        {
            get { return confirmed; }
            set { confirmed = value; }
        }

        public AppointmentConfirmed(bool appointmentConfirmed) : base()
        {
            userConfirmed = appointmentConfirmed;
        }
    }

    class AppointmentDialog : DialogFragment
    {
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;
        bool confirmed;
        string cost, date;
        Button confirmAppointment, cancelAppointment;
        TextView appointment_date, appointment_cost;

        public event EventHandler<AppointmentConfirmed> onComplete;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            var view = inflater.Inflate(Resource.Layout.ConfirmAppointment_fragment, container, false);

            confirmAppointment = view.FindViewById<Button>(Resource.Id.btnConfirm_Appointment);
            cancelAppointment = view.FindViewById<Button>(Resource.Id.btnCancel_Appointment);

            appointment_cost = view.FindViewById<TextView>(Resource.Id.txtCost);
            appointment_date = view.FindViewById<TextView>(Resource.Id.txtDate);

            // This code below will trim down the cost and date of an appointment to its core values that i want to display
            string[] _Cost = cost.Split('{');
            string[] _Date = date.Split('{');
            string appCost = _Cost[1];
            string appDate = _Date[1];
            string[] getCost = appCost.Split('}');
            string[] getDate = appDate.Split(' ');
            string appointmentCost = getCost[0];
            string appointmentDate = getDate[0];

            appointment_cost.Text = appointmentCost;
            appointment_date.Text = appointmentDate;

            confirmAppointment.Click += ConfirmAppointment_Click;
            cancelAppointment.Click += CancelAppointment_Click;

            return view;
        }

        private async void showData()
        {
            var firebase = new FirebaseClient(FirebaseURL);

            var appointments = await firebase
                .Child("appointment")
                .OnceAsync<Appointment>();

            foreach (var item in appointments)
            {
                Appointment appointment = new Appointment();
                decimal appCost = appointment.cost;
                DateTime appDate = appointment.Date;
                string appParentEmail = appointment.userEmail;
            }
        }

        private void CancelAppointment_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void ConfirmAppointment_Click(object sender, EventArgs e)
        {
            confirmed = true;
            // payment confirmed by user
            onComplete.Invoke(this, new AppointmentConfirmed(confirmed));
            this.Dismiss();
        }

        internal void setCost(Bundle passCost)
        {
            cost = passCost.ToString();

        }
        internal void setDate(Bundle passDate)
        {
            date = passDate.ToString();
        }
    }
}