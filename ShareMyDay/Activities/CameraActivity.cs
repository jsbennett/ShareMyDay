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

            Button button = FindViewById<Button>(Resource.Id.submitButton);

            button.Click += (o, e) => {
                if (_previousActivity == "QuickMenu")
                {
                    Toast.MakeText (this, "Back to homepage", ToastLength.Short).Show ();
                    var childMenu = new Intent(this, typeof(MainActivity));
                    StartActivity(childMenu);
                }
            };

            CancelButton cancelButton = new CancelButton(this);
            cancelButton.Get().Click += (o, e) => { cancelButton.Functionality(_previousActivity, this); };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            _camera.DisplayPicture(requestCode, resultCode, this,_camera);
        }

    }
}