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


namespace HelpingHand.Model
{
    public class CancelConfirmed : EventArgs
    {
        public bool cancelled;
        public bool userConfirmed
        {
            get { return cancelled; }
            set { cancelled = value; }
        }

        public CancelConfirmed(bool cancelConfirmed) : base()
        {
            userConfirmed = cancelConfirmed;
        }
    }

    class CancelAppointmentDialog : DialogFragment
    {
        bool cancelled;
        Button cancel_app, close;
        public event EventHandler<CancelConfirmed> onCancelComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.CancelAppointment_fragment, container, false);

            cancel_app = view.FindViewById<Button>(Resource.Id.btnCancelAppointment);
            close = view.FindViewById<Button>(Resource.Id.btnCloseCancel);

            cancel_app.Click += Cancel_app_Click;
            close.Click += Close_Click;

            return view;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void Cancel_app_Click(object sender, EventArgs e)
        {
            cancelled = true;
            onCancelComplete.Invoke(this, new CancelConfirmed(cancelled));
        }
    }
}