using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            input_new_age.Visibility = ViewStates.Invisible;
            payRateLayout.Visibility = ViewStates.Invisible;
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

                if (userLogin == email)
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

                if (sitters.Any((_) => _.Object.id == auth.CurrentUser.Uid))
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

        private async void UpdateUser(string uid, string newName, string newPhone, string newAddress, 
            string newCity, string newEmail, string newAge, string newEircode, decimal newRate)
        {
            //string uid = auth.CurrentUser.Uid;
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
                    //await firebase.Child("parent").Child(userEmail).Child("name").PutAsync(newPassword);
                    await firebase.Child("parent").Child(auth.CurrentUser.Uid).Child("email").PutAsync(newEmail);
                    await firebase.Child("parent").Child(auth.CurrentUser.Uid).Child("eircode").PutAsync(newEircode);
                }
            }

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
                StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                Finish();
            }
            else if (id == Resource.Id.menu_save) // Update users details
            {
                UpdateUser(auth.CurrentUser.Uid, input_new_name.Text, input_new_phone.Text, input_new_address.Text,
                    input_new_city.Text, input_new_email.Text, input_new_age.Text, input_new_eircode.Text, decimal.Parse(input_new_rate.Text));
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