using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using HelpingHand.Model;
using XamarinFirebaseAuth;
using static Android.Views.View;

namespace HelpingHand
{
    [Activity(Label = "SignUp", Theme = "@style/AppTheme")]
    public class SignUpBabysitter : Activity, IOnClickListener, IOnCompleteListener
    {
        FloatingActionButton btnSignup;
        TextView btnLogin;
        EditText input_name, input_email, input_password, input_age,
            input_phone, input_address, input_city, input_eircode;
        RelativeLayout Babysitter_reg;

        private List<BabySitter> list_babysitters = new List<BabySitter>();
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.BabysitterReg);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //Views
            btnSignup = FindViewById<FloatingActionButton>(Resource.Id.signup_btn_Babysitter);
            btnLogin = FindViewById<TextView>(Resource.Id.signup_btn_login);

            input_name = FindViewById<EditText>(Resource.Id.signup_name);
            input_email = FindViewById<EditText>(Resource.Id.signup_email);
            input_password = FindViewById<EditText>(Resource.Id.signup_password);
            input_phone = FindViewById<EditText>(Resource.Id.signup_phone);
            input_age = FindViewById<EditText>(Resource.Id.signup_age);
            input_eircode = FindViewById<EditText>(Resource.Id.signup_eircode);
            input_address = FindViewById<EditText>(Resource.Id.signup_address);
            input_city = FindViewById<EditText>(Resource.Id.signup_city);

            Babysitter_reg = FindViewById<RelativeLayout>(Resource.Id.activity_Babysitter_reg);

            btnLogin.SetOnClickListener(this);
            btnSignup.SetOnClickListener(this);
            //btnForgetPass.SetOnClickListener(this);

        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.signup_btn_login)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
            //else
            //if (v.Id == Resource.Id.signup_btn_forget_password)
            //{
            //    StartActivity(new Intent(this, typeof(ForgetPassword)));
            //    Finish();
            //}
            else
            if (v.Id == Resource.Id.signup_btn_Babysitter)
            {
                CreateUser();
                SignUpUser(input_email.Text, input_password.Text);
            }
        }

        private void SignUpUser(string email, string password)
        {
            auth.CreateUserWithEmailAndPassword(email, password).AddOnCompleteListener(this, this);
        }

        private async void CreateUser()
        {
            BabySitter babysitter = new BabySitter();
            babysitter.id = string.Empty;
            babysitter.name = input_name.Text;
            babysitter.email = input_email.Text;
            babysitter.age = Convert.ToInt32(input_age.Text);
            babysitter.phone = input_phone.Text;
            babysitter.address = input_address.Text;
            babysitter.city = input_city.Text;
            babysitter.eircode = input_eircode.Text;

            var firebase = new FirebaseClient(FirebaseURL);
            //Add Item
            var item = await firebase.Child("babysitter").PostAsync<BabySitter>(babysitter);
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful == true)
            {
                Toast.MakeText(this, "Sign up Successful !", ToastLength.Short).Show();
                StartActivity(new Intent(this, typeof(MainActivity)));
            }
            else
            {
                Toast.MakeText(this, "Register Failed", ToastLength.Short).Show();
            }
        }
    }
}