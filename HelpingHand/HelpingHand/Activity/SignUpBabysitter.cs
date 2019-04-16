using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Storage;
using Firebase.Xamarin.Database;
using HelpingHand.Model;
using XamarinFirebaseAuth;
using static Android.Views.View;

namespace HelpingHand
{
    [Activity(Label = "SignUp", Theme = "@style/AppTheme")]
    public class SignUpBabysitter : Activity, IOnClickListener, IOnCompleteListener, IOnProgressListener, IOnSuccessListener, IOnFailureListener
    {
        FirebaseStorage storage;
        StorageReference storageRef;
        Button btnMonMorn, btnTueMorn, btnWedMorn, btnThuMorn, btnFriMorn, btnSatMorn, btnSunMorn,
            btnMonAft, btnTueAft, btnWedAft, btnThuAft, btnFriAft, btnSatAft, btnSunAft,
            btnMonEve, btnTueEve, btnWedEve, btnThuEve, btnFriEve, btnSatEve, btnSunEve,
            btnMonNigh, btnTueNigh, btnWedNigh, btnThuNigh, btnFriNigh, btnSatNigh, btnSunNigh;
        int count = 0;
        private const int PICK_IMAGE_REQUSET = 71;
        FloatingActionButton btnSignup;
        TextView btnLogin;
        private Button btnUpload, btnChoose;
        private ImageView imgView;

        private Android.Net.Uri filePath;
        EditText input_name, input_email, input_password, input_age,
            input_phone, input_address, input_city, input_eircode, input_rate;
        RelativeLayout Babysitter_reg;

        ProgressBar progressBar;
        private List<BabySitter> list_babysitters = new List<BabySitter>();
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;
        string[] values = new string[28];

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.BabysitterReg);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            storage = FirebaseStorage.Instance;
            storageRef = storage.GetReferenceFromUrl("gs://th-year-project-37928.appspot.com");
            StorageReference userImage = storageRef.Child("user/vetted document/");

            //Views
            btnSignup = FindViewById<FloatingActionButton>(Resource.Id.signup_btn_Babysitter);
            btnLogin = FindViewById<TextView>(Resource.Id.signup_btn_login);

            //this.FindViewById<Button>(Resource.Id.btnMonAfter).Click += this.Button_Click;
            btnMonMorn = FindViewById<Button>(Resource.Id.btnMonMorn); btnTueMorn = FindViewById<Button>(Resource.Id.btnTueMorn); btnWedMorn = FindViewById<Button>(Resource.Id.btnWedMorn);
            btnThuMorn = FindViewById<Button>(Resource.Id.btnThuMorn); btnFriMorn = FindViewById<Button>(Resource.Id.btnFriMorn); btnSatMorn = FindViewById<Button>(Resource.Id.btnSatMorn);
            btnSunMorn = FindViewById<Button>(Resource.Id.btnSunMorn);
            btnMonAft = FindViewById<Button>(Resource.Id.btnMonAfter); btnTueAft = FindViewById<Button>(Resource.Id.btnTueAfter); btnWedAft = FindViewById<Button>(Resource.Id.btnWedAfter);
            btnThuAft = FindViewById<Button>(Resource.Id.btnThuAfter); btnFriAft = FindViewById<Button>(Resource.Id.btnFriAfter); btnSatAft = FindViewById<Button>(Resource.Id.btnSatAfter);
            btnSunAft = FindViewById<Button>(Resource.Id.btnSunAfter);
            btnMonEve = FindViewById<Button>(Resource.Id.btnMonEve); btnTueEve = FindViewById<Button>(Resource.Id.btnTueEve); btnWedEve = FindViewById<Button>(Resource.Id.btnWedEve);
            btnThuEve = FindViewById<Button>(Resource.Id.btnThuEve); btnFriEve = FindViewById<Button>(Resource.Id.btnFriEve); btnSatEve = FindViewById<Button>(Resource.Id.btnSatEve);
            btnSunEve = FindViewById<Button>(Resource.Id.btnSunEve);
            btnMonNigh = FindViewById<Button>(Resource.Id.btnMonNight); btnTueNigh = FindViewById<Button>(Resource.Id.btnTueNight); btnWedNigh = FindViewById<Button>(Resource.Id.btnWedNight);
            btnThuNigh = FindViewById<Button>(Resource.Id.btnThuNight); btnFriNigh = FindViewById<Button>(Resource.Id.btnFriNight); btnSatNigh = FindViewById<Button>(Resource.Id.btnSatNight);
            btnSunNigh = FindViewById<Button>(Resource.Id.btnSunNight);

            input_name = FindViewById<EditText>(Resource.Id.signup_name);
            input_email = FindViewById<EditText>(Resource.Id.signup_email);
            input_password = FindViewById<EditText>(Resource.Id.signup_password);
            input_phone = FindViewById<EditText>(Resource.Id.signup_phone);
            input_age = FindViewById<EditText>(Resource.Id.signup_age);
            input_eircode = FindViewById<EditText>(Resource.Id.signup_eircode);
            input_address = FindViewById<EditText>(Resource.Id.signup_address);
            input_city = FindViewById<EditText>(Resource.Id.signup_city);
            input_rate = FindViewById<EditText>(Resource.Id.signup_rate);

            Babysitter_reg = FindViewById<RelativeLayout>(Resource.Id.activity_Babysitter_reg);
            btnChoose = FindViewById<Button>(Resource.Id.btnChoose);
            btnUpload = FindViewById<Button>(Resource.Id.btnUpload);
            imgView = FindViewById<ImageView>(Resource.Id.imgView);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar);
            progressBar.Visibility = ViewStates.Invisible;

            //Events  
            btnChoose.Click += delegate
            {
                ChooseFile();
            };
            btnUpload.Click += delegate
            {
                UploadFile();
            };

            btnLogin.SetOnClickListener(this);
            btnSignup.SetOnClickListener(this);
            //btnForgetPass.SetOnClickListener(this);

            btnMonMorn.Click += Button_Click; btnTueMorn.Click += Button_Click; btnWedMorn.Click += Button_Click; btnThuMorn.Click += Button_Click; btnFriMorn.Click += Button_Click;
            btnSatMorn.Click += Button_Click; btnSunMorn.Click += Button_Click;
            btnMonAft.Click += Button_Click; btnTueAft.Click += Button_Click; btnWedAft.Click += Button_Click; btnThuAft.Click += Button_Click; btnFriAft.Click += Button_Click;
            btnSatAft.Click += Button_Click; btnSunAft.Click += Button_Click;
            btnMonEve.Click += Button_Click; btnTueEve.Click += Button_Click; btnWedEve.Click += Button_Click; btnThuEve.Click += Button_Click; btnFriEve.Click += Button_Click;
            btnSatEve.Click += Button_Click; btnSunEve.Click += Button_Click;
            btnMonNigh.Click += Button_Click; btnTueNigh.Click += Button_Click; btnWedNigh.Click += Button_Click; btnThuNigh.Click += Button_Click; btnFriNigh.Click += Button_Click;
            btnSatNigh.Click += Button_Click; btnSunNigh.Click += Button_Click;
        }
        void Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Id)
            {
                case Resource.Id.btnMonMorn:
                    //button mon Morn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Monday Morning selected", ToastLength.Short).Show();
                        btnMonMorn.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        //string day = "monday";
                        values[0] = "Monday Morning";
                    }
                    else if (count >= 1)
                    {
                        btnMonMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnTueMorn:
                    //button tueMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Tuesday Morning selected", ToastLength.Short).Show();
                        btnTueMorn.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[1] = "Tuesday Morning";
                    }
                    else if (count >= 1)
                    {
                        btnTueMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnWedMorn:
                    //button wedMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Wednesday Morning selected", ToastLength.Short).Show();
                        btnWedMorn.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[2] = "Wednesday Morning";
                    }
                    else if (count >= 1)
                    {
                        btnWedMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnThuMorn:
                    //button thuMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Thursday Morning selected", ToastLength.Short).Show();
                        btnThuMorn.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[3] = "Thursday Morning";
                    }
                    else if (count >= 1)
                    {
                        btnThuMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnFriMorn:
                    //button friMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Friday Morning selected", ToastLength.Short).Show();
                        btnFriMorn.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[4] = "Friday Morning";
                    }
                    else if (count >= 1)
                    {
                        btnFriMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSatMorn:
                    //button satMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Saturday Morning selected", ToastLength.Short).Show();
                        btnSatMorn.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[5] = "Saturday Morning";
                    }
                    else if (count >= 1)
                    {
                        btnSatMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSunMorn:
                    //button satMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Sunday Morning selected", ToastLength.Short).Show();
                        btnSunMorn.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[6] = "Sunday Morning";
                    }
                    else if (count >= 1)
                    {
                        btnSunMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnMonAfter:
                    //button monAfter was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Monday Afternoon selected", ToastLength.Short).Show();
                        btnMonAft.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[7] = "Monday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        btnMonAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnTueAfter:
                    //button tueAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Tuesday Afternoon selected", ToastLength.Short).Show();
                        btnTueAft.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[8] = "Tuesday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        btnTueAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnWedAfter:
                    //button wedAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Wednesday Afternoon selected", ToastLength.Short).Show();
                        btnWedAft.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[9] = "Wednesday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        btnWedAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnThuAfter:
                    //button thuAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Thursday Afternoon selected", ToastLength.Short).Show();
                        btnThuAft.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[10] = "Thursday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        btnThuAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnFriAfter:
                    //button friAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Friday Afternoon selected", ToastLength.Short).Show();
                        btnFriAft.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[11] = "Friday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        btnFriAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSatAfter:
                    //button satAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Saturday Afternoon selected", ToastLength.Short).Show();
                        btnSatAft.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[12] = "Saturday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        btnSatAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSunAfter:
                    //button sunAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Sunday Afternoon selected", ToastLength.Short).Show();
                        btnSunAft.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[13] = "Sunday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        btnSunAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnMonEve:
                    //button monEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Monday Evening selected", ToastLength.Short).Show();
                        btnMonEve.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[14] = "Monday Evening";
                    }
                    else if (count >= 1)
                    {
                        btnMonEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnTueEve:
                    //button tueEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Tuesday Evening selected", ToastLength.Short).Show();
                        btnTueEve.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[15] = "Tuesday Evening";
                    }
                    else if (count >= 1)
                    {
                        btnTueEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnWedEve:
                    //button wedEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Wednesday Evening selected", ToastLength.Short).Show();
                        btnWedEve.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[16] = "Wednesday Evening";
                    }
                    else if (count >= 1)
                    {
                        btnWedEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnThuEve:
                    //button thuEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Thursday Evening selected", ToastLength.Short).Show();
                        btnThuEve.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[17] = "Thursday Evening";
                    }
                    else if (count >= 1)
                    {
                        btnThuEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnFriEve:
                    //button friEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Friday Evening selected", ToastLength.Short).Show();
                        btnFriEve.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[18] = "Friday Evening";
                    }
                    else if (count >= 1)
                    {
                        btnFriEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSatEve:
                    //button satEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Saturday Evening selected", ToastLength.Short).Show();
                        btnSatEve.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[19] = "Saturday Evening";
                    }
                    else if (count >= 1)
                    {
                        btnSatEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSunEve:
                    //button sunEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Sunday Evening selected", ToastLength.Short).Show();
                        btnSunEve.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[20] = "Sunday Evening";
                    }
                    else if (count >= 1)
                    {
                        btnSunEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnMonNight:
                    //button monNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Monday Night selected", ToastLength.Short).Show();
                        btnMonNigh.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[21] = "Monday Night";
                    }
                    else if (count >= 1)
                    {
                        btnMonNigh.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnTueNight:
                    //button tueNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Tuesday Night selected", ToastLength.Short).Show();
                        btnTueNigh.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[22] = "Tuesday Night";
                    }
                    else if (count >= 1)
                    {
                        btnTueNigh.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnWedNight:
                    //button wedNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Wednesday Night selected", ToastLength.Short).Show();
                        btnWedNigh.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[23] = "Wednesday Night";
                    }
                    else if (count >= 1)
                    {
                        btnWedNigh.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnThuNight:
                    //button thuNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Thursday Night selected", ToastLength.Short).Show();
                        btnThuNigh.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[24] = "Thursday Night";
                    }
                    else if (count >= 1)
                    {
                        btnThuNigh.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnFriNight:
                    //button friNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Friday Night selected", ToastLength.Short).Show();
                        btnFriNigh.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[25] = "Friday Night";
                    }
                    else if (count >= 1)
                    {
                        btnFriNigh.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSatNight:
                    //button satNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Saturday Night selected", ToastLength.Short).Show();
                        btnSatNigh.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[26] = "Saturday Night";
                    }
                    else if (count >= 1)
                    {
                        btnSatNigh.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSunNight:
                    //button sunNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Sunday Night selected", ToastLength.Short).Show();
                        btnSunNigh.SetBackgroundColor(Android.Graphics.Color.Beige);
                        count++;
                        values[27] = "Sunday Night";
                    }
                    else if (count >= 1)
                    {
                        btnSunNigh.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
            }
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
                SignUpUser(input_email.Text, input_password.Text);
            }
        }
        private void UploadFile()
        {
            if (filePath != null)

                progressBar.Visibility = ViewStates.Visible;
            var images = storageRef.Child("Garda Vetted Document/" + Guid.NewGuid().ToString());
            images.PutFile(filePath)
                .AddOnProgressListener(this)
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);
        }
        private void ChooseFile()
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PICK_IMAGE_REQUSET);
        }

        private void SignUpUser(string email, string password)
        {
            auth.CreateUserWithEmailAndPassword(email, password).AddOnCompleteListener(this, this);
            CreateUser();
        }

        private async void CreateUser()
        {
            string userName = input_name.Text.ToString();
            string userPassword = input_password.Text.ToString();
            string userPhone = input_phone.Text.ToString();
            string userAddress = input_address.Text.ToString();
            string userCity = input_city.Text.ToString();
            string userEmail = input_email.Text.ToString();
            string userRate = input_rate.Text.ToString();

            if ( userName == "" || userPassword == "" || userPhone == "" ||  userAddress == ""
                || userCity == "" || userEmail == "" || userRate == "")
            {
                Toast.MakeText(this, "Please enter all fields !", ToastLength.Short).Show();
                btnSignup.Enabled = false;
                return;
            }
            else
            {
                var id = auth.CurrentUser.Uid;
                string[] _days = values;
                StringBuilder strTime = new StringBuilder();
                foreach (var i in _days)
                {
                    string s = i;
                    if (s != null)
                    {

                        string availableDays = s + ", ";
                        strTime.Append(availableDays);
                    }
                }
                string availableTime = strTime.ToString();
                decimal payRate = decimal.Parse(input_rate.Text);

                CheckBox vetted = FindViewById<CheckBox>(Resource.Id.signup_vetted_yes);

                BabySitter babysitter = new BabySitter();
                babysitter.id = id;
                string babysitterName = input_name.Text;
                string[] splitName = babysitterName.Split(' ');
                string name = splitName[0];

                babysitter.name = name;
                babysitter.email = input_email.Text;
                babysitter.age = Convert.ToInt32(input_age.Text);
                babysitter.rate = payRate;
                babysitter.phone = input_phone.Text;
                babysitter.address = input_address.Text;
                babysitter.city = input_city.Text;
                babysitter.eircode = input_eircode.Text;
                babysitter.gardaVetted = vetted.Checked;
                babysitter.availability = availableTime;

                var firebase = new FirebaseClient(FirebaseURL);
                //Add Item
                var item = await firebase.Child("babysitter").PostAsync<BabySitter>(babysitter);
            }
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

        public void OnProgress(Java.Lang.Object snapshot)
        {
            var taskSnapShot = (UploadTask.TaskSnapshot)snapshot;
            double progress = (100.0 * taskSnapShot.BytesTransferred / taskSnapShot.TotalByteCount);
        }
        public void OnSuccess(Java.Lang.Object result)
        {
            progressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(this, "Upload Successful", ToastLength.Short).Show();
        }
        public void OnFailure(Java.Lang.Exception e)
        {
            progressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(this, "" + e.Message, ToastLength.Short).Show();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PICK_IMAGE_REQUSET &&
                resultCode == Result.Ok &&
                data != null &&
                data.Data != null)
            {
                filePath = data.Data;
                try
                {
                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, filePath);
                    imgView.SetImageBitmap(bitmap);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}