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
using Xamarin.Essentials;
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
        string selectedUser, appointmentCreator;
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
                    if (account.Date > DateTime.Now)
                    {
                        list_appointments.Add(account);
                        AppointmentAdapter = new AppointmentListAdapter(this, list_appointments);
                    }
                    if (account.Date.Year == DateTime.Now.Year)
                    {
                        if (account.Date.Month < DateTime.Now.Month - 1)
                        {
                            // Delete appointments more than a month old
                        }
                    }
                    else { list_data.Visibility = ViewStates.Invisible; }

                }
                if(auth.CurrentUser.Email == babysitterEmail)
                {
                    if (account.Date > DateTime.Now)
                    {
                        list_appointments.Add(account);
                        AppointmentAdapter = new AppointmentListAdapter(this, list_appointments);
                    }
                    if (account.Date.Year == DateTime.Now.Year)
                    {
                        if (account.Date.Month < DateTime.Now.Month - 1)
                        {
                            // Delete appointments more than a month old
                        }
                    }
                    else
                    {
                        list_data.Visibility = ViewStates.Invisible;
                    }
                }
            }

            list_data.Visibility = ViewStates.Visible;
            //AppointmentAdapter.NotifyDataSetChanged();
            list_data.Adapter = AppointmentAdapter;

            list_data.ItemClick += (s, e) =>
            {
                selectedAppointment = list_appointments[e.Position];
                selectedUser = list_appointments[e.Position].babysitterEmail;
                appointmentCreator = list_appointments[e.Position].userEmail;
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

            if (selectedUser == auth.CurrentUser.Email)
            {
                var toDeleteAppointment = (await firebase
                    .Child("appointment")
                    .OnceAsync<Appointment>()).Where(a => a.Object.Date == selectedDate).FirstOrDefault();

                await firebase.Child("appointment").Child(toDeleteAppointment.Key).DeleteAsync();

                string Subject = "HelpingHand Appointment Cancellation";
                string Body = "The date of cancelled appointment: " + date;
                var EmailList = new List<string>();
                EmailList.Add(auth.CurrentUser.Email);
                EmailList.Add(selectedUser);

                SendEmail(Subject, Body, EmailList);
            }
            else { }

            if (appointmentCreator == auth.CurrentUser.Email)
            {
                var toDeleteAppointment = (await firebase
                    .Child("appointment")
                    .OnceAsync<Appointment>()).Where(a => a.Object.Date == selectedDate).FirstOrDefault();

                await firebase.Child("appointment").Child(toDeleteAppointment.Key).DeleteAsync();

                string Subject = "HelpingHand Appointment Cancellation";
                string Body = "The date of cancelled appointment: " + date;
                var EmailList = new List<string>();
                EmailList.Add(auth.CurrentUser.Email);
                EmailList.Add(selectedUser);

                SendEmail(Subject, Body, EmailList);
            }
            else { }

            AppointmentAdapter.NotifyDataSetChanged();
            list_data.Adapter = AppointmentAdapter;
        }

        public async void SendEmail(string subject, string body, List<string> recipients)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                    //Cc = ccRecipients,
                    //Bcc = bccRecipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                fbsEx.Message.ToString();
                // Email is not supported on this device

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                // Some other exception occurred
            }
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