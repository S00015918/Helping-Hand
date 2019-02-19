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
    public class StripeServices : IStripeServices
    {
        public string CardToToken(CreditCard creditCard)
        {
            var stripeTokenCreateOptions = new StripeTokenCreateOptions
            {
                Card = new StripeCreditCardOptions
                {
                    Number = creditCard.CardNumber,
                    ExpirationMonth = creditCard.Month,
                    ExpirationYear = creditCard.Year,
                    Cvc = creditCard.Cvc
                }
            };

            var tokenService = new StripeTokenService();
            var stripeToken = tokenService.Create(stripeTokenCreateOptions);

            return creditCard.CardNumber;
        }
    }
}