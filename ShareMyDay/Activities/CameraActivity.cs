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

namespace ShareMyDay.Activities
{
    [Activity(Label = "CameraActivity")]
    public class CameraActivity : Activity
    {
        private ImageView _imageViewer;
        private readonly Camera.Camera _camera = new Camera.Camera();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PictureViewer);

            _imageViewer = _camera.GetImageViewer(Resource.Id.imageView, this);
            _camera.Start(_imageViewer, this);

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            _camera.DisplayPicture(requestCode, resultCode, this,_camera);
        }
    }
}