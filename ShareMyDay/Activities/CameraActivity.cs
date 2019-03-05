
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using ShareMyDay.UIComponents;

namespace ShareMyDay.Activities
{
    [Activity(Label = "CameraActivity")]
    public class CameraActivity : Activity
    {
        private ImageView _imageViewer;
        private readonly Camera.Camera _camera = new Camera.Camera();
        private string _previousActivity;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            //StrictMode.SetVmPolicy(builder.Build());
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PictureViewer);
            
            _previousActivity = Intent.GetStringExtra("PreviousActivity");
            
            SpinnerComponent spinner = new SpinnerComponent (this, Resource.Id.eventSelector, this);
            spinner.Setup();

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
            _imageViewer = _camera.GetImageViewer(Resource.Id.imageView, this);
            _camera.Start(_imageViewer, this, _previousActivity);

            Button submitButton = FindViewById<Button>(Resource.Id.submitButton); 

            bool anotherPicture = false; 
            submitButton.Click += (o, e) => {
                if (anotherPicture == false)
                {
                   
                    bool uploadedSuccessful;
                    if (spinner.GetSelected().Equals("New Event"))
                    {
                        uploadedSuccessful =_camera.SaveNewEvent(ticked);
                    }
                    else
                    {
                        uploadedSuccessful = _camera.SaveExistingEvent(spinner, ticked); 
                    }

             
                    if (uploadedSuccessful)
                    {
                        spinner.Disable();
                        eventComplete.Enabled = false; 
                        submitButton.Text = "Take Another Picture";
                        AlertBoxComponent voiceRecording = new AlertBoxComponent(this);
                        voiceRecording.RepeateFunctionSetup<VoiceRecordingActivity>("Take Voice Recording",
                            "Do you want to take a voice recording?", this, this, _previousActivity);
                        voiceRecording.Show();
                        anotherPicture = true;
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
                    Intent repeatedActivity = new Intent(this, typeof(CameraActivity));
                    repeatedActivity.PutExtra("PreviousActivity", _previousActivity);
                    StartActivity(repeatedActivity);
                }
            };

            CancelButtonComponent cancelButton = new CancelButtonComponent(this);
            cancelButton.Get().Click += (o, e) => { cancelButton.Functionality(_previousActivity, this); };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            _camera.DisplayPicture(requestCode, resultCode, this,_camera, this);
        }

    }
}