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

namespace HelpingHand
{
    [Activity(Label = "MapWithMarkersActivity")]
    public class MapWithMarkersActivity : AppCompatActivity, IOnMapReadyCallback
    {
        FirebaseAuth auth;
        Marker myMarker;
        GoogleMap mMap;
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

            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            GoogleMapOptions mapOptions = new GoogleMapOptions()
                .InvokeMapType(GoogleMap.MapTypeNormal)
                .InvokeZoomControlsEnabled(false)
                .InvokeCompassEnabled(true);
            //FragmentTransaction fragTx = FragmentManager.BeginTransaction();
            //mapFragment = MapFragment.NewInstance(mapOptions);
            //fragTx.Add(Resource.Id.map, mapFragment, "map");
            //fragTx.Commit();
            //GetLocationFromAddress();
        }

        public async void OnMapReady(GoogleMap map)
        {
            string _locations = this.Intent.GetStringExtra("KEY");

            var getLocation = _locations.Split(',');
            string address = getLocation[4];
            //Appointment userAddress = JsonConvert.DeserializeObject<Appointment>(_locations);
            //var address = userAddress.Address + userAddress.City + userAddress.Eircode;

            map.MapType = GoogleMap.MapTypeNormal;
            map.UiSettings.ZoomControlsEnabled = true;
            map.UiSettings.CompassEnabled = true;

            newMarker.SetPosition(new LatLng(53.270962, -9.062691));

            //MarkerOptions markerOpt = new MarkerOptions();
            //var marker = markerOpt.SetPosition(new LatLng(54.1553, -8.6065));
            //markerOpt.SetTitle("Sligo Airport");

            try
            {
                //var address = "ITSligo Knocknarea Arena Ballytivnan Sligo";
                //var address = "Microsoft Building 25 Redmond WA USA";
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
                    //Toast.MakeText(this, $"Latitude: {location.Latitude}, Longitude: {location.Longitude}", ToastLength.Long).Show();

                    //MarkerOptions markerOpt = new MarkerOptions();
                    var Nmarker = newMarker.SetPosition(new LatLng(lat, lon));

                    map.AddMarker(Nmarker);
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

            //map.AddMarker(newMarker);
            //map.AddMarker(markerOpt);
        }

        public async void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
        {
            markerClickEventArgs.Handled = true;

            var marker = markerClickEventArgs.Marker;
            if (marker.Id.Equals(newMarker))
            {
                var address = "ITSligo Knocknarea Arena Ballytivnan Sligo";
                //var address = "Microsoft Building 25 Redmond WA USA";
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
                    mMap.AnimateCamera(locationView);
                    myMarker = null;
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