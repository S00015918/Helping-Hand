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
using HelpingHand.Model;
using Newtonsoft.Json;

namespace HelpingHand
{
    [Activity(Label = "View Profile", Theme = "@style/AppTheme")]
    public class viewUser : Activity
    {
        TextView userName = null;
        TextView userAge = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.user_view);
            userName = FindViewById<TextView>(Resource.Id.name);
            userAge = FindViewById<TextView>(Resource.Id.age);

            string stringName = this.Intent.GetStringExtra("KEY");

            BabySitter user = JsonConvert.DeserializeObject<BabySitter>(stringName);

            userName.Text = user.name;
            userAge.Text = Convert.ToString(user.age);
        }
    }
}