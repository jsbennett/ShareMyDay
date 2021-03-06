﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using ShareMyDay.UIComponents;

namespace ShareMyDay.Activities
{
    /*
     * Class name: Camera Activity
     * Purpose: To be the activity for displaying the camera and submit page 
     */
    [Activity(Label = "CameraActivity")]
    public class CameraActivity : Activity
    {
        private ImageView _imageViewer;
        private readonly Camera.Camera _camera = new Camera.Camera();
        private string _previousActivity;

        /*
         * Method name: OnCreate
         * Purpose: Used to add functionality to the camera page 
         */
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //To allow the use of picture URL, found from https://stackoverflow.com/questions/39242026/fileuriexposedexception-in-android-n-with-camera/42632654
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());
            
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PictureViewer);
            
            Typeface buttonFont = Typeface.CreateFromAsset(Assets, "Kano.otf");
            _previousActivity = Intent.GetStringExtra("PreviousActivity");

            SpinnerComponent spinner = new SpinnerComponent(this, Resource.Id.eventSelector, this);
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

            CancelButtonComponent cancelButton = new CancelButtonComponent(this);
            cancelButton.Get().SetTypeface(buttonFont, TypefaceStyle.Bold);
            cancelButton.Get().Click += (o, e) => { cancelButton.Functionality(_previousActivity, this); };

            Button submitButton = FindViewById<Button>(Resource.Id.submitButton);
            submitButton.SetTypeface(buttonFont, TypefaceStyle.Bold);
            bool anotherPicture = false;
            
            submitButton.Click += (o, e) =>
            {
                if (anotherPicture == false)
                {
                    bool uploadedSuccessful;
                    if (spinner.GetSelected().Equals("New Event"))
                    {
                        uploadedSuccessful = _camera.SaveNewEvent(ticked);
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
                        cancelButton.Get().Text = "Close";
                        AlertBoxComponent voiceRecording = new AlertBoxComponent(this);
                        voiceRecording.RepeateFunctionSetup<VoiceRecordingActivity>("Take Voice Recording",
                            "Do you want to take a voice recording?", this, this, _previousActivity);
                        voiceRecording.Show();
                        anotherPicture = true;
                    }
                    else
                    {
                        AlertBoxComponent errorUplaodingAlertBox = new AlertBoxComponent(this);
                        errorUplaodingAlertBox.Setup("Cannot Add To Event",
                            "This event has already been made into a story and cannot be updated. Select a different event or make a new event.");
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
        }

        /*
         * Method name:  OnActivityResult
         * Purpose: The action to occur after a picture has been taken i.e. display the picture and submission options 
         */
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            _camera.DisplayPicture(requestCode, resultCode, this, _camera, this);
        }
    }
}