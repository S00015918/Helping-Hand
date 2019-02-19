using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using HelpingHand.Model;
using Newtonsoft.Json;
using XamarinFirebaseAuth;

namespace HelpingHand
{
    [Activity(Label = "View User Profile", Theme = "@style/AppTheme")]
    public class viewUser : AppCompatActivity
    {
        Button btnCreateAppointment;
        TextView userName, userAge, userEmail, userAddress, userCity, userPhone, userEircode;
        ImageView userImage;
        FirebaseAuth auth;
        Array userAvailabilty;
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.user_view);
            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            userName = FindViewById<TextView>(Resource.Id.name);
            userAge = FindViewById<TextView>(Resource.Id.age);
            userEmail = FindViewById<TextView>(Resource.Id.email);
            userAddress = FindViewById<TextView>(Resource.Id.address);
            userCity = FindViewById<TextView>(Resource.Id.city);
            userPhone = FindViewById<TextView>(Resource.Id.phone);
            userEircode = FindViewById<TextView>(Resource.Id.eircode);
            userImage = FindViewById<ImageView>(Resource.Id.imgUser);

            btnCreateAppointment = FindViewById<Button>(Resource.Id.btnCreate_appointment);
            btnCreateAppointment.Click += CreateAppointment;

            string babysitter = this.Intent.GetStringExtra("KEY");

            BabySitter userSitter = JsonConvert.DeserializeObject<BabySitter>(babysitter);

            if (userSitter.id != auth.CurrentUser.Uid)
            {
                userName.Text = userSitter.name;
                userAge.Text = Convert.ToString(userSitter.age);
                userEmail.Text = userSitter.email;
                userAddress.Text = userSitter.address;
                userCity.Text = userSitter.city;
                userPhone.Text = userSitter.phone;
                userEircode.Text = userSitter.eircode;
                userAvailabilty = userSitter.availability;
                //userImage.ImageMatrix = user.ImageUrl;
            }
            else
            {
                string _parent = this.Intent.GetStringExtra("KEY");
                Parent userParent = JsonConvert.DeserializeObject<Parent>(_parent);

                userName.Text = userParent.name;
                userEmail.Text = userParent.email;
                userAddress.Text = userParent.address;
                userCity.Text = userParent.city;
                userPhone.Text = userParent.phone;
                userEircode.Text = userParent.eircode;
            }

            RatingBar ratingbar = FindViewById<RatingBar>(Resource.Id.ratingbar);

            ratingbar.RatingBarChange += (o, e) => {
                Toast.MakeText(this, "New Rating: " + ratingbar.Rating.ToString(), ToastLength.Short).Show();

                int newrating = Convert.ToInt32(ratingbar.Rating.ToString());

                UpdateUser(userSitter.id, newrating);
            };
        }


        void CreateAppointment(object sender, EventArgs e)
        {
            string babysitter = this.Intent.GetStringExtra("KEY");

            BabySitter userSitter = JsonConvert.DeserializeObject<BabySitter>(babysitter);

            userName.Text = userSitter.name;
            userAge.Text = Convert.ToString(userSitter.age);
            userEmail.Text = userSitter.email;
            userAddress.Text = userSitter.address;
            userCity.Text = userSitter.city;
            userPhone.Text = userSitter.phone;
            userEircode.Text = userSitter.eircode;
            userAvailabilty = userSitter.availability;

            var userJson = JsonConvert.SerializeObject(userSitter);

            var appointment = new Intent(this, typeof(CreateAppointment));
            appointment.PutExtra("KEY", Convert.ToString(userSitter));
            StartActivity(appointment);
            Finish();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_viewUser, menu);
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
            else if (id == Resource.Id.menu_message) // message user
            {
                StartActivity(new Android.Content.Intent(this, typeof(MessageActivity)));
                Finish();
            }
            else if (id == Resource.Id.menu_star) //favourites
            {
                StartActivity(new Android.Content.Intent(this, typeof(userFavourites)));
                Finish();
            }

            return base.OnOptionsItemSelected(item);
        }

        private async void UpdateUser(string uid, int rating)
        {
            var firebase = new FirebaseClient(FirebaseURL);

            await firebase.Child("babysitter").Child(uid).Child("id").PutAsync(uid);
            await firebase.Child("babysitter").Child(uid).Child("rating").PutAsync(rating);

            Toast.MakeText(this, "Details Updated.", ToastLength.Short).Show();
        }
    }
}