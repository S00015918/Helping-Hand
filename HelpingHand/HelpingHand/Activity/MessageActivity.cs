using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Xamarin.Database;
using Android.Support.V7.App;
using HelpingHand.Model;
using Android.Support.Design.Widget;
using XamarinFirebaseAuth;
using Firebase.Xamarin.Database.Query;
using Com.Toptoche.Searchablespinnerlibrary;
using HelpingHand.Interface;
using static Android.Widget.AdapterView;
using HelpingHand.Adapter;

namespace HelpingHand
{
    [Activity(Label = "Messages", Theme = "@style/AppTheme")]
    public class MessageActivity : AppCompatActivity, IValueEventListener, IFirebaseLoadDone, IOnItemSelectedListener
    {
        SearchableSpinner searchableSpinner;
        IFirebaseLoadDone firebaseLoadDone;
        List<Model.BabySitter> babySitters = new List<Model.BabySitter>();
        FirebaseClient firebaseClient;
        private List<MessageContent> lstMessage = new List<MessageContent>();
        private ListView lstChat;
        private EditText edtChat;
        private FloatingActionButton send;
        FirebaseAuth auth;

        private BabysitterViewAdapter babysitterAdapter;
        public string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm"), selectedBabysitter;
        BottomSheetDialog bottomSheetDialog;
        TextView babysitter_name, babysitter_email;
        FloatingActionButton btn_fav;
        bool isFirstTime = true;

        public int MyResultCode = 1;
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.messages_main);

            //Init View
            searchableSpinner = FindViewById<SearchableSpinner>(Resource.Id.searchable_spinner);
            searchableSpinner.OnItemSelectedListener = this;

            //Init interface
            firebaseLoadDone = this;

            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //Init DB
            firebaseClient = new FirebaseClient(FirebaseURL);
            FirebaseDatabase.Instance.GetReference("messages").AddValueEventListener(this);

            //Init Dialog
            bottomSheetDialog = new BottomSheetDialog(this);
            View bottom_sheet_view = LayoutInflater.Inflate(Resource.Layout.layout_babysitters, null);
            bottomSheetDialog.SetContentView(bottom_sheet_view);
            //bottomSheetDialog.Show();

            babysitter_name = bottom_sheet_view.FindViewById<TextView>(Resource.Id.babysitter_name);
            babysitter_email = bottom_sheet_view.FindViewById<TextView>(Resource.Id.babysitter_email);
            btn_fav = bottom_sheet_view.FindViewById<FloatingActionButton>(Resource.Id.btn_fav);

            //searchableSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(searchableSpinner_ItemSelected);
            searchableSpinner.Adapter = babysitterAdapter;

            btn_fav.Click += delegate
            {;
                bottomSheetDialog.Dismiss();
            };

            send = FindViewById<FloatingActionButton>(Resource.Id.fab);
            edtChat = FindViewById<EditText>(Resource.Id.input);
            lstChat = FindViewById<ListView>(Resource.Id.list_of_messages);

            send.Click += delegate { PostMessage(); };
        }

        private void searchableSpinner_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            SearchableSpinner spinner = (SearchableSpinner)sender;

            string toast = string.Format("The babysitter is {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();

            var selected = spinner.GetItemAtPosition(e.Position);
        }

        private async void PostMessage()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            FirebaseUser user = auth.CurrentUser;
            string name = user.DisplayName;
            string email = user.Email;
            string message = edtChat.Text;

            MessageContent messages = new MessageContent();
            messages.Recipient = auth.CurrentUser.DisplayName;
            messages.Email = auth.CurrentUser.Email;
            messages.Message = edtChat.Text;
            messages.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            messages.Recipient = selectedBabysitter;

            var items = await firebaseClient.Child("messages").
                PostAsync<MessageContent>(messages);
            edtChat.Text = "";
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            DisplayChatMessage();
            DisplayBabySitters();
        }

        private async void DisplayBabySitters()
        {
            List<BabySitter> local = new List<BabySitter>();
            var firebase = new FirebaseClient(FirebaseURL);
            var babysitters = await firebase
                    .Child("babysitter")
                    .OnceAsync<BabySitter>();
            babySitters.Clear();
            babysitterAdapter = null;
            foreach (var item in babysitters)
            {
                BabySitter babysitter = new BabySitter();
                babysitter.name = item.Object.name;
                babysitter.email = item.Object.email;
                local.Add(babysitter);
            }

            firebaseLoadDone.OnFirebaseLoadSuccess(local);
        }

        private async void DisplayChatMessage()
        {
            lstMessage.Clear();
            var items = await firebaseClient.Child("messages")
                .OnceAsync<MessageContent>();
            foreach (var item in items)
                lstMessage.Add(item.Object);
            MessageAdapter adapter = new MessageAdapter(this, lstMessage);
            lstChat.Adapter = adapter;
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
                StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                Finish();
            }

            return base.OnOptionsItemSelected(item);
        }

        public void OnFirebaseLoadSuccess(List<BabySitter> babySitters)
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

        public void OnFirebaseLoadFailed(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            // Fix Error first selected (Always fire this event when app starts)
            if (!isFirstTime)
            {
                BabySitter babysitter = babySitters[position];
                babysitter_name.Text = babysitter.name;
                babysitter_email.Text = babysitter.email;

                selectedBabysitter = babysitter.name.ToString();
                bottomSheetDialog.Show();
                Toast.MakeText(this, "Message Babysitter - " + selectedBabysitter, ToastLength.Long).Show();

                MessageContent filterMessages = new MessageContent();
                filterMessages.Recipient = selectedBabysitter;
            }
            else { isFirstTime = false; }

        }

        public void OnNothingSelected(AdapterView parent)
        {
        }
    }
}