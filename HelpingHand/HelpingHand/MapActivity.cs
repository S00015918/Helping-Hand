using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using HelpingHand.Model;
using Xamarin.Essentials;

namespace HelpingHand
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : AppCompatActivity, IOnMapReadyCallback
    {
        MarkerOptions newMarker = new MarkerOptions();
        private const string FirebaseURL = "https://th-year-project-37928.firebaseio.com/";
        private List<BabySitter> list_babysitters = new List<BabySitter>();
        private List<Parent> list_parents = new List<Parent>();
        FirebaseAuth auth;
        Button goBack;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async void OnMapReady(GoogleMap googleMap)
        {
            var firebase = new FirebaseClient(FirebaseURL);

            googleMap.MapType = GoogleMap.MapTypeNormal;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;

            string userLogin = auth.CurrentUser.Email;

            var items = await firebase
                .Child("parent")
                //.WithAuth(auth.CurrentUser.Uid)
                .OnceAsync<Parent>();
            list_parents.Clear();
            foreach (var item in items)
            {
                Parent account = new Parent();
                account.email = item.Object.email;
                string email = account.email;

                if (userLogin == email) //  parent viewing their map - 
                {
                    var users = await firebase
                        .Child("babysitter")
                        .OnceAsync<BabySitter>();
                    list_babysitters.Clear();
                    foreach (var babysitter in users)
                    {
                        BabySitter getLocation = new BabySitter();
                        getLocation.address = babysitter.Object.address;
                        getLocation.city = babysitter.Object.city;

                        string fullAddress = getLocation.address + ", " + getLocation.city;
                        try
                        {
                            var locations = await Geocoding.GetLocationsAsync(fullAddress);

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
                                Nmarker.SetTitle(fullAddress);

                                googleMap.AddMarker(Nmarker);

                                LatLng zoomTo = new LatLng(53.27194, -9.04889);
                                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                                builder.Target(zoomTo);
                                builder.Zoom(7);

                                CameraPosition cameraPosition = builder.Build();

                                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                                googleMap.MoveCamera(cameraUpdate);
                            }
                        }
                        catch (FeatureNotSupportedException)
                        {
                            // Feature not supported on device
                            Toast.MakeText(this, "Feature not supported on device", ToastLength.Long).Show();
                        }
                    }
                }
                else
                {
                 var sitters = await firebase
                    .Child("babysitter")
                    .OnceAsync<BabySitter>();
                list_babysitters.Clear();
                    foreach (var i in sitters)
                    {
                        BabySitter _account = new BabySitter();
                        _account.email = i.Object.email;

                        if (userLogin == _account.email) // babysitter vewing their map-
                        {
                            var users = await firebase
                                .Child("parent")
                                .OnceAsync<Parent>();
                            list_parents.Clear();

                            foreach (var parent in users)
                            {
                                Parent getLocation = new Parent();
                                getLocation.address = parent.Object.address;
                                getLocation.city = parent.Object.city;

                                string fullAddress = getLocation.address + ", " + getLocation.city;
                                try
                                {
                                    var locations = await Geocoding.GetLocationsAsync(fullAddress);

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
                                        Nmarker.SetTitle(fullAddress);

                                        googleMap.AddMarker(Nmarker);

                                        LatLng zoomTo = new LatLng(53.27194, -9.04889);
                                        CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                                        builder.Target(zoomTo);
                                        builder.Zoom(7);

                                        CameraPosition cameraPosition = builder.Build();

                                        CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                                        googleMap.MoveCamera(cameraUpdate);
                                    }
                                }
                                catch (FeatureNotSupportedException)
                                {
                                    // Feature not supported on device
                                    Toast.MakeText(this, "Feature not supported on device", ToastLength.Long).Show();
                                }
                            }
                        }
                    }
                }
            } 
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.map_view);
            goBack = FindViewById<Button>(Resource.Id.btnBack);

            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            GoogleMapOptions mapOptions = new GoogleMapOptions()
                .InvokeMapType(GoogleMap.MapTypeNormal)
                .InvokeZoomControlsEnabled(true)
                .InvokeCompassEnabled(true);

            goBack.Click += GoBack_Click;
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(this, typeof(HomeActivity)));
            Finish();
        }
    }
}