using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        Button btnMonMorn, btnTueMorn, btnWedMorn, btnThuMorn, btnFriMorn, btnSatMorn, btnSunMorn,
            btnMonAft, btnTueAft, btnWedAft, btnThuAft, btnFriAft, btnSatAft, btnSunAft,
            btnMonEve, btnTueEve, btnWedEve, btnThuEve, btnFriEve, btnSatEve, btnSunEve,
            btnMonNigh, btnTueNigh, btnWedNigh, btnThuNigh, btnFriNigh, btnSatNigh, btnSunNigh;
        Button btnCreateAppointment;
        TextView userName, userAge, userEmail, userAddress, userCity, userPhone, userEircode;
        ImageView userImage;
        FirebaseAuth auth;
        RatingBar ratingbar;
        int userRating, returnRating; string userRated, ratedBabysitter, RatersEmail, userRatedName;

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

            ratingbar = FindViewById<RatingBar>(Resource.Id.ratingbar);

            btnCreateAppointment = FindViewById<Button>(Resource.Id.btnCreate_appointment);
            btnCreateAppointment.Click += CreateAppointment;

            string babysitter = this.Intent.GetStringExtra("KEY");
            BabySitter userSitter = JsonConvert.DeserializeObject<BabySitter>(babysitter);

            btnMonMorn = FindViewById<Button>(Resource.Id.btnMonMorn); btnMonMorn.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnTueMorn = FindViewById<Button>(Resource.Id.btnTueMorn); btnTueMorn.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnWedMorn = FindViewById<Button>(Resource.Id.btnWedMorn); btnWedMorn.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnThuMorn = FindViewById<Button>(Resource.Id.btnThuMorn); btnThuMorn.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnFriMorn = FindViewById<Button>(Resource.Id.btnFriMorn); btnFriMorn.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnSatMorn = FindViewById<Button>(Resource.Id.btnSatMorn); btnSatMorn.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnSunMorn = FindViewById<Button>(Resource.Id.btnSunMorn); btnSunMorn.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnMonAft = FindViewById<Button>(Resource.Id.btnMonAfter); btnMonAft.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnTueAft = FindViewById<Button>(Resource.Id.btnTueAfter); btnTueAft.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnWedAft = FindViewById<Button>(Resource.Id.btnWedAfter); btnWedAft.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnThuAft = FindViewById<Button>(Resource.Id.btnThuAfter); btnThuAft.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnFriAft = FindViewById<Button>(Resource.Id.btnFriAfter); btnFriAft.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnSatAft = FindViewById<Button>(Resource.Id.btnSatAfter); btnSatAft.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnSunAft = FindViewById<Button>(Resource.Id.btnSunAfter); btnSunAft.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnMonEve = FindViewById<Button>(Resource.Id.btnMonEve); btnMonEve.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnTueEve = FindViewById<Button>(Resource.Id.btnTueEve); btnTueEve.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnWedEve = FindViewById<Button>(Resource.Id.btnWedEve);  btnWedEve.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnThuEve = FindViewById<Button>(Resource.Id.btnThuEve); btnThuEve.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnFriEve = FindViewById<Button>(Resource.Id.btnFriEve); btnFriEve.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnSatEve = FindViewById<Button>(Resource.Id.btnSatEve); btnSatEve.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnSunEve = FindViewById<Button>(Resource.Id.btnSunEve); btnSunEve.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnMonNigh = FindViewById<Button>(Resource.Id.btnMonNight); btnMonNigh.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnTueNigh = FindViewById<Button>(Resource.Id.btnTueNight); btnTueNigh.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnWedNigh = FindViewById<Button>(Resource.Id.btnWedNight); btnWedNigh.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnThuNigh = FindViewById<Button>(Resource.Id.btnThuNight);  btnThuNigh.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnFriNigh = FindViewById<Button>(Resource.Id.btnFriNight); btnFriNigh.SetBackgroundColor(Android.Graphics.Color.IndianRed); btnSatNigh = FindViewById<Button>(Resource.Id.btnSatNight); btnSatNigh.SetBackgroundColor(Android.Graphics.Color.IndianRed);
            btnSunNigh = FindViewById<Button>(Resource.Id.btnSunNight);
            btnSunNigh.SetBackgroundColor(Android.Graphics.Color.IndianRed);

            if (userSitter.id != auth.CurrentUser.Uid)
            {
                userName.Text = userSitter.name;
                userAge.Text = Convert.ToString(userSitter.age);
                userEmail.Text = userSitter.email;
                userAddress.Text = userSitter.address;
                userCity.Text = userSitter.city;
                userPhone.Text = userSitter.phone;
                userEircode.Text = userSitter.eircode;
                string userAvailabilty = userSitter.availability;
                userRated = userEmail.Text;
                userRatedName = userName.Text;
                //userImage.ImageMatrix = user.ImageUrl;
                string[] availabilty = userAvailabilty.Split(',');
                foreach (var item in availabilty)
                {
                    string dayTime = item;
                    if (dayTime.Contains("Monday Morning"))
                    {
                        btnMonMorn = FindViewById<Button>(Resource.Id.btnMonMorn);
                        btnMonMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Tuesday Morning"))
                    {
                        btnTueMorn = FindViewById<Button>(Resource.Id.btnTueMorn);
                        btnTueMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Wednesday Morning"))
                    {
                        btnWedMorn = FindViewById<Button>(Resource.Id.btnWedMorn);
                        btnWedMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Thursday Morning"))
                    {
                        btnThuMorn = FindViewById<Button>(Resource.Id.btnThuMorn);
                        btnThuMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Friday Morning"))
                    {
                        btnFriMorn = FindViewById<Button>(Resource.Id.btnFriMorn);
                        btnFriMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Saturday Morning"))
                    {
                        btnSatMorn = FindViewById<Button>(Resource.Id.btnSatMorn);
                        btnSatMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Sunday Morning"))
                    {
                        btnSunMorn = FindViewById<Button>(Resource.Id.btnSunMorn);
                        btnSunMorn.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Monday Afternoon"))
                    {
                        btnMonAft = FindViewById<Button>(Resource.Id.btnMonAfter);
                        btnMonAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Tuesday Afternoon"))
                    {
                        btnTueAft = FindViewById<Button>(Resource.Id.btnTueAfter);
                        btnTueAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Wednesday Afternoon"))
                    {
                        btnWedAft = FindViewById<Button>(Resource.Id.btnWedAfter);
                        btnWedAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Thursday Afternoon"))
                    {
                        btnThuAft = FindViewById<Button>(Resource.Id.btnThuAfter);
                        btnThuAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Friday Afternoon"))
                    {
                        btnFriAft = FindViewById<Button>(Resource.Id.btnFriAfter);
                        btnFriAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Saturday Afternoon"))
                    {
                        btnSatAft = FindViewById<Button>(Resource.Id.btnSatAfter);
                        btnSatAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Sunday Afternoon"))
                    {
                        btnSunAft = FindViewById<Button>(Resource.Id.btnSunAfter);
                        btnSunAft.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Monday Evening"))
                    {
                        btnMonEve = FindViewById<Button>(Resource.Id.btnMonEve);
                        btnMonEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Tuesday Evening"))
                    {
                        btnTueEve = FindViewById<Button>(Resource.Id.btnTueEve);
                        btnTueEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Wednesday Evening"))
                    {
                        btnWedEve = FindViewById<Button>(Resource.Id.btnWedEve);
                        btnWedEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Thursday Evening"))
                    {
                        btnThuEve = FindViewById<Button>(Resource.Id.btnThuEve);
                        btnThuEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Friday Evening"))
                    {
                        btnFriEve = FindViewById<Button>(Resource.Id.btnFriEve);
                        btnFriEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Saturday Evening"))
                    {
                        btnSatEve = FindViewById<Button>(Resource.Id.btnSatEve);
                        btnSatEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Sunday Evening"))
                    {
                        btnSunEve = FindViewById<Button>(Resource.Id.btnSunEve);
                        btnSunEve.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Monday Night"))
                    {
                        btnMonNigh = FindViewById<Button>(Resource.Id.btnMonNight);
                        btnMonNigh.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Tuesday Night"))
                    {
                        btnTueNigh = FindViewById<Button>(Resource.Id.btnTueNight);
                        btnTueNigh.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Wednesday Night"))
                    {
                        btnWedNigh = FindViewById<Button>(Resource.Id.btnWedNight);
                        btnWedNigh.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Thursday Night"))
                    {
                        btnThuNigh = FindViewById<Button>(Resource.Id.btnThuNight);
                        btnThuNigh.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Friday Night"))
                    {
                        btnFriNigh = FindViewById<Button>(Resource.Id.btnFriNight);
                        btnFriNigh.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Saturday Night"))
                    {
                        btnSatNigh = FindViewById<Button>(Resource.Id.btnSatNight);
                        btnSatNigh.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                    if (dayTime.Contains("Sunday Night"))
                    {
                        btnSunNigh = FindViewById<Button>(Resource.Id.btnSunNight);
                        btnSunNigh.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                    }
                }

                ratingbar.NumStars = userRating;
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

                btnCreateAppointment.Visibility = ViewStates.Invisible;
            }

            getRatings();

            ratingbar.RatingBarChange += (o, e) => {

                userRating = Convert.ToInt32(ratingbar.Rating.ToString());

                UpdateUserRating(userRated, userRating, userRatedName);
            };
        }

        public async void getRatings()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            var Ratings = await firebase
                        .Child("rating").Child(auth.CurrentUser.Uid)
                        .OnceAsync<Rating>();
            foreach (var rating in Ratings)
            {
                Rating account = new Rating();
                account.userRatedEmail = rating.Object.userRatedEmail;
                account.rating = rating.Object.rating;
                account.ratedByEmail = rating.Object.ratedByEmail;

                returnRating = account.rating;
                ratedBabysitter = account.userRatedEmail;

                string babysitter = this.Intent.GetStringExtra("KEY");
                BabySitter userSitter = JsonConvert.DeserializeObject<BabySitter>(babysitter);

                if (userSitter.email == ratedBabysitter && account.ratedByEmail == auth.CurrentUser.Email)
                {
                    RatingBar ratingbar = FindViewById<RatingBar>(Resource.Id.ratingbar);
                    ratingbar.Rating = returnRating;
                }
            }
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
            //userAvailabilty = userSitter.availability;

            var userJson = JsonConvert.SerializeObject(userSitter);

            var appointment = new Intent(this, typeof(CreateAppointment));
            appointment.PutExtra("KEY", Convert.ToString(userJson));
            StartActivity(appointment);
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
                StartActivity(new Android.Content.Intent(this, typeof(HomeActivity)));
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

        private async void UpdateUserRating(string userRated, int rating, string ratedName)
        {
            var babysitterEmailList = new List<string>();
            var parentEmailList = new List<string>();

            var firebase = new FirebaseClient(FirebaseURL);
            // Get rating details- who rated, rating score and who is being rated
            Rating _rating = new Rating();
            _rating.ratedByEmail = auth.CurrentUser.Email;
            _rating.rating = rating;
            _rating.userRatedEmail = userRated;
            _rating.ratedUsersName = ratedName;
            string ratedByEmail = _rating.ratedByEmail;

            //Add Item
            //await firebase.Child("rating").Child(auth.CurrentUser.Uid).PostAsync<Rating>(_rating);
            var checkRatings = await firebase
                        .Child("rating").Child(auth.CurrentUser.Uid)
                        .OnceAsync<Rating>();
            foreach (var rated in checkRatings)
            {
                // Get all ratings already stored in the database - rater, rating and rated
                Rating account = new Rating();
                account.userRatedEmail = rated.Object.userRatedEmail;
                account.ratedByEmail = rated.Object.ratedByEmail;
                account.rating = rated.Object.rating;

                RatersEmail = account.ratedByEmail;
                ratedBabysitter = account.userRatedEmail;
                babysitterEmailList.Add(account.userRatedEmail);
                parentEmailList.Add(account.ratedByEmail);

            }
            var ratedByEmailList = new HashSet<string>(parentEmailList).ToList();

            if (ratedByEmailList.Contains(ratedByEmail)) // Has the parent rated before
            {
                if (babysitterEmailList.Contains(userRated)) // Has the babysitter been rated before
                {
                    if (ratedByEmail == RatersEmail) // Which parent is rating?
                    {
                        foreach (var item in babysitterEmailList)
                        {
                            if (userRated == item)
                            {
                                var toUpdateRating = (await firebase
                              .Child("rating").Child(auth.CurrentUser.Uid)
                              .OnceAsync<Rating>()).Where(a => a.Object.userRatedEmail == userRated).FirstOrDefault();

                                await firebase
                                  .Child("rating")
                                  .Child(auth.CurrentUser.Uid)
                                  .Child(toUpdateRating.Key)
                                  .PutAsync(new Rating() { ratedByEmail = ratedByEmail, rating = rating, userRatedEmail = userRated, ratedUsersName = ratedName });

                                Toast.MakeText(this, "Rating: " + ratingbar.Rating.ToString() + " Stars", ToastLength.Short).Show();
                            }
                            else { } // No update or add needed
                        }
                    }
                    else { } // No update or add needed
                }
                else { AddRating(ratedByEmail, rating, userRated, ratedName); }
            }
            else { AddRating(ratedByEmail, rating, userRated, ratedName); }
        }

        public async void AddRating(string _ratedByEmail, int userRating, string _userRatedEmail, string _userRatedName)
        {
            var firebase = new FirebaseClient(FirebaseURL);
            await firebase
              .Child("rating").Child(auth.CurrentUser.Uid)
              .PostAsync(new Rating() { ratedByEmail = _ratedByEmail, rating = userRating, userRatedEmail = _userRatedEmail, ratedUsersName = _userRatedName });
            Toast.MakeText(this, "Rating: " + ratingbar.Rating.ToString() + " Stars", ToastLength.Short).Show();
        }
    }
}