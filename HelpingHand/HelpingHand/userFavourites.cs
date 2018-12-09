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

namespace HelpingHand
{
    [Activity(Label = "Favourites", Theme = "@style/AppTheme")]
    public class userFavourites : AppCompatActivity
    {
        protected async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.myFavourites);

            string stringFavourite = this.Intent.GetStringExtra("FAV");

            //BabySitter user = JsonConvert.DeserializeObject<BabySitter>(stringName);
        }
    }
}