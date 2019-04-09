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
    public class PaymentConfirmed : EventArgs
    {
        public bool paid;

        public bool userPaid
        {
          get { return paid; }
          set { paid = value; }
        }

        public PaymentConfirmed(bool paymentConfirmed) : base()
        {
            userPaid = paymentConfirmed;
        }
    }

    public class Dialogclass : DialogFragment
    {
        bool paid;
        Button confirmPayment, cancelPayment;
        public event EventHandler<PaymentConfirmed> onPaymentComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.confirmPayment_popup, container, false);

            confirmPayment = view.FindViewById<Button>(Resource.Id.btnConfirmPayment);
            cancelPayment = view.FindViewById<Button>(Resource.Id.btnCancelPayment);

            confirmPayment.Click += ConfirmPayment_Click;
            cancelPayment.Click += CancelPayment_Click;

            return view;

        }

        private void CancelPayment_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void ConfirmPayment_Click(object sender, EventArgs e)
        {
            paid = true;
            // payment confirmed by user
            onPaymentComplete.Invoke(this, new PaymentConfirmed(paid));
            this.Dismiss();
        }
    }
}