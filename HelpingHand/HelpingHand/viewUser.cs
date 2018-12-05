using System;

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using HelpingHand.Model;
using Newtonsoft.Json;
using XamarinFirebaseAuth;

namespace HelpingHand
{
    [Activity(Label = "View User Profile", Theme = "@style/AppTheme")]
    public class viewUser : AppCompatActivity
    {
        TextView userName, userAge, userEmail, userAddress, userCity, userPhone, userEircode;
        ImageView userImage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.user_view);
            //Add Toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            userName = FindViewById<TextView>(Resource.Id.name);
            userAge = FindViewById<TextView>(Resource.Id.age);
            userEmail = FindViewById<TextView>(Resource.Id.email);
            userAddress = FindViewById<TextView>(Resource.Id.address);
            userCity = FindViewById<TextView>(Resource.Id.city);
            userPhone = FindViewById<TextView>(Resource.Id.phone);
            userEircode = FindViewById<TextView>(Resource.Id.eircode);
            userImage = FindViewById<ImageView>(Resource.Id.imgUser);

            string stringName = this.Intent.GetStringExtra("KEY");

            BabySitter user = JsonConvert.DeserializeObject<BabySitter>(stringName);

            userName.Text = user.name;
            userAge.Text = Convert.ToString(user.age);
            userEmail.Text = user.email;
            userAddress.Text = user.address;
            userCity.Text = user.city;
            userPhone.Text = user.phone;
            userEircode.Text = user.eircode;
            //userImage.ImageMatrix = user.ImageUrl;

            var userJson = JsonConvert.SerializeObject(userEmail);
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
                StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                Finish();
            }
            else if (id == Resource.Id.menu_message) // message user
            {
                StartActivity(new Android.Content.Intent(this, typeof(MessageActivity)));
                Finish();
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}