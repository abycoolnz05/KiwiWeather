using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace kiwiWeather
{
    [Activity(Label = "kiwiWeather", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);


            Button btnStart = FindViewById<Button>(Resource.Id.btnStart);  //1. Start New List Button
            btnStart.Click += BtnStart_Click;

            Button btnQuit = FindViewById<Button>(Resource.Id.btnQuit); //2.  Quit Button
            btnQuit.Click += BtnQuit_Click;
        }

        private void BtnStart_Click(object sender, EventArgs e) // Start Activity Cover
        {
            StartActivity(typeof(CoverActivity));
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

	}
}

