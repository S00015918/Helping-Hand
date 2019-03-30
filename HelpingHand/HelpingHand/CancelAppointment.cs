using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
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
    [Activity(Label = "Cancel Appointment", Theme = "@style/AppTheme")]
    public class CancelAppointment : AppCompatActivity
    {
        private ListView list_data;
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;
        List<Appointment> list_appointments = new List<Appointment>();
        private AppointmentListAdapter AppointmentAdapter;
        Appointment selectedAppointment;
        string selectedUser, babysitterEmail;
        Button btnCancelAppointment;
        DateTime selectedDate;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.cancel_appointment);
            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            var firebase = new FirebaseClient(FirebaseURL);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            list_data = FindViewById<ListView>(Resource.Id.list_appointments);
            list_data.Visibility = ViewStates.Invisible;

            var items = await firebase
                    .Child("appointment")
                    .OnceAsync<Appointment>();
            list_appointments.Clear();
            AppointmentAdapter = null;
            foreach (var item in items)
            {
                Appointment account = new Appointment();
                account.Parent = item.Object.Parent;
                account.Babysitter = item.Object.Babysitter;
                account.Date = item.Object.Date;
                string dateTime = account.Date.ToString();
                account.startTime = item.Object.startTime;
                account.endTime = item.Object.endTime;
                account.userEmail = item.Object.userEmail;
                account.babysitterEmail = item.Object.babysitterEmail;
                string parentEmail = account.userEmail;
                string babysitterEmail = account.babysitterEmail;

                if (auth.CurrentUser.Email == parentEmail )
                {
                    string day = account.Date.Day.ToString();
                    string month = account.Date.Month.ToString();
                    string year = account.Date.Year.ToString();
                    string _date = year + month + day;
                    int date = int.Parse(_date);
                    string todaysDay = DateTime.Today.Day.ToString();
                    string todaysMonth = DateTime.Today.Month.ToString();
                    string todaysYear = DateTime.Today.Year.ToString();
                    string _todaysDate = todaysYear + todaysMonth + todaysDay;
                    int todaysDate = int.Parse(_todaysDate);
                    if (date > todaysDate)
                    {
                        list_appointments.Add(account);
                        AppointmentAdapter = new AppointmentListAdapter(this, list_appointments);
                    }
                    if (date < todaysDate -1)
                    {
                        // Delete appointments more than a month old
                    }

                }
                if(auth.CurrentUser.Email == babysitterEmail)
                {
                    string day = account.Date.Day.ToString();
                    string month = account.Date.Month.ToString();
                    string year = account.Date.Year.ToString();
                    string _date = year + month + day;
                    int date = int.Parse(_date);
                    string todaysDay = DateTime.Today.Day.ToString();
                    string todaysMonth = DateTime.Today.Month.ToString();
                    string todaysYear = DateTime.Today.Year.ToString();
                    string _todaysDate = todaysYear + todaysMonth + todaysDay;
                    int todaysDate = int.Parse(_todaysDate);
                    if (date > todaysDate)
                    {
                        list_appointments.Add(account);
                        AppointmentAdapter = new AppointmentListAdapter(this, list_appointments);
                    }
                }
            }

            list_data.Visibility = ViewStates.Visible;
            AppointmentAdapter.NotifyDataSetChanged();
            list_data.Adapter = AppointmentAdapter;

            list_data.ItemClick += (s, e) =>
            {
                selectedAppointment = list_appointments[e.Position];
                selectedUser = list_appointments[e.Position].babysitterEmail;
                selectedDate = list_appointments[e.Position].Date;
                string selectedBabysitter = list_appointments[e.Position].Babysitter;

                //list_data.SetBackgroundColor(Android.Graphics.Color.Beige); 

                Toast.MakeText(this, selectedBabysitter + " Selected", ToastLength.Short).Show();
            };

            //btnCancelAppointment.ItemClick += MListView_ItemClick;

            btnCancelAppointment = FindViewById<Button>(Resource.Id.btnDelete);
            btnCancelAppointment.Click += delegate
            {
                DeleteAppointment();
            };
        }

        private async void DeleteAppointment()
        {
            string appointmentDate = selectedDate.ToString();
            string[] getDate = appointmentDate.Split(' ');
            string date = getDate[0];

            var firebase = new FirebaseClient(FirebaseURL);
            var babysittters = await firebase
                    .Child("babysitter")
                    .OnceAsync<BabySitter>();

            foreach (var item in babysittters)
            {
                BabySitter sitter = new BabySitter();
                sitter.email = item.Object.email;
                babysitterEmail = sitter.email;


                if (babysitterEmail == selectedUser)
                {
                    var toDeleteAppointment = (await firebase
                      .Child("appointment")
                      .OnceAsync<Appointment>()).Where(a => a.Object.Date == selectedDate).FirstOrDefault();

                    await firebase.Child("appointment").Child(toDeleteAppointment.Key).DeleteAsync();
                }
                else { }
            }
            AppointmentAdapter.NotifyDataSetChanged();
            list_data.Adapter = AppointmentAdapter;
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
    }
}