using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Xamarin.Essentials;
using Android.Widget;
using System.Collections;
using Firebase.Auth;
using HelpingHand.Model;
using Newtonsoft.Json;
using Android.Views;

namespace HelpingHand
{
    [Activity(Label = "MapWithMarkersActivity")]
    public class MapWithMarkersActivity : AppCompatActivity, IOnMapReadyCallback
    {
        FirebaseAuth auth;
        Button goBack;
        MarkerOptions newMarker = new MarkerOptions();

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Init Firebase
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.map_view);
            goBack = FindViewById<Button>(Resource.Id.btnBack);

            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            GoogleMapOptions mapOptions = new GoogleMapOptions()
                .InvokeMapType(GoogleMap.MapTypeNormal)
                .InvokeZoomControlsEnabled(false)
                .InvokeCompassEnabled(true);

            goBack.Click += GoBack_Click;
        }
        private void GoBack_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(this, typeof(HomeActivity)));
            Finish();
        }

        public async void OnMapReady(GoogleMap map)
        {
            string _locations = this.Intent.GetStringExtra("KEY");

            var getLocation = _locations.Split(',');
            string postcode = getLocation[5];
            string city = getLocation[4];
            string street = getLocation[3];
            string address = street + city + postcode;

            map.MapType = GoogleMap.MapTypeNormal;
            map.UiSettings.ZoomControlsEnabled = true;
            map.UiSettings.CompassEnabled = true;

            try
            {
                var locations = await Geocoding.GetLocationsAsync(address);

                var location = locations?.FirstOrDefault().ToString();
                if (location != null)
                {
                    var end = location.Split(',');
                    string latx = end[0];
                    string lonx = end[1];

                    var markerX = latx.Split(':');
                    string latitude = markerX[1];
                    double lat = Convert.ToDouble(latitude);
                    var markerY = lonx.Split(':');
                    string longitude = markerY[1];
                    double lon = Convert.ToDouble(longitude);

                    var Nmarker = newMarker.SetPosition(new LatLng(lat, lon));
                    Nmarker.SetTitle(street);
                    LatLng zoomTo = new LatLng(lat, lon);

                    map.AddMarker(Nmarker);

                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(zoomTo);
                    builder.Zoom(10);

                    CameraPosition cameraPosition = builder.Build();

                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                    map.MoveCamera(cameraUpdate);
                }
            }
            catch (FeatureNotSupportedException)
            {
                // Feature not supported on device
                Toast.MakeText(this, "Feature not supported on device", ToastLength.Long).Show();
            }
            catch (Exception)
            {
                // Handle exception that may have occurred in geocoding
                Toast.MakeText(this, "Handle exception that may have occurred in geocoding", ToastLength.Long).Show();
            }
        }

        public async void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
        {
            markerClickEventArgs.Handled = true;

            var marker = markerClickEventArgs.Marker;
            if (marker.Id.Equals(newMarker))
            {
                string _locations = this.Intent.GetStringExtra("KEY");

                var getLocation = _locations.Split(',');
                string postcode = getLocation[5];
                string city = getLocation[4];
                string street = getLocation[3];
                string address = street + city + postcode;
                var locations = await Geocoding.GetLocationsAsync(address);

                var location = locations?.FirstOrDefault().ToString();
                if (location != null)
                {
                    var end = location.Split(',');
                    string latx = end[0];
                    string lonx = end[1];

                    var markerX = latx.Split(':');
                    string latitude = markerX[1];
                    double lat = Convert.ToDouble(latitude);
                    var markerY = lonx.Split(':');
                    string longitude = markerY[1];
                    double lon = Convert.ToDouble(longitude);

                    LatLng moveTo = new LatLng(lat, lon);
                    CameraUpdate locationView = CameraUpdateFactory.NewLatLngZoom(
                        moveTo, 15);
                    //mMap.AnimateCamera(locationView);
                    //myMarker = null;
                }
                else
                {
                    Toast.MakeText(this, $"You clicked on Marker ID {marker.Id}", ToastLength.Short).Show();
                }
            }
        }

        public async void GetLocationFromAddress()
        {
            try
            {
                var lat = 47.673988;
                var lon = -122.121513;

                var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon);

                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var geocodeAddress =
                        $"AdminArea:       {placemark.AdminArea}\n" +
                        $"CountryCode:     {placemark.CountryCode}\n" +
                        $"CountryName:     {placemark.CountryName}\n" +
                        $"FeatureName:     {placemark.FeatureName}\n" +
                        $"Locality:        {placemark.Locality}\n" +
                        $"PostalCode:      {placemark.PostalCode}\n" +
                        $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                        $"SubLocality:     {placemark.SubLocality}\n" +
                        $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                        $"Thoroughfare:    {placemark.Thoroughfare}\n";

                    Console.WriteLine(geocodeAddress);
                }
            }
            catch (FeatureNotSupportedException)
            {
                // Feature not supported on device
            }
            catch (Exception)
            {
                // Handle exception that may have occurred in geocoding
            }
        }
    }
}