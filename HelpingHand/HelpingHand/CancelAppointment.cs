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

namespace HelpingHand
{
    [Activity(Label = "CancelAppointment")]
    public class CancelAppointment : AppCompatActivity
    {
        private ListView list_data;
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        FirebaseAuth auth;
        List<Appointment> list_appointments = new List<Appointment>();
        private AppointmentListAdapter AppointmentAdapter;
        Appointment selectedAppointment;
        string selectedUser;

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
                account.startTime = item.Object.startTime;
                account.endTime = item.Object.endTime;
                account.userEmail = item.Object.userEmail;
                account.babysitterEmail = item.Object.babysitterEmail;
                string parentEmail = account.userEmail;
                list_appointments.Add(account);

                if (auth.CurrentUser.Email == parentEmail )
                {
                    AppointmentAdapter = new AppointmentListAdapter(this, list_appointments);
                    AppointmentAdapter.NotifyDataSetChanged();
                    list_data.Adapter = AppointmentAdapter;
                }
            }

            list_data.ItemSelected += (s, e) =>
            {
                selectedAppointment = list_appointments[e.Position];
                selectedUser = list_appointments[e.Position].userEmail;

            };
        }

        public async void DeleteAppointment(string email)
        {
            var firebase = new FirebaseClient(FirebaseURL);
            var toDeleteAppointment = (await firebase
              .Child("appointment")
              .OnceAsync<Appointment>()).Where(a => a.Object.userEmail == email).FirstOrDefault();

            await firebase.Child("appointment").Child(toDeleteAppointment.Key).DeleteAsync();

        }

        private async void BtnDelete_Clicked(object sender, EventArgs e)
        {
            string selected = list_data.SelectedItem.ToString();
            DeleteAppointment(selectedUser);
        }
    }
}