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
        FirebaseAuth auth;
        bool confirmed;
        Button confirmAppointment, cancelAppointment;

        public event EventHandler<AppointmentConfirmed> onComplete;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            var view = inflater.Inflate(Resource.Layout.ConfirmAppointment_fragment, container, false);

            confirmAppointment = view.FindViewById<Button>(Resource.Id.btnConfirm_Appointment);
            cancelAppointment = view.FindViewById<Button>(Resource.Id.btnCancel_Appointment);

            confirmAppointment.Click += ConfirmAppointment_Click;
            cancelAppointment.Click += CancelAppointment_Click;

            return view;

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
    }
}