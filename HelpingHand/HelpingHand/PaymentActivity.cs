using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Wallet;
using Android.Gms.Wallet.Fragment;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using HelpingHand.Model;
using Stripe;

namespace HelpingHand
{
    [Activity(Label = "PaymentActivity")]
    public class PaymentActivity : Activity
    {
        FirebaseAuth auth;
        EditText creditCardNumber, cardExpiryMonth, cardExpiryYear, cardCVV;
        Button AcceptPayment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.payment_form);
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            AcceptPayment = FindViewById<Button>(Resource.Id.btnAccept);
            creditCardNumber = FindViewById<EditText>(Resource.Id.txtCreditCardNumber);

            creditCardNumber.AddTextChangedListener(new CreditCardFormatter(creditCardNumber));

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);
            //SupportActionBar.Title = "Home";

            //ChargeCard();
            AcceptPayment.Click += delegate
            {
                ChargeCard();
                MakeStripePayment();
            };
        }

        public void ChargeCard()
        {
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.SetApiKey("sk_test_LMuAkBgF8zxl2ha3G66Yygdq00s09S6uzP");

            PaymentModel payment = new PaymentModel();
            var token = payment.Token;
            var amountCharged = payment.Amount;

            var options = new ChargeCreateOptions
            {
                Amount = 999, //Convert.ToInt32(amountCharged * 100), //for cents
                Currency = "eur",
                SourceId = "tok_visa", // token
                Description = "Babysitter Hired",
                ReceiptEmail = auth.CurrentUser.Email,
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);
        }

        //public async void ChargeCard()
        //{
        //    try
        //    {
        //        // Set your secret key: remember to change this to your live secret key in production
        //        // See your keys here: https://dashboard.stripe.com/account/apikeys //call stripe to process the charge
        //        StripeConfiguration.SetApiKey("TESTKEYHERE");
        //        // var card = new Card { h}

        //        var options = new ChargeCreateOptions
        //        {
        //            Amount = Convert.ToInt32(AMOUNTTOCHARGE * 100),//for cents
        //            Currency = "eur",//charge in euro
        //            SourceId = "tok_visa",

        //            ReceiptEmail = "",
        //            Description = "Purchase amount " + CartPrice
        //        };
        //        var service = new ChargeService();
        //        Charge charge = service.Create(options);

        //        await DisplayAlert("Alert", "Payment of €" + CartPrice.ToString("0.00") + " Successful! Thank You", "Ok");
        //    }
        //    catch (StripeException ex)
        //    {
        //        switch (ex.StripeError.ErrorType)
        //        {
        //            case "card_error":
        //                await DisplayAlert("Error", "Payment Declined!!!! " + ex.StripeError.Message, "Ok");
        //                System.Diagnostics.Debug.WriteLine("   Code: " + ex.StripeError.Code);
        //                System.Diagnostics.Debug.WriteLine("Message: " + ex.StripeError.Message);
        //                break;
        //            case "api_connection_error":
        //                System.Diagnostics.Debug.WriteLine(" apic  Code: " + ex.StripeError.Code);
        //                System.Diagnostics.Debug.WriteLine("apic Message: " + ex.StripeError.Message);
        //                break;
        //            case "api_error":
        //                System.Diagnostics.Debug.WriteLine("api   Code: " + ex.StripeError.Code);
        //                System.Diagnostics.Debug.WriteLine("api Message: " + ex.StripeError.Message);
        //                break;
        //            case "authentication_error":
        //                System.Diagnostics.Debug.WriteLine(" auth  Code: " + ex.StripeError.Code);
        //                System.Diagnostics.Debug.WriteLine("auth Message: " + ex.StripeError.Message);
        //                break;
        //            case "invalid_request_error":
        //                System.Diagnostics.Debug.WriteLine(" invreq  Code: " + ex.StripeError.Code);
        //                System.Diagnostics.Debug.WriteLine("invreq Message: " + ex.StripeError.Message);
        //                break;
        //            case "rate_limit_error":
        //                System.Diagnostics.Debug.WriteLine("  rl Code: " + ex.StripeError.Code);
        //                System.Diagnostics.Debug.WriteLine("rl Message: " + ex.StripeError.Message);
        //                break;
        //            case "validation_error":
        //                System.Diagnostics.Debug.WriteLine(" val  Code: " + ex.StripeError.Code);
        //                System.Diagnostics.Debug.WriteLine("val Message: " + ex.StripeError.Message);
        //                break;
        //            default:
        //                // Unknown Error Type
        //                break;
        //        }
        //    }
        //}

        public string MakeStripePayment()
        {
            creditCardNumber = FindViewById<EditText>(Resource.Id.txtCreditCardNumber);
            cardExpiryMonth = FindViewById<EditText>(Resource.Id.txtExpiryMonth);
            cardExpiryYear = FindViewById<EditText>(Resource.Id.txtExpiryYear);
            cardCVV = FindViewById<EditText>(Resource.Id.txtCVV);

            string cardNumber = creditCardNumber.Text;
            string cardExpMonth = cardExpiryMonth.Text;
            string cardExpYear = cardExpiryYear.Text;
            string cardCVC = cardCVV.Text;

            PaymentModel payment = new PaymentModel();
            Token
                stripeToken = new Token();
            try
            {
                StripeConfiguration.SetApiKey("sk_test_LMuAkBgF8zxl2ha3G66Yygdq00s09S6uzP");

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
            }
            catch (Exception exception)
            {
                StripeException
                    stripeException = exception.InnerException as StripeException;

                if (stripeException != null)
                {
                    Dictionary<String, Int32>
                        stripeErrorDictionary = new Dictionary<string, int>() {
                    // Error messages from stripe.com/docs/api#errors
                    { "invalid_number", 1 },
                    { "invalid_expiry_month", 2 },
                    { "invalid_expiry_year", 3 },
                    { "invalid_cvc", 4 },
                    { "invalid_swipe_data", 5 },
                    { "incorrect_number", 6 },
                    { "expired_card", 7 },
                    { "incorrect_cvc", 8 },
                    { "incorrect_zip", 9 },
                    { "card_declined", 10 },
                    { "missing", 11 },
                    { "processing_error", 12 },
                        };

                    String
                        errorMessage = "";

                    if (stripeErrorDictionary.ContainsKey(stripeException.StripeError.Code))
                    {
                        string errorNumber = stripeErrorDictionary[stripeException.StripeError.Code].ToString();
                        errorMessage = errorNumber;
                    }
                    else { errorMessage = "An unknown error occurred."; }
                    // Show error
                    return stripeToken.ToString();
                }
            }
            if (stripeToken == null)
            {
                // Show error.
                return null;
            }
            // Use 'stripeToken.Id' as the token to make your payments
            return stripeToken.Id;
        }
    }

    public class MakePayment
    {
        public string CreateToken(string cardNumber, string cardExpMonth, string cardExpYear, string cardCVC)
        {
            StripeConfiguration.SetApiKey("sk_test_LMuAkBgF8zxl2ha3G66Yygdq00s09S6uzP");

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
            //Token stripeToken = tokenService.Create(tokenOptions);
            var stripeToken = tokenService.Create(tokenOptions);
            return stripeToken.ToString(); // This is the token
            //return null;
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