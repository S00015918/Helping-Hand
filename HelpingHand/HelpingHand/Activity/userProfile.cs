using System;
using System.Collections.Generic;
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
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Storage;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using HelpingHand.Adapter;
using HelpingHand.Model;
using XamarinFirebaseAuth;

namespace HelpingHand
{
    [Activity(Label = "Profile", Theme = "@style/AppTheme")]
    public class userProfile : AppCompatActivity, IOnProgressListener, IOnSuccessListener, IOnFailureListener
    {
        EditText input_new_email, input_new_name, input_new_rate,
            input_new_address, input_new_phone, input_new_city, input_new_eircode, input_new_age;
        ImageView input_image, upload_image;
        LinearLayout payRateLayout;
        GridLayout gridAvailability;
        Button MonMorn, TueMorn, WedMorn, ThuMorn, FriMorn, SatMorn, SunMorn,
            MonAft, TueAft, WedAft, ThuAft, FriAft, SatAft, SunAft,
            MonEve, TueEve, WedEve, ThuEve, FriEve, SatEve, SunEve,
            MonNight, TueNight, WedNight, ThuNight, FriNight, SatNight, SunNight;
        Button showAvailability;

        string[] values = new string[28];
        int count = 0;
        bool showAvailabiltyGrid = false;

        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        private const int PICK_IMAGE_REQUSET = 71;
        ProgressBar progressBar;
        FirebaseStorage storage;
        StorageReference storageRef;
        private List<Parent> list_parents = new List<Parent>();
        private List<BabySitter> list_babysitters = new List<BabySitter>();

        private Android.Net.Uri filePath;
        private ParentViewAdapter adapter;
        private BabysitterViewAdapter sitteradapter;

        RelativeLayout activity_userProfile, activity_Rlayout;
        FirebaseAuth auth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profilePage);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            storage = FirebaseStorage.Instance;
            storageRef = storage.GetReferenceFromUrl("gs://th-year-project-37928.appspot.com");
            StorageReference userImage = storageRef.Child("user/profile pic/");

            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            //SupportActionBar.Title = auth.CurrentUser.Email;

            //View
            activity_userProfile = FindViewById<RelativeLayout>(Resource.Id.activity_userProfile);
            activity_Rlayout = FindViewById<RelativeLayout>(Resource.Id.Rlayout);
            payRateLayout = FindViewById<LinearLayout>(Resource.Id.layoutRate);

            MonMorn = FindViewById<Button>(Resource.Id.MonMorn); TueMorn = FindViewById<Button>(Resource.Id.TueMorn); WedMorn = FindViewById<Button>(Resource.Id.WedMorn);
            ThuMorn = FindViewById<Button>(Resource.Id.ThuMorn); FriMorn = FindViewById<Button>(Resource.Id.FriMorn); SatMorn = FindViewById<Button>(Resource.Id.SatMorn);
            SunMorn = FindViewById<Button>(Resource.Id.SunMorn);
            MonAft = FindViewById<Button>(Resource.Id.MonAfter); TueAft = FindViewById<Button>(Resource.Id.TueAfter); WedAft = FindViewById<Button>(Resource.Id.WedAfter);
            ThuAft = FindViewById<Button>(Resource.Id.ThuAfter); FriAft = FindViewById<Button>(Resource.Id.FriAfter); SatAft = FindViewById<Button>(Resource.Id.SatAfter);
            SunAft = FindViewById<Button>(Resource.Id.SunAfter);
            MonEve = FindViewById<Button>(Resource.Id.MonEve); TueEve = FindViewById<Button>(Resource.Id.TueEve); WedEve = FindViewById<Button>(Resource.Id.WedEve);
            ThuEve = FindViewById<Button>(Resource.Id.ThuEve); FriEve = FindViewById<Button>(Resource.Id.FriEve); SatEve = FindViewById<Button>(Resource.Id.SatEve);
            SunEve = FindViewById<Button>(Resource.Id.SunEve);
            MonNight = FindViewById<Button>(Resource.Id.MonNight); TueNight = FindViewById<Button>(Resource.Id.TueNight); WedNight = FindViewById<Button>(Resource.Id.WedNight);
            ThuNight = FindViewById<Button>(Resource.Id.ThuNight); FriNight = FindViewById<Button>(Resource.Id.FriNight); SatNight = FindViewById<Button>(Resource.Id.SatNight);
            SunNight = FindViewById<Button>(Resource.Id.SunNight);

            input_image = FindViewById<ImageView>(Resource.Id.imgUser);
            upload_image = FindViewById<ImageView>(Resource.Id.uploadImage);
            input_new_name = FindViewById<EditText>(Resource.Id.name);
            input_new_email = FindViewById<EditText>(Resource.Id.email);
            //input_new_password = FindViewById<EditText>(Resource.Id.signup_password);
            input_new_phone = FindViewById<EditText>(Resource.Id.phone);
            input_new_address = FindViewById<EditText>(Resource.Id.address);
            input_new_age = FindViewById<EditText>(Resource.Id.age);
            input_new_rate = FindViewById<EditText>(Resource.Id.payRate);
            input_new_city = FindViewById<EditText>(Resource.Id.city);
            input_new_eircode = FindViewById<EditText>(Resource.Id.eircode);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar);
            gridAvailability = FindViewById<GridLayout>(Resource.Id.gridAvailability);
            showAvailability = FindViewById<Button>(Resource.Id.show_Availability);

            input_new_age.Visibility = ViewStates.Invisible;
            payRateLayout.Visibility = ViewStates.Invisible;
            progressBar.Visibility = ViewStates.Invisible;
            gridAvailability.Visibility = ViewStates.Invisible;

            MonMorn.Click += Button_Click; TueMorn.Click += Button_Click; WedMorn.Click += Button_Click; ThuMorn.Click += Button_Click; FriMorn.Click += Button_Click;
            SatMorn.Click += Button_Click; SunMorn.Click += Button_Click;
            MonAft.Click += Button_Click; TueAft.Click += Button_Click; WedAft.Click += Button_Click; ThuAft.Click += Button_Click; FriAft.Click += Button_Click;
            SatAft.Click += Button_Click; SunAft.Click += Button_Click;
            MonEve.Click += Button_Click; TueEve.Click += Button_Click; WedEve.Click += Button_Click; ThuEve.Click += Button_Click; FriEve.Click += Button_Click;
            SatEve.Click += Button_Click; SunEve.Click += Button_Click;
            MonNight.Click += Button_Click; TueNight.Click += Button_Click; WedNight.Click += Button_Click; ThuNight.Click += Button_Click; FriNight.Click += Button_Click;
            SatNight.Click += Button_Click; SunNight.Click += Button_Click;

            showAvailability.Click += ShowAvailability_Click;

            activity_Rlayout.Click += delegate
            {
                ChooseImage();
            };
            upload_image.Click += delegate
            {
                UploadImage();
            };

            LoadData();
        }

        private void ShowAvailability_Click(object sender, EventArgs e)
        {
            showAvailabiltyGrid = true;
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
                        MonMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        //string day = "monday";
                        values[0] = "Monday Morning";
                    }
                    else if (count >= 1)
                    {
                        MonMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnTueMorn:
                    //button tueMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Tuesday Morning selected", ToastLength.Short).Show();
                        TueMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[1] = "Tuesday Morning";
                    }
                    else if (count >= 1)
                    {
                        TueMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnWedMorn:
                    //button wedMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Wednesday Morning selected", ToastLength.Short).Show();
                        WedMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[2] = "Wednesday Morning";
                    }
                    else if (count >= 1)
                    {
                        WedMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnThuMorn:
                    //button thuMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Thursday Morning selected", ToastLength.Short).Show();
                        ThuMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[3] = "Thursday Morning";
                    }
                    else if (count >= 1)
                    {
                        ThuMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnFriMorn:
                    //button friMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Friday Morning selected", ToastLength.Short).Show();
                        FriMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[4] = "Friday Morning";
                    }
                    else if (count >= 1)
                    {
                        FriMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSatMorn:
                    //button satMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Saturday Morning selected", ToastLength.Short).Show();
                        SatMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[5] = "Saturday Morning";
                    }
                    else if (count >= 1)
                    {
                        SatMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSunMorn:
                    //button satMorn was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Sunday Morning selected", ToastLength.Short).Show();
                        SunMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[6] = "Sunday Morning";
                    }
                    else if (count >= 1)
                    {
                        SunMorn.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnMonAfter:
                    //button monAfter was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Monday Afternoon selected", ToastLength.Short).Show();
                        MonAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[7] = "Monday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        MonAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnTueAfter:
                    //button tueAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Tuesday Afternoon selected", ToastLength.Short).Show();
                        TueAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[8] = "Tuesday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        TueAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnWedAfter:
                    //button wedAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Wednesday Afternoon selected", ToastLength.Short).Show();
                        WedAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[9] = "Wednesday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        WedAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnThuAfter:
                    //button thuAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Thursday Afternoon selected", ToastLength.Short).Show();
                        ThuAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[10] = "Thursday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        ThuAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnFriAfter:
                    //button friAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Friday Afternoon selected", ToastLength.Short).Show();
                        FriAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[11] = "Friday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        FriAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSatAfter:
                    //button satAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Saturday Afternoon selected", ToastLength.Short).Show();
                        SatAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[12] = "Saturday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        SatAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSunAfter:
                    //button sunAft was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Sunday Afternoon selected", ToastLength.Short).Show();
                        SunAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[13] = "Sunday Afternoon";
                    }
                    else if (count >= 1)
                    {
                        SunAft.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnMonEve:
                    //button monEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Monday Evening selected", ToastLength.Short).Show();
                        MonEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[14] = "Monday Evening";
                    }
                    else if (count >= 1)
                    {
                        MonEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnTueEve:
                    //button tueEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Tuesday Evening selected", ToastLength.Short).Show();
                        TueEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[15] = "Tuesday Evening";
                    }
                    else if (count >= 1)
                    {
                        TueEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnWedEve:
                    //button wedEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Wednesday Evening selected", ToastLength.Short).Show();
                        WedEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[16] = "Wednesday Evening";
                    }
                    else if (count >= 1)
                    {
                        WedEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnThuEve:
                    //button thuEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Thursday Evening selected", ToastLength.Short).Show();
                        ThuEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[17] = "Thursday Evening";
                    }
                    else if (count >= 1)
                    {
                        ThuEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnFriEve:
                    //button friEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Friday Evening selected", ToastLength.Short).Show();
                        FriEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[18] = "Friday Evening";
                    }
                    else if (count >= 1)
                    {
                        FriEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSatEve:
                    //button satEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Saturday Evening selected", ToastLength.Short).Show();
                        SatEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[19] = "Saturday Evening";
                    }
                    else if (count >= 1)
                    {
                        SatEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSunEve:
                    //button sunEve was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Sunday Evening selected", ToastLength.Short).Show();
                        SunEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[20] = "Sunday Evening";
                    }
                    else if (count >= 1)
                    {
                        SunEve.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnMonNight:
                    //button monNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Monday Night selected", ToastLength.Short).Show();
                        MonNight.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[21] = "Monday Night";
                    }
                    else if (count >= 1)
                    {
                        MonNight.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnTueNight:
                    //button tueNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Tuesday Night selected", ToastLength.Short).Show();
                        TueNight.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[22] = "Tuesday Night";
                    }
                    else if (count >= 1)
                    {
                        TueNight.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnWedNight:
                    //button wedNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Wednesday Night selected", ToastLength.Short).Show();
                        WedNight.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[23] = "Wednesday Night";
                    }
                    else if (count >= 1)
                    {
                        WedNight.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnThuNight:
                    //button thuNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Thursday Night selected", ToastLength.Short).Show();
                        ThuNight.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[24] = "Thursday Night";
                    }
                    else if (count >= 1)
                    {
                        ThuNight.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnFriNight:
                    //button friNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Friday Night selected", ToastLength.Short).Show();
                        FriNight.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[25] = "Friday Night";
                    }
                    else if (count >= 1)
                    {
                        FriNight.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSatNight:
                    //button satNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Saturday Night selected", ToastLength.Short).Show();
                        SatNight.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[26] = "Saturday Night";
                    }
                    else if (count >= 1)
                    {
                        SatNight.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
                case Resource.Id.btnSunNight:
                    //button sunNight was clicked
                    if (count == 0)
                    {
                        Toast.MakeText(this, "Sunday Night selected", ToastLength.Short).Show();
                        SunNight.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                        count++;
                        values[27] = "Sunday Night";
                    }
                    else if (count >= 1)
                    {
                        SunNight.SetBackgroundColor(Android.Graphics.Color.LightGray);
                        count = 0;
                    }
                    break;
            }
        }

        private async void LoadData()
        {
            input_new_rate = FindViewById<EditText>(Resource.Id.payRate);
            input_new_age = FindViewById<EditText>(Resource.Id.age);

            string userLogin = auth.CurrentUser.Email;
            var firebase = new FirebaseClient(FirebaseURL);
            var items = await firebase
                .Child("parent")
                //.WithAuth(auth.CurrentUser.Uid)
                .OnceAsync<Parent>();
            list_parents.Clear();
            adapter = null;
            foreach (var item in items)
            {
                Parent account = new Parent();
                account.id = item.Key;
                account.name = item.Object.name;
                account.phone = item.Object.phone;
                account.city = item.Object.city;
                account.address = item.Object.address;
                account.email = item.Object.email;
                string email = account.email;
                account.eircode = item.Object.eircode;
                account.image = item.Object.image;

                if (userLogin == email) // a parent has logged into their profile - 
                {
                    input_new_email.Text = account.email;
                    input_new_name.Text = account.name;
                    input_new_city.Text = account.city;
                    input_new_phone.Text = account.phone;
                    input_new_address.Text = account.address;
                    input_new_eircode.Text = account.eircode;
                }
            }
            adapter = new ParentViewAdapter(this, list_parents);
            adapter.NotifyDataSetChanged();

            var sitters = await firebase
                .Child("babysitter")
                .OnceAsync<BabySitter>();
            list_babysitters.Clear();
            adapter = null;
            foreach (var item in sitters)
            {
                BabySitter account = new BabySitter();
                account.id = item.Key;
                account.name = item.Object.name;
                account.phone = item.Object.phone;
                account.city = item.Object.city;
                account.address = item.Object.address;
                account.email = item.Object.email;
                string email = account.email;
                account.eircode = item.Object.eircode;
                account.age = item.Object.age;
                account.image = item.Object.image;
                account.rate = item.Object.rate;

                if (sitters.Any((_) => _.Object.id == auth.CurrentUser.Uid)) // a babysitter has logged into their profile, display according to type - 
                {
                    input_new_age.Visibility = ViewStates.Visible;
                    payRateLayout.Visibility = ViewStates.Visible;
                }

                if (userLogin == email)
                {
                    input_new_email.Text = account.email;
                    input_new_name.Text = account.name;
                    input_new_city.Text = account.city;
                    input_new_phone.Text = account.phone;
                    input_new_age.Text = account.age.ToString();
                    input_new_address.Text = account.address;
                    input_new_eircode.Text = account.eircode;
                    input_new_rate.Text = account.rate.ToString();
                }
            }
            sitteradapter = new BabysitterViewAdapter(this, list_babysitters);
            sitteradapter.NotifyDataSetChanged();
        }

        private async void UpdateBabysitter(string uid, string newName, string newPhone, string newAddress, 
            string newCity, string newEmail, string newAge, string newEircode, decimal newRate)
        {
            //string uid = auth.CurrentUser.Uid;
            var firebase = new FirebaseClient(FirebaseURL);
            string userLogin = auth.CurrentUser.Email;
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

            var sitters = await firebase
                .Child("babysitter")
                .OnceAsync<BabySitter>();
            list_babysitters.Clear();
            adapter = null;
            foreach (var item in sitters)
            {
                BabySitter account = new BabySitter();
                account.id = item.Key;
                account.name = item.Object.name;
                account.age = item.Object.age;
                account.phone = item.Object.phone;
                account.city = item.Object.city;
                account.address = item.Object.address;
                account.gardaVetted = item.Object.gardaVetted;
                account.email = item.Object.email;
                string email = account.email;
                account.eircode = item.Object.eircode;
                account.image = item.Object.image;
                account.availability = item.Object.availability;
                account.rating = item.Object.rating;
                account.rate = item.Object.rate;

                if (userLogin == email)
                {
                    bool vetted = account.gardaVetted;
                    string availabilty = account.availability;
                    int rating = account.rating;

                    if (showAvailabiltyGrid == true)
                    {
                        availabilty = availableTime;
                    }

                    StorageReference userImage = storageRef.Child("user/profile pic/");
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("id").PutAsync(auth.CurrentUser.Uid);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("name").PutAsync(newName);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("phone").PutAsync(newPhone);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("address").PutAsync(newAddress);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("city").PutAsync(newCity);
                    //await firebase.Child("parent").Child(userEmail).Child("name").PutAsync(newPassword);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("email").PutAsync(newEmail);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("age").PutAsync(newAge);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("rate").PutAsync(newRate);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("availability").PutAsync(availabilty);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("eircode").PutAsync(newEircode);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("gardaVetted").PutAsync(vetted);
                    await firebase.Child("babysitter").Child(auth.CurrentUser.Uid).Child("rating").PutAsync(rating);
                }
            }
            LoadData();
            Toast.MakeText(this, "Details Updated.", ToastLength.Short).Show();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_profile, menu);
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
            else if (id == Resource.Id.menu_save) // Update users details
            {
                getUserType();
              
            }

            return base.OnOptionsItemSelected(item);
        }

        public async void getUserType()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            string userLogin = auth.CurrentUser.Email;

            var items = await firebase
                .Child("parent")
                //.WithAuth(auth.CurrentUser.Uid)
                .OnceAsync<Parent>();
            list_parents.Clear();
            foreach (var item in items)
            {
                Parent account = new Parent();
                account.email = item.Object.email;
                string parentEmail = account.email;

                if (userLogin == parentEmail) //  parent viewing their map - 
                {
                    UpdateParent(auth.CurrentUser.Uid, input_new_name.Text, input_new_phone.Text, input_new_address.Text,
                        input_new_city.Text, input_new_email.Text, input_new_eircode.Text);
                }         
            }
            var users = await firebase
                        .Child("babysitter")
                        .OnceAsync<BabySitter>();
            foreach (var sitter in users)
            {
                BabySitter getAccount = new BabySitter();
                getAccount.email = sitter.Object.email;

                if (userLogin == getAccount.email)
                {
                    UpdateBabysitter(auth.CurrentUser.Uid, input_new_name.Text, input_new_phone.Text, input_new_address.Text,
                        input_new_city.Text, input_new_email.Text, input_new_age.Text, input_new_eircode.Text, decimal.Parse(input_new_rate.Text));
                }
            }
        }
        private async void UpdateParent(string uid, string newName, string newPhone, string newAddress,
            string newCity, string newEmail, string newEircode)
        {
            var firebase = new FirebaseClient(FirebaseURL);

            string userLogin = auth.CurrentUser.Email;
            var items = await firebase
                .Child("parent")
                //.WithAuth(auth.CurrentUser.Uid)
                .OnceAsync<Parent>();
            list_parents.Clear();
            adapter = null;
            foreach (var item in items)
            {
                Parent account = new Parent();
                account.id = item.Key;
                account.name = item.Object.name;
                account.phone = item.Object.phone;
                account.city = item.Object.city;
                account.address = item.Object.address;
                account.noOfKids = item.Object.noOfKids;
                account.email = item.Object.email;
                string email = account.email;
                account.eircode = item.Object.eircode;
                account.image = item.Object.image;

                if (userLogin == email)
                {
                    int kidCount = account.noOfKids;
                    StorageReference userImage = storageRef.Child("user/profile pic/");
                    await firebase.Child("parent").Child(auth.CurrentUser.Uid).Child("id").PutAsync(auth.CurrentUser.Uid);
                    await firebase.Child("parent").Child(auth.CurrentUser.Uid).Child("name").PutAsync(newName);
                    await firebase.Child("parent").Child(auth.CurrentUser.Uid).Child("phone").PutAsync(newPhone);
                    await firebase.Child("parent").Child(auth.CurrentUser.Uid).Child("address").PutAsync(newAddress);
                    await firebase.Child("parent").Child(auth.CurrentUser.Uid).Child("city").PutAsync(newCity);
                    await firebase.Child("parent").Child(auth.CurrentUser.Uid).Child("noOfKids").PutAsync(kidCount);
                    await firebase.Child("parent").Child(auth.CurrentUser.Uid).Child("email").PutAsync(newEmail);
                    await firebase.Child("parent").Child(auth.CurrentUser.Uid).Child("eircode").PutAsync(newEircode);
                }
            }
            LoadData();
            Toast.MakeText(this, "Details Updated.", ToastLength.Short).Show();
        }

        private void ChooseImage()
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PICK_IMAGE_REQUSET);
        }
        private void UploadImage()
        {
            FirebaseUser user = auth.CurrentUser;
            string userEmail = user.Email;
            if (filePath != null)

                progressBar.Visibility = ViewStates.Visible;
            var images = storageRef.Child("Images/" + userEmail + "/profile pic/" + user.DisplayName);
            images.PutFile(filePath)
                .AddOnProgressListener(this)
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);
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
                    input_image.SetImageBitmap(bitmap);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                }
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
    }
}