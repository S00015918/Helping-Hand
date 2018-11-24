using System;
using System.Collections;
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
using HelpingHand.Model;

namespace HelpingHand
{
    [Activity(Label = "Profile", Theme = "@style/AppTheme")]
    public class userProfile : AppCompatActivity
    {
        EditText input_new_password, input_name, input_email;
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        //private List<Parent> list_parents = new List<Parent>();
        List<BabySitter> list_babySitters = new List<BabySitter>();

        private ListViewAdapter adapter;
        //Parent selectedParent;
        //BabySitter selectedBabysitter;

        RelativeLayout activity_userProfile;
        FirebaseAuth auth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profilePage);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = auth.CurrentUser.Email;

            //View
            activity_userProfile = FindViewById<RelativeLayout>(Resource.Id.activity_userProfile);

            //LoadData();
        }

        private async void LoadData()
        {
            //String uid = FirebaseAuth.GetInstance().CurrentUser.Uid;
            var firebase = new FirebaseClient(FirebaseURL);
            var items = await firebase
                .Child("parent")
                .OnceAsync<Parent>();
            list_babySitters.Clear();
            adapter = null;
            if (auth.CurrentUser == items)
            {
                Parent account = new Parent();
            }
            adapter.NotifyDataSetChanged();
 
        }
    }
}