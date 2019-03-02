using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace HelpingHand
{
    [Activity(Label = "MapWithMarkersActivity")]
    public class MapWithMarkersActivity : AppCompatActivity, IOnMapReadyCallback
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.map_view);

            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            GoogleMapOptions mapOptions = new GoogleMapOptions()
                .InvokeMapType(GoogleMap.MapTypeSatellite)
                .InvokeZoomControlsEnabled(false)
                .InvokeCompassEnabled(true);
        }

        public void OnMapReady(GoogleMap map)
        {
            map.MapType = GoogleMap.MapTypeNormal;
        }
    }
}