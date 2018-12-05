using Android.App;
using Android.Widget;
using Android.OS;
using Firebase;
using Firebase.Auth;
using System;
using static Android.Views.View;
using Android.Views;
using Android.Gms.Tasks;
using Android.Support.Design.Widget;
using XamarinFirebaseAuth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace HelpingHand
{
    [Activity(Label = "XamarinFirebaseAuth", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : Activity, IOnClickListener, IOnCompleteListener
    {
        FloatingActionButton btnLogin;
        EditText input_email, input_password;
        TextView btnSignUp, btnForgetPassword, btnBabysitterReg;
        private RelativeLayout activity_main;
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        public static FirebaseApp app;
        FirebaseAuth auth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Init Auth
            InitFirebaseAuth();

            //Views
            btnLogin = FindViewById<FloatingActionButton>(Resource.Id.login_btn_login);
            input_email = FindViewById<EditText>(Resource.Id.login_email);
            input_password = FindViewById<EditText>(Resource.Id.login_password);
            btnSignUp = FindViewById<TextView>(Resource.Id.login_btn_sign_up);
            btnBabysitterReg = FindViewById<TextView>(Resource.Id.login_btn_Babysitter_reg);
            btnForgetPassword = FindViewById<TextView>(Resource.Id.login_btn_forget_password);

            btnBabysitterReg.SetOnClickListener(this);
            btnSignUp.SetOnClickListener(this);
            btnLogin.SetOnClickListener(this);
            btnForgetPassword.SetOnClickListener(this);
        }

        private void InitFirebaseAuth()
        {
            var options = new FirebaseOptions.Builder()
               .SetApplicationId("1:815188105158:android:5ecf53fa3f4f75fc")
               .SetApiKey("AIzaSyBbvAUY7yqYkc1yKZilamfU1TzywnKKmhM")
               .Build();

        //    FirebaseApp myApp = FirebaseApp.InitializeApp(this,options,
        //"MyAppName");

            if (app == null)
                app = FirebaseApp.InitializeApp(this, options,"MyAppName");
            auth = FirebaseAuth.GetInstance(app);
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.login_btn_forget_password)
            {
                StartActivity(new Android.Content.Intent(this, typeof(ForgetPassword)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.login_btn_sign_up)
            {
                StartActivity(new Android.Content.Intent(this, typeof(SignUp)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.login_btn_Babysitter_reg)
            {
                StartActivity(new Android.Content.Intent(this, typeof(SignUpBabysitter)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.login_btn_login)
            {
                if (input_email.Text == "" || input_password.Text == null)
                {
                    Toast.MakeText(this, "Login Failed", ToastLength.Short).Show();
                }
                else
                {
                    LoginUser(input_email.Text, input_password.Text);
                }
                
            }
        }

        private void LoginUser(string email, string password)
        {
            auth.SignInWithEmailAndPassword(email, password).AddOnCompleteListener(this);
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                Finish();
            }
            else
            {
                Toast.MakeText(this, "Login Failed", ToastLength.Short).Show();
            }
        }
    }
}