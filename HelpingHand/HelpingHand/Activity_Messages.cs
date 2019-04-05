using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Toptoche.Searchablespinnerlibrary;
using Firebase.Database;
using HelpingHand.Interface;
using HelpingHand.Model;
using static Android.Widget.AdapterView;

namespace HelpingHand
{
    [Activity(Label = "Messages")]
    public class Activity_messages : AppCompatActivity, IFirebaseLoadDone, IValueEventListener, IOnItemSelectedListener
    {
        SearchableSpinner searchableSpinner;
        DatabaseReference movieRef;
        IFirebaseLoadDone firebaseLoadDone;
        List<Model.BabySitter> babySitters = new List<Model.BabySitter>();
        BottomSheetDialog bottomSheetDialog;

        //Dialog view
        TextView babysitter_name, babysitter_email;
        FloatingActionButton btn_fav;
        bool isFirstTime = true;

        public void OnCancelled(DatabaseError error)
        {
            firebaseLoadDone.OnFirebaseLoadFailed(error.Message);
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            List<BabySitter> local = new List<BabySitter>();
            foreach (DataSnapshot dataSnapShot in snapshot.Children.ToEnumerable())
            {
                BabySitter babysitter = new BabySitter();
                babysitter.name = dataSnapShot.Child("name").GetValue(true).ToString();
                babysitter.email = dataSnapShot.Child("email").GetValue(true).ToString();
                local.Add(babysitter);
            }
            firebaseLoadDone.OnFirebaseLoadSuccess(local);
        }

        public void OnFirebaseLoadFailed(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        public void OnFirebaseLoadSuccess(List<Model.BabySitter> babySitters)
        {
            this.babySitters = babySitters;
            //get All name
            List<string> babysitter_list = new List<string>();
            foreach (var babysitter in babySitters)
            {
                babysitter_list.Add(babysitter.name);
            }
            // Create Adapter
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, babysitter_list);
            searchableSpinner.Adapter = adapter;
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            // Fix Error first selected (Always fire this event when app starts)
            if (!isFirstTime)
            {
                Model.BabySitter babysitter = babySitters[position];
                babysitter_name.Text = babysitter.name;
                babysitter_email.Text = babysitter.email;

                bottomSheetDialog.Show();
            }
            else { isFirstTime = false; }
        }

        public void OnNothingSelected(AdapterView parent)
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.messages_main);

            //Init DB
            movieRef = FirebaseDatabase.Instance.GetReference("babysitter");
            movieRef.AddListenerForSingleValueEvent(this);

            //Init interface
            firebaseLoadDone = this;

            //Init Dialog
            bottomSheetDialog = new BottomSheetDialog(this);
            View bottom_sheet_view = LayoutInflater.Inflate(Resource.Layout.layout_babysitters, null);
            bottomSheetDialog.SetContentView(bottom_sheet_view);
            //bottomSheetDialog.Show();

            babysitter_name = bottom_sheet_view.FindViewById<TextView>(Resource.Id.babysitter_name);
            babysitter_email = bottom_sheet_view.FindViewById<TextView>(Resource.Id.babysitter_email);
            btn_fav = bottom_sheet_view.FindViewById<FloatingActionButton>(Resource.Id.btn_fav);
            btn_fav.Click += delegate
            {
                Toast.MakeText(this, "Added view", ToastLength.Short).Show();
                bottomSheetDialog.Dismiss();
            };

            //Init View
            searchableSpinner = FindViewById<SearchableSpinner>(Resource.Id.searchable_spinner);
            searchableSpinner.OnItemSelectedListener = this;
        }
    }
}