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
using ShareMyDay.UIComponents;
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

            //TO-DO: Fix this properly
            string _previousActivity = "QuickMenu";

            SpinnerComponent spinner = new SpinnerComponent (this, Resource.Id.eventSelector, this);
            spinner.Setup();
            
            Button startRecordingButton = FindViewById<Button> (Resource.Id.startRecordingButton);
            Button playRecordingButton = FindViewById<Button> (Resource.Id.playButton);
            VoiceRecording.VoiceRecording voiceRecorder = new VoiceRecording.VoiceRecording();
            startRecordingButton.Click += delegate {
                voiceRecorder.Begin(startRecordingButton);
            };
            playRecordingButton.Click += delegate {
                voiceRecorder.Play(playRecordingButton);
            };

            CancelButton cancelButton = new CancelButton(this);
            cancelButton.Get().Click += (o, e) => { cancelButton.Functionality(_previousActivity, this); };
        }
    }
}