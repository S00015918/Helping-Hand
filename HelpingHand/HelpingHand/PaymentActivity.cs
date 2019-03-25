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
using Stripe;

namespace HelpingHand
{
    [Activity(Label = "PaymentActivity")]
    public class PaymentActivity : Activity
    {
        FirebaseAuth auth;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.payment_form);
            auth = FirebaseAuth.GetInstance(MainActivity.app);
        }
    }

    public class MakePayment
    {
        public string CreateToken(string cardNumber, string cardExpMonth, string cardExpYear, string cardCVC)
        {
            StripeConfiguration.SetApiKey("pk_test_xxxxxxxxxxxxxxxxx");

            var tokenOptions = new StripeTokenCreateOptions()
            {
                Card = new StripeCreditCardOptions()
                {
                    Number = cardNumber,
                    ExpirationYear = cardExpYear,
                    ExpirationMonth = cardExpMonth,
                    Cvc = cardCVC
                }
            };

            var tokenService = new StripeTokenService();
            //StripeToken stripeToken = tokenService.Create(tokenOptions);

            //return stripeToken.Id; // This is the token
            return null;
        }
    }

    internal class StripeTokenService
    {
        public StripeTokenService()
        {

        }

        internal object Create(StripeTokenCreateOptions stripeTokenCreateOptions)
        {
            throw new NotImplementedException();
        }
    }
}