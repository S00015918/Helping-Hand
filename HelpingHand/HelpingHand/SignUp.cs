using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using HelpingHand;
using HelpingHand.Model;
using System;
using System.Collections.Generic;
using static Android.Views.View;

namespace XamarinFirebaseAuth
{
    [Activity(Label = "SignUp", Theme ="@style/AppTheme")]
    public class SignUp : Activity, IOnClickListener, IOnCompleteListener
    {
        Button btnSignup;
        TextView btnLogin, btnForgetPass;
        EditText input_name, input_email, input_password, input_phone, input_address, input_eircode;
        RelativeLayout activity_sign_up;

        private List<Parent> list_parents = new List<Parent>();

        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;

        public void OnClick(View v)
        {
            if(v.Id == Resource.Id.signup_btn_login)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.signup_btn_forget_password)
            {
                StartActivity(new Intent(this, typeof(ForgetPassword)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.signup_btn_register)
            {
                CreateUser();
                SignUpUser(input_email.Text, input_password.Text); 
            }
                
        }

        private void SignUpUser(string email, string password)
        {
            auth.CreateUserWithEmailAndPassword(email, password).AddOnCompleteListener(this, this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SignUp);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //Views
            btnSignup = FindViewById<Button>(Resource.Id.signup_btn_register);
            btnLogin = FindViewById<TextView>(Resource.Id.signup_btn_login);
            btnForgetPass = FindViewById<TextView>(Resource.Id.signup_btn_forget_password);
            input_name = FindViewById<EditText>(Resource.Id.signup_name);
            input_email = FindViewById<EditText>(Resource.Id.signup_email);
            input_password = FindViewById<EditText>(Resource.Id.signup_password);
            input_phone = FindViewById<EditText>(Resource.Id.signup_phone);
            input_eircode = FindViewById<EditText>(Resource.Id.signup_eircode);
            input_address = FindViewById<EditText>(Resource.Id.signup_address);
            activity_sign_up = FindViewById<RelativeLayout>(Resource.Id.activity_sign_up);

            btnLogin.SetOnClickListener(this);
            btnSignup.SetOnClickListener(this);
            btnForgetPass.SetOnClickListener(this);

        }

        private async void CreateUser()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            var Uid = firebase.Child("parent").Child(auth.CurrentUser.Uid).Equals(Parent);

            Parent parent = new Parent();
            parent.id = Convert.ToString(Uid);
            parent.name = input_name.Text;
            //parent.surname = input_surname.Text;
            parent.email = input_email.Text;
            parent.phone = input_phone.Text;
            parent.eircode = input_eircode.Text;
            parent.address = input_address.Text;
            //parent.noOfKids = Convert.ToInt32(input_childCount);

            //Add Item
            var item = await firebase.Child("parent").PostAsync<Parent>(parent);

        }

        public void OnComplete(Task task)
        {
            if(task.IsSuccessful == true)
            {
                Snackbar snackbar = Snackbar.Make(activity_sign_up, "Register Successfully ", Snackbar.LengthShort);
                snackbar.Show();
                StartActivity(new Intent(this, typeof(MainActivity)));
            }
            else
            {
                Snackbar snackbar = Snackbar.Make(activity_sign_up, "Register Failed ", Snackbar.LengthShort);
                snackbar.Show();
            }
        }
    }
}