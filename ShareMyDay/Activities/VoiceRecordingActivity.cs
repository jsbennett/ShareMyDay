using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Environment = Android.OS.Environment;

namespace ShareMyDay.Activities
{
    [Activity(Label = "VoiceRecordingActivity")]
    public class VoiceRecordingActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.VoiceRecView);

            // Create your application here
            Spinner spinner = FindViewById<Spinner> (Resource.Id.eventSelector);

            List<string> list = new List<string> {"one", "two", "three", "four"};
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);
            var adapter =  new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleSpinnerItem, list);

            adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            
            Button startRecordingButton = FindViewById<Button> (Resource.Id.startRecordingButton);
            Button playRecordingButton = FindViewById<Button> (Resource.Id.playButton);

            VoiceRecording.VoiceRecording voiceRecorder = new VoiceRecording.VoiceRecording();

            startRecordingButton.Click += delegate {
                voiceRecorder.Begin(startRecordingButton);
            };

            playRecordingButton.Click += delegate {
                voiceRecorder.Play(playRecordingButton);
            };

            Button cancelButton = FindViewById<Button> (Resource.Id.cancelButton);
            cancelButton.Click += delegate {
                Toast.MakeText (this, "Back to homepage", ToastLength.Short).Show ();
                var childMenu = new Intent(this, typeof(MainActivity));
                StartActivity(childMenu);
            };
        }

        private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format ("{0} selected", spinner.GetItemAtPosition (e.Position));
            Toast.MakeText (this, toast, ToastLength.Long).Show ();
        }
    }
}