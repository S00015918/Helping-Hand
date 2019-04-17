
using Android.App;
using Android.Support.V7.App;
using Firebase.Database;
using HelpingHand.Interface;
using static Android.Widget.AdapterView;
using Android.Runtime;
using Android.Widget;
using Com.Toptoche.Searchablespinnerlibrary;
using System.Collections.Generic;
using System.Linq;
using Android.Support.Design.Widget;
using Android.Views;
using Square.PicassoLib;
using Android.OS;

namespace HelpingHand
{
    [Activity(Label = "Messages")]
    public class Activity_messages : AppCompatActivity, IValueEventListener, IOnItemSelectedListener
    {
        SearchableSpinner searchableSpinner;
        DatabaseReference movieRef;
        IFirebaseLoadDone firebaseLoadDone;
        List<Model.BabySitter> list_babysitters= new List<Model.BabySitter>();
        BottomSheetDialog bottomSheetDialog;

        //Dialog view
        TextView baby_name, baby_email;
        FloatingActionButton btn_fav;
        bool isFirstTime = true;

        public void OnCancelled(DatabaseError error)
        {
            firebaseLoadDone.OnFirebaseLoadFailed(error.Message);
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            List<Model.BabySitter> local = new List<Model.BabySitter>();
            foreach (DataSnapshot movieSnapShot in snapshot.Children.ToEnumerable())
            {
                Model.BabySitter baby = new Model.BabySitter();
                baby.name = movieSnapShot.Child("name").GetValue(true).ToString();
                baby.email = movieSnapShot.Child("email").GetValue(true).ToString();

                local.Add(baby);
            }
            //firebaseLoadDone.OnFirebaseLoadSuccess(local);
        }

        public void OnFirebaseLoadFailed(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        public void OnFirebaseLoadSuccess(List<Model.BabySitter> sitters)
        {
            this.list_babysitters = sitters;
            //get All name
            List<string> _list = new List<string>();
            foreach (var babysitters in sitters)
            {
                _list.Add(babysitters.name);
            }
            // Create Adapter
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _list);
            searchableSpinner.Adapter = adapter;
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            // Fix Error first selected (Always fire this event when app starts)
            if (!isFirstTime)
            {
                Model.BabySitter baby = list_babysitters[position];
                baby_email.Text = baby.email;
                baby_name.Text = baby.name;

                bottomSheetDialog.Show();
            }
            else { isFirstTime = false; }
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            // Fix Error first selected (Always fire this event when app starts)
            if (!isFirstTime)
            {
                Model.BabySitter baby = list_babysitters[position];
                baby_email.Text = baby.email;
                baby_name.Text = baby.name;

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
            //firebaseLoadDone = this;

            //Init Dialog
            bottomSheetDialog = new BottomSheetDialog(this);
            View bottom_sheet_view = LayoutInflater.Inflate(Resource.Layout.layout_babysitters, null);
            bottomSheetDialog.SetContentView(bottom_sheet_view);
            //bottomSheetDialog.Show();

            baby_name = bottom_sheet_view.FindViewById<TextView>(Resource.Id.babysitter_name);
            baby_email = bottom_sheet_view.FindViewById<TextView>(Resource.Id.babysitter_email);
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