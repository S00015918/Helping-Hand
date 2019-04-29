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
using Newtonsoft.Json;

namespace HelpingHand
{
    [Activity(Label = "Messages", Theme = "@style/AppTheme")]
    public class MessageActivity : AppCompatActivity, IValueEventListener, IFirebaseLoadDone, IOnItemSelectedListener
    {
        SearchableSpinner searchableSpinner;
        IFirebaseLoadDone firebaseLoadDone;
        List<Parent> list_parents = new List<Parent>();
        List<BabySitter> list_babySitters = new List<BabySitter>();
        FirebaseClient firebaseClient;
        private List<MessageContent> lstMessage = new List<MessageContent>();
        private ListView lstChat;
        private EditText edtChat;
        private FloatingActionButton send;
        FirebaseAuth auth;
        public bool userIsParent = true;

        MessageContent filterMessages = new MessageContent();
        private BabysitterViewAdapter babysitterAdapter;
        private ParentViewAdapter parentAdapter;
        public string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm"), selectedUser, currentUserName;
        BottomSheetDialog bottomSheetDialog;
        TextView user_name, user_email;
        FloatingActionButton btn_fav;
        bool isFirstTime = true;

        public int MyResultCode = 1;
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.messages_main);
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //Init View
            send = FindViewById<FloatingActionButton>(Resource.Id.fab);
            edtChat = FindViewById<EditText>(Resource.Id.input);
            lstChat = FindViewById<ListView>(Resource.Id.list_of_messages);

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

            user_name = bottom_sheet_view.FindViewById<TextView>(Resource.Id.babysitter_name);
            user_email = bottom_sheet_view.FindViewById<TextView>(Resource.Id.babysitter_email);
            btn_fav = bottom_sheet_view.FindViewById<FloatingActionButton>(Resource.Id.btn_fav);

            //searchableSpinner.Adapter = babysitterAdapter;

            string userLogin = auth.CurrentUser.Email;
            var parentEmailList = new List<string>();

            var items = await firebaseClient
                .Child("parent")
                .OnceAsync<Parent>();
            list_parents.Clear();
            foreach (var item in items)
            {
                Parent account = new Parent();
                account.email = item.Object.email;
                string parent_email = account.email;
                parentEmailList.Add(parent_email);
            }

            if (parentEmailList.Contains(userLogin))
            { // Display Parents in spinner 
                DisplaySpinnerUserParents();
            }
            else
            {
                // Display Babysitters in spinner
                DisplaySpinnerUserBabysitters();
            }

            btn_fav.Click += delegate
            {
                ;
                bottomSheetDialog.Dismiss();
            };

            send.Click += delegate { PostMessage(); };
        }

        private async void PostMessage()
        {
            var firebase = new FirebaseClient(FirebaseURL);
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            FirebaseUser user = auth.CurrentUser;
            string currentUserEmail = user.Email;
            string message = edtChat.Text;
            var babysitterEmailList = new List<string>();
            var parentEmailList = new List<string>();

            var parents = await firebaseClient
                   .Child("parent")
                   .OnceAsync<Parent>();
            list_babySitters.Clear();
            babysitterAdapter = null;
            foreach (var item in parents)
            {
                Parent account = new Parent();
                account.email = item.Object.email;
                parentEmailList.Add(account.email);
            }

            if (parentEmailList.Contains(auth.CurrentUser.Email))
            {
                // current user is a parent -
                foreach (var item in parents)
                {
                    Parent account = new Parent();
                    account.email = item.Object.email;
                    if (account.email == auth.CurrentUser.Email)
                    {
                        account.name = item.Object.name;
                        currentUserName = account.name.ToString();
                    }                  
                }               
            }

            var Babysitters = await firebase
                .Child("babysitter")
                .OnceAsync<BabySitter>();
            list_babySitters.Clear();
            parentAdapter = null;
            foreach (var sitters in Babysitters)
            {
                BabySitter _account = new BabySitter();
                _account.email = sitters.Object.email;
                babysitterEmailList.Add(_account.email);
            }

            if (babysitterEmailList.Contains(auth.CurrentUser.Email))
            {
                // current user is a babysitter -
                foreach (var sitters in Babysitters)
                {
                    BabySitter _account = new BabySitter();
                    _account.email = sitters.Object.email;
                    if (_account.email == auth.CurrentUser.Email)
                    {
                        _account.name = sitters.Object.name;
                        currentUserName = _account.name.ToString();
                    }
                }
            }

            MessageContent messages = new MessageContent();
            messages.recieversEmail = user_email.Text;
            messages.sendersEmail = auth.CurrentUser.Email;
            messages.Message = edtChat.Text;
            messages.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            messages.Recipient = user_name.Text;
            messages.Sender = currentUserName;

            var items = await firebaseClient.Child("messages").
                PostAsync<MessageContent>(messages);
            edtChat.Text = "";
        }
        public void OnCancelled(DatabaseError error)
        {
            firebaseLoadDone.OnFirebaseLoadFailed(error.Message);
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            DisplayChatMessage();

            if (userIsParent == true)
            { DisplaySpinnerUserParents(); }
            else { DisplaySpinnerUserBabysitters(); }

        }

        public async void DisplaySpinnerUserParents()
        {
            babysitterAdapter = null;
            parentAdapter = null;
            var babysitters = await firebaseClient
                        .Child("babysitter")
                        .OnceAsync<BabySitter>();
            list_babySitters.Clear();
            foreach (var item in babysitters)
            {
                BabySitter babysitter = new BabySitter();
                babysitter.name = item.Object.name;
                babysitter.email = item.Object.email;
                list_babySitters.Add(babysitter);
            }
            firebaseLoadDone.OnFirebaseLoadBabysitterSuccess(list_babySitters);
        }

        public async void DisplaySpinnerUserBabysitters()
        {
            var parents = await firebaseClient
                   .Child("parent")
                   .OnceAsync<Parent>();
            list_parents.Clear();
            babysitterAdapter = null;
            foreach (var item in parents)
            {
                Parent account = new Parent();
                account.id = item.Key;
                account.name = item.Object.name;
                account.email = item.Object.email;
                list_parents.Add(account);
            }
            firebaseLoadDone.OnFirebaseLoadParentSuccess(list_parents);
            userIsParent = false;
        }

        private async void DisplayChatMessage()
        {
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            FirebaseUser user = auth.CurrentUser;

            lstMessage.Clear();
            var messages = await firebaseClient.Child("messages")
                .OnceAsync<MessageContent>();
            foreach (var item in messages)
            {
                string recieverEmail = item.Object.recieversEmail;
                string senderEmail = item.Object.sendersEmail;
                if (senderEmail == auth.CurrentUser.Email || recieverEmail == auth.CurrentUser.Email)
                {
                    lstMessage.Add(item.Object);
                    MessageAdapter adapter = new MessageAdapter(this, lstMessage);
                    lstChat.Adapter = adapter;
                }
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

        public void OnFirebaseLoadBabysitterSuccess(List<BabySitter> babySitters)
        {
            this.list_babySitters = babySitters;
            //get All babysitters
            List<string> babysitter_list = new List<string>();
            foreach (var babysitter in babySitters)
            {
                babysitter_list.Add(babysitter.name);
            }
            // Create Adapter
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, babysitter_list);
            searchableSpinner.Adapter = adapter;
        }

        public void OnFirebaseLoadParentSuccess(List<Parent> parents)
        {
            this.list_parents = parents;
            //get all parents
            List<string> parent_list = new List<string>();
            foreach (var parent in parents)
            {
                parent_list.Add(parent.name);
                // create adapter
                ArrayAdapter<string> parentAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, parent_list);
                searchableSpinner.Adapter = parentAdapter;
            }
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
                if (userIsParent == true)
                {
                    BabySitter babysitter = list_babySitters[position];
                    user_name.Text = babysitter.name;
                    user_email.Text = babysitter.email;

                    selectedUser = babysitter.name.ToString();
                    bottomSheetDialog.Show();
                    Toast.MakeText(this, "Message Babysitter - " + selectedUser, ToastLength.Long).Show();
                }
                if (userIsParent == false)
                {
                    Parent _parent = list_parents[position];
                    user_name.Text = _parent.name;
                    user_email.Text = _parent.email;

                    selectedUser = _parent.name.ToString();
                    bottomSheetDialog.Show();
                    Toast.MakeText(this, "Message Parent - " + selectedUser, ToastLength.Long).Show();
                }

            }
            else { isFirstTime = false; }

        }

        public void OnNothingSelected(AdapterView parent)
        {
        }
    }
}