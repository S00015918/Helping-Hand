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

namespace HelpingHand
{
    [Activity(Label = "Messages", Theme = "@style/AppTheme")]
    public class MessageActivity : AppCompatActivity, IValueEventListener
    {
        private FirebaseClient firebaseClient;
        private List<MessageContent> lstMessage = new List<MessageContent>();
        private ListView lstChat;
        private EditText edtChat;
        private FloatingActionButton send;
        FirebaseAuth auth;

        public int MyResultCode = 1;
        private const string firebase_database_url = "https://th-year-project-37928.firebaseio.com/";

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.messages_main);
            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);

            firebaseClient = new FirebaseClient(firebase_database_url);
            FirebaseDatabase.Instance.GetReference("chats").AddValueEventListener(this);
            send = FindViewById<FloatingActionButton>(Resource.Id.fab);
            edtChat = FindViewById<EditText>(Resource.Id.input);
            lstChat = FindViewById<ListView>(Resource.Id.list_of_messages);

            send.Click += delegate { PostMessage(); };

            //if (FirebaseAuth.Instance.CurrentUser == null)
            //    StartActivityForResult(new Android.Content.Intent(this, typeof(MainActivity)), MyResultCode);
            //else
            //{
             //Toast.MakeText(this, "Welcome" + auth.CurrentUser.Email, ToastLength.Short).Show();
            //    DisplayChatMessage();
            //}
        }

        private async void PostMessage()
        {
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            FirebaseUser user = auth.CurrentUser;
            var items = await firebaseClient.Child("chats").PostAsync(new MessageContent(auth.CurrentUser.Email, edtChat.Text));
            edtChat.Text = "";
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            DisplayChatMessage();
        }

        private async void DisplayChatMessage()
        {
            lstMessage.Clear();
            var items = await firebaseClient.Child("chats")
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
    }
}