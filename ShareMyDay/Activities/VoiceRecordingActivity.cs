using Android.App;
using Android.OS;
using Android.Widget;
using ShareMyDay.UIComponents;

namespace ShareMyDay.Activities
{
    [Activity(Label = "VoiceRecordingActivity")]
    public class VoiceRecordingActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.VoiceRecView);

            string previousActivity = Intent.GetStringExtra("PreviousActivity");

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
            cancelButton.Get().Click += (o, e) => { cancelButton.Functionality(previousActivity, this); };
        }
    }
}