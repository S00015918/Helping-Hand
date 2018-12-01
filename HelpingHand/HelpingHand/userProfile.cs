using System;
using System.Collections;
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
using HelpingHand.Model;
using Java.Lang;
using Newtonsoft.Json;
using XamarinFirebaseAuth;

namespace HelpingHand
{
    [Activity(Label = "Profile", Theme = "@style/AppTheme")]
    public class userProfile : AppCompatActivity, IOnProgressListener, IOnSuccessListener, IOnFailureListener
    {
        EditText input_new_password, input_new_name, input_new_email,
            input_new_address, input_new_phone, input_new_city;
        ImageView input_image, upload_image;

        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        private const int PICK_IMAGE_REQUSET = 71;
        ProgressBar progressBar;
        FirebaseStorage storage;
        StorageReference storageRef;
        private List<Parent> list_parents = new List<Parent>();
        //List<BabySitter> list_babySitters = new List<BabySitter>();
        private Android.Net.Uri filePath;
        private ListViewAdapter adapter;
        //Parent selectedParent;
        //BabySitter selectedBabysitter;

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

            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            //SupportActionBar.Title = auth.CurrentUser.Email;

            //View
            activity_userProfile = FindViewById<RelativeLayout>(Resource.Id.activity_userProfile);
            activity_Rlayout = FindViewById<RelativeLayout>(Resource.Id.Rlayout);

            input_image = FindViewById<ImageView>(Resource.Id.imgUser);
            upload_image = FindViewById<ImageView>(Resource.Id.uploadImage);
            input_new_name = FindViewById<EditText>(Resource.Id.name);
            input_new_email = FindViewById<EditText>(Resource.Id.email);
            //input_new_password = FindViewById<EditText>(Resource.Id.signup_password);
            input_new_phone = FindViewById<EditText>(Resource.Id.phone);
            input_new_address = FindViewById<EditText>(Resource.Id.age);
            input_new_city = FindViewById<EditText>(Resource.Id.city);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar);
            progressBar.Visibility = ViewStates.Invisible;

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

        private async void LoadData()
        {
            System.String frstname = input_new_name.GetType().ToString().Trim();
            System.String city = input_new_city.GetType().ToString().Trim();
            System.String phone = input_new_phone.GetType().ToString().Trim();
            string userLogin = auth.CurrentUser.Email;
            var firebase = new FirebaseClient(FirebaseURL);
            var items = await firebase
                .Child("parent")
                .OnceAsync<Parent>();
            //list_parents.Clear();
            //adapter = null;
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
                account.ImageUrl = item.Object.ImageUrl;

                if (userLogin == email)
                {
                    input_new_email.Text = account.email;
                    input_new_name.Text = account.name;
                    input_new_city.Text = account.city;
                    input_new_phone.Text = account.phone;
                    input_new_address.Text = account.address;
                }
             }

                //adapter.NotifyDataSetChanged();

          }

        private async void UpdateUser(string userEmail, string newName, string newCity, string newPassword, string newEmail)
        {
            var firebase = new FirebaseClient(FirebaseURL);
            var user = firebase.Child("parent").Child(auth.CurrentUser.Email);

            await firebase.Child("parent").Child(userEmail).Child("id").PutAsync(userEmail);
            await firebase.Child("parent").Child(userEmail).Child("name").PutAsync(newName);
            await firebase.Child("parent").Child(userEmail).Child("city").PutAsync(newCity);
            await firebase.Child("parent").Child(userEmail).Child("name").PutAsync(newPassword);
            await firebase.Child("parent").Child(userEmail).Child("city").PutAsync(newEmail);
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
                StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                Finish();
            }
            else if (id == Resource.Id.menu_save) //save new data
            {
                UpdateUser(auth.CurrentUser.Email, input_new_name.Text, input_new_city.Text, input_new_password.Text, input_new_email.Text);
            }

            return base.OnOptionsItemSelected(item);
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
            if (filePath != null)

                progressBar.Visibility = ViewStates.Visible;
            var images = storageRef.Child("images/" + Guid.NewGuid().ToString());
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
            //progressBar.Visibility.ToString("Uploaded " + (int)progress + " %");
        }
        public void OnSuccess(Java.Lang.Object result)
        {
            progressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(this, "Uploaded Successful", ToastLength.Short).Show();
        }
        public void OnFailure(Java.Lang.Exception e)
        {
            progressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(this, "" + e.Message, ToastLength.Short).Show();
        }
    }
}