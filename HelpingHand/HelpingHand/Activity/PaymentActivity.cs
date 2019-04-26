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
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using HelpingHand.Model;
using Newtonsoft.Json;
using Stripe;

namespace HelpingHand
{
    [Activity(Label = "Make Payment")]
    public class PaymentActivity : AppCompatActivity
    {
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;
        EditText creditCardNumber, cardExpiryMonth, cardExpiryYear, cardCVV;
        TextView appointmentCost, appointmentDate;
        Button AcceptPayment;
        string startTime, endTime, userEmail, babysitterEmail, Babysitter, City, Address, Eircode, _date;
        DateTime Date;
        decimal Cost;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.payment_form);
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            AcceptPayment = FindViewById<Button>(Resource.Id.btnAccept);
            creditCardNumber = FindViewById<EditText>(Resource.Id.txtCreditCardNumber);
            cardExpiryMonth = FindViewById<EditText>(Resource.Id.txtExpiryMonth);
            cardExpiryYear = FindViewById<EditText>(Resource.Id.txtExpiryYear);

            creditCardNumber.AddTextChangedListener(new CreditCardFormatter(creditCardNumber));
            cardExpiryMonth.SetFilters(new Android.Text.IInputFilter[]{ new MinMaxInputFilter(1, 12) });
            cardExpiryYear.SetFilters(new Android.Text.IInputFilter[] { new MinMaxInputFilter(1, 29) });

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            //SupportActionBar.Title = "Cancel";

            appointmentCost = FindViewById<TextView>(Resource.Id.txtviewCost);
            appointmentDate = FindViewById<TextView>(Resource.Id.txtviewDate);

            string appointment = this.Intent.GetStringExtra("KEY");
            Appointment newAppointment = JsonConvert.DeserializeObject<Appointment>(appointment);
            Date = newAppointment.Date;
            _date = Date.ToString();
            string[] dateNotTime = _date.Split(' ');
            string justDate = dateNotTime[0];

            startTime = newAppointment.startTime;
            endTime = newAppointment.endTime;
            userEmail = auth.CurrentUser.Email;
            babysitterEmail = newAppointment.babysitterEmail;
            Babysitter = newAppointment.Babysitter;
            City = newAppointment.City;
            Address = newAppointment.Address;
            Eircode = newAppointment.Eircode;
            Cost = newAppointment.cost;
            decimal AppFee = Cost * 10 / 100;
            decimal appointmentCharge = AppFee + Cost;

            appointmentCost.Text = "Appointment Cost + 10% fee: " + appointmentCharge.ToString();
            appointmentDate.Text = "Appointment Date: " + justDate.ToString();

            //ChargeCard();
            AcceptPayment.Click += (object sender, EventArgs args) =>
            {
                FragmentTransaction transcation = FragmentManager.BeginTransaction();
                Dialogclass payment = new Dialogclass();
                payment.Show(transcation, "Dialog Fragment");
                payment.onPaymentComplete += Signup_onPaymentComplete;
            };
        }

        private void Signup_onPaymentComplete(object sender, PaymentConfirmed e)
        {
            MakeStripePayment();
            ChargeCard();
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

        public async void ChargeCard()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.SetApiKey("sk_test_LMuAkBgF8zxl2ha3G66Yygdq00s09S6uzP");

            PaymentModel payment = new PaymentModel();
            var token = payment.Token;

            decimal AppFee = Cost * 10 / 100;
            decimal appointmentCharge = AppFee + Cost;
            long amountCharged = Convert.ToInt32(appointmentCharge);
            string convertCost = amountCharged.ToString();
            decimal finalCost = decimal.Parse(convertCost);

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(amountCharged * 100), //for cents
                Currency = "eur",
                SourceId = "tok_visa", // token
                Description = "Babysitter Hired",
                ReceiptEmail = auth.CurrentUser.Email,
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            Appointment appointment = new Appointment();
            appointment.Date = Date;
            appointment.startTime = startTime;
            appointment.endTime = endTime;
            appointment.userEmail = auth.CurrentUser.Email;
            appointment.babysitterEmail = babysitterEmail;
            appointment.Babysitter = Babysitter;
            appointment.City = City;
            appointment.Address = Address;
            appointment.Eircode = Eircode;
            appointment.cost = finalCost;

            var item = await firebase.Child("appointment").PostAsync<Appointment>(appointment);
            Toast.MakeText(this, "Payment Made", ToastLength.Short).Show();

            var appointmentJson = JsonConvert.SerializeObject(appointment);

            var viewReciept = new Intent(this, typeof(AppointmentRecieptActivity));
            viewReciept.PutExtra("KEY", appointmentJson);
            StartActivity(viewReciept);
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