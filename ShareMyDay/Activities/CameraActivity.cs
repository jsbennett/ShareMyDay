using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
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
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PictureViewer);
            
            _previousActivity = Intent.GetStringExtra("PreviousActivity");
            
            SpinnerComponent spinner = new SpinnerComponent (this, Resource.Id.eventSelector, this);
            spinner.Setup();
     
            _imageViewer = _camera.GetImageViewer(Resource.Id.imageView, this);
            _camera.Start(_imageViewer, this);

            Button submitButton = FindViewById<Button>(Resource.Id.submitButton);

            bool anotherPicture = false; 
            submitButton.Click += (o, e) => {
                if (anotherPicture == false)
                {
                    //submit to database stuff goes here 
                    bool uploadedSuccessful = true;
                    if (uploadedSuccessful)
                    {
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
            _camera.DisplayPicture(requestCode, resultCode, this,_camera);
        }

    }
}