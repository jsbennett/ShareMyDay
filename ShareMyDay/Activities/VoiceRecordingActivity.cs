using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
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
            Typeface buttonFont = Typeface.CreateFromAsset (Assets, "Kano.otf");
            string previousActivity = Intent.GetStringExtra("PreviousActivity");

            SpinnerComponent spinner = new SpinnerComponent (this, Resource.Id.eventSelector, this);
            spinner.Setup();
            
            Button startRecordingButton = FindViewById<Button> (Resource.Id.startRecordingButton);
            startRecordingButton.SetTypeface(buttonFont,TypefaceStyle.Bold);
            startRecordingButton.SetBackgroundColor(Color.ParseColor("#213f5e"));
            startRecordingButton.SetTextColor(Color.White);
            startRecordingButton.SetTextSize(ComplexUnitType.Dip,15);
            
            Button playRecordingButton = FindViewById<Button> (Resource.Id.playButton);
            playRecordingButton.SetTypeface(buttonFont,TypefaceStyle.Bold);
            playRecordingButton.SetBackgroundResource(Resource.Drawable.ButtonGenerator);
            playRecordingButton.SetTextColor(Color.White);
            playRecordingButton.SetTextSize(ComplexUnitType.Dip,15);
            
            Button submitButton = FindViewById<Button>(Resource.Id.submitButton);
            submitButton.SetTypeface(buttonFont,TypefaceStyle.Bold);
            submitButton.SetBackgroundResource(Resource.Drawable.ButtonGenerator);
            submitButton.SetTextColor(Color.White);
            submitButton.SetTextSize(ComplexUnitType.Dip,15);
            playRecordingButton.Enabled = false; 
            playRecordingButton.SetBackgroundResource(Resource.Drawable.ButtonDimmedGenerator);
            playRecordingButton.SetTextColor(Color.ParseColor("#969a90"));
            VoiceRecording.VoiceRecording voiceRecorder = new VoiceRecording.VoiceRecording();
           
            CancelButtonComponent cancelButton = new CancelButtonComponent(this);
            cancelButton.Get().SetTypeface(buttonFont,TypefaceStyle.Bold);
            cancelButton.Get().SetBackgroundResource(Resource.Drawable.ButtonGenerator);
            cancelButton.Get().SetTextColor(Color.White);
            cancelButton.Get().SetTextSize(ComplexUnitType.Dip,15);
            cancelButton.Get().Click += (o, e) => { cancelButton.Functionality(previousActivity, this); };

            startRecordingButton.Click += delegate {
                startRecordingButton.SetTextColor(Color.Black);
               
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

            CheckBox eventComplete = FindViewById<CheckBox>(Resource.Id.eventComplete);
            bool ticked = false;
            eventComplete.Click += delegate
            {
                if (eventComplete.Checked)
                {
                    ticked = true; 
                }
                else
                {
                    ticked = false; 
                }
                
            };
           
            submitButton.Enabled = false; 
            submitButton.SetBackgroundResource(Resource.Drawable.ButtonDimmedGenerator);
            submitButton.SetTextColor(Color.ParseColor("#969a90"));
            bool anotherRecording = false; 
            submitButton.Click += (o, e) => {
                if (anotherRecording == false)
                {
                    //submit to database stuff goes here 
                    
                    bool uploadedSuccessful;
                    if (spinner.GetSelected().Equals("New Event"))
                    {
                        uploadedSuccessful = voiceRecorder.SaveNewEvent(ticked);
                    }
                    else
                    {
                        uploadedSuccessful = voiceRecorder.SaveExistingEvent(spinner, ticked); 
                    }
                    if (uploadedSuccessful)
                    {
                        spinner.Disable();
                        eventComplete.Enabled = false; 
                        submitButton.Text = "Take Another Voice Recording";
                        cancelButton.Get().Text = "Close";
                        startRecordingButton.Enabled = false;
                        startRecordingButton.SetBackgroundColor(Color.ParseColor("#142635"));
                        startRecordingButton.SetTextColor(Color.ParseColor("#969a90"));
                        playRecordingButton.Enabled = false; 
                        playRecordingButton.SetBackgroundResource(Resource.Drawable.ButtonDimmedGenerator);
                        playRecordingButton.SetTextColor(Color.ParseColor("#969a90"));
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

            
        }
    }
}