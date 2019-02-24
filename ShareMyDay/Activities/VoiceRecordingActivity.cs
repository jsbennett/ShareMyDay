using Android.App;
using Android.Content;
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
            Button submitButton = FindViewById<Button>(Resource.Id.submitButton);
            playRecordingButton.Enabled = false; 
            VoiceRecording.VoiceRecording voiceRecorder = new VoiceRecording.VoiceRecording();
            startRecordingButton.Click += delegate {
                if (startRecordingButton.Text.Equals("Redo voice recording"))
                {
                    voiceRecorder.Begin(startRecordingButton, submitButton, playRecordingButton, true);
                }
                else
                {
                    voiceRecorder.Begin(startRecordingButton, submitButton, playRecordingButton, false);
                }
               
                
            };
            playRecordingButton.Click += delegate {
                voiceRecorder.Play();
                startRecordingButton.Text = "Redo voice recording";
            };

           
            submitButton.Enabled = false; 
            bool anotherRecording = false; 
            submitButton.Click += (o, e) => {
                if (anotherRecording == false)
                {
                    //submit to database stuff goes here 
                    
                    bool uploadedSuccessful;
                    if (spinner.GetSelected().Equals("New Event"))
                    {
                        uploadedSuccessful =voiceRecorder.SaveNewEvent();
                    }
                    else
                    {
                        uploadedSuccessful = voiceRecorder.SaveExistingEvent(spinner); 
                    }
                    if (uploadedSuccessful)
                    {
                        submitButton.Text = "Take Another Voice Recording";
                        startRecordingButton.Enabled = false;
                        playRecordingButton.Enabled = false; 
                        AlertBoxComponent voiceRecording = new AlertBoxComponent(this);
                        voiceRecording.RepeateFunctionSetup<CameraActivity>("Take Picture",
                            "Do you want to take a picture?", this, this, previousActivity);
                        voiceRecording.Show();
                        anotherRecording = true;
                    }
                    else
                    {
                        AlertBoxComponent errorUplaodingAlertBox = new AlertBoxComponent(this);
                        errorUplaodingAlertBox.Setup("Error Uploading",
                            "Please click submit again.");
                        errorUplaodingAlertBox.Show();
                    }
                }
                else
                {
                    Intent repeatedActivity = new Intent(this, typeof(VoiceRecordingActivity));
                    repeatedActivity.PutExtra("PreviousActivity", previousActivity);
                    StartActivity(repeatedActivity);
                }
            };

            CancelButtonComponent cancelButton = new CancelButtonComponent(this);
            cancelButton.Get().Click += (o, e) => { cancelButton.Functionality(previousActivity, this); };
        }
    }
}