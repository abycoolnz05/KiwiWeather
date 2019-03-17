using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static kiwiWeather.JSONWeather;

namespace kiwiWeather
{
    [Activity(Label = "kiwiWeather", WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden)]
    public class CoverActivity : Activity, ILocationListener
    {
        private Button btnGPS;
        private string Lat;
        private string Lon;
        LocationManager locMgr;
        string locationProvider;
        private Location CurrentLocation;
        TextView GPSText;
        TextView AllText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.content_main);
            Button btnMainMenu = FindViewById<Button>(Resource.Id.btnMainMenu);
            btnMainMenu.Click += BtnMainMenu_Click1;

            GPSText = FindViewById<TextView>(Resource.Id.txtGPS);
            btnGPS = FindViewById<Button>(Resource.Id.btnGPS);
            btnGPS.Click += BtnGPS_Click;
            InitializeLocationManager();
        }

        private void BtnMainMenu_Click1(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity)); // Start Activity 
        }

        async void BtnGPS_Click(object sender, EventArgs e)
        {
            if (CurrentLocation == null)  //GPS button click 
            {
                AllText.Text = "Can't determine the current address. Try again in a few minutes.";
                return;
            }

            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);
        }

        private void DisplayAddress(Address address)  //Display address function
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++) //for loop to append line
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                AllText.Text = deviceAddress.ToString();
            }
            else
            {
                AllText.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }

        async private Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(CurrentLocation.Latitude, CurrentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void ILocationListener.OnLocationChanged(Location location)   //GPS location
        {
            CurrentLocation = location;
            UpdateGPSLocation();
        }

        private void UpdateGPSLocation()  //Update gps location function
        {
            Lat = CurrentLocation.Latitude.ToString();
            Lon = CurrentLocation.Longitude.ToString();
            Toast.MakeText(this, "Lat " + Lat + "Lon " + Lon, ToastLength.Long).Show();
            GPSText.Text = "Lat " + Lat + "Lon " + Lon; // just so we know it exists
        }

        void ILocationListener.OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        void ILocationListener.OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "Provider Enabled", ToastLength.Short).Show();
        }

        void ILocationListener.OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }

        void InitializeLocationManager()
        {
            locMgr = (LocationManager)GetSystemService(LocationService);
            // Criteria for the best location provider to use GPS, WiFi and Cell Towers
            Criteria criteriaForLocationService = new Criteria
            {
                //A constant indicating an approximate accuracy
                Accuracy = Accuracy.Coarse, 
                PowerRequirement = Power.Medium
            };
            //Gets the best providor
            locationProvider = locMgr.GetBestProvider(criteriaForLocationService, true);

            Toast.MakeText(this, "Using " + locationProvider, ToastLength.Short).Show();
        }
        protected override void OnPause()
        {
            base.OnPause();
            locMgr.RemoveUpdates(this);
        }
        public void DownLloadGPSTemp()  // Gps temp down via the website using APi key with location
        {
            try
            {
                string URL = "http://api.worldweatheronline.com/premium/v1/weather.ashx?q=" + CurrentLocation.Latitude + "," + CurrentLocation.Longitude + "&num_of_days=1&format=json&key=951ca4d96d1b4314b0f100139190403";
                

                Uri webaddress = new Uri(URL); //Get the URL change it to a Uniform Resource Identifier
                var webclient = new WebClient(); //Make a webclient to dl stuff
                webclient.DownloadStringAsync(webaddress); //dl the website
                webclient.DownloadStringCompleted += Webclient_DownloadStringCompleted; ;
                //Connect a method to the run when the DL is finished,
            }
            catch (Exception e)
            {
                //Run an error message if it doesn’t work
                var toast = string.Format("DL not working? " + e.Message);
                Toast.MakeText(this, toast, ToastLength.Long).Show();
            }
        }

        private void Webclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void webclient_DownloadJSONCompleted(object Sender, DownloadStringCompletedEventArgs e)
        {
            string TempDataJSON = e.Result; 

            var root = JsonConvert.DeserializeObject<RootObject>(TempDataJSON); // Deserialize json object 

            CurrentCondition currentCondition = root.data.current_condition[0];
            Weather weather = root.data.weather[0];
            //Display current info in the text filed.   
            AllText.Text = "Current Temp = " + currentCondition.temp_C + "Min " + weather.tempMinC + " Max " + weather.tempMaxC + " Wind " + currentCondition.windspeedKmph;

            AllText.SetText(Html.FromHtml("<bold>Current Temp = " + currentCondition.temp_C + "</bold>"), TextView.BufferType.Normal);
        }
    }
}