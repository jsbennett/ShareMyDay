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
        private string _previousActivity;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PictureViewer);
            _previousActivity = Intent.GetStringExtra("PreviousActivity");
            
            //TO DO: make a spinner class + methods 
            Spinner spinner = FindViewById<Spinner> (Resource.Id.eventSelector);

            List<string> list = new List<string> {"one", "two", "three", "four"};
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);
            var adapter =  new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleSpinnerItem, list);

            adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            _imageViewer = _camera.GetImageViewer(Resource.Id.imageView, this);
            _camera.Start(_imageViewer, this);

            //TO DO: make a button class + methods 
            Button button = FindViewById<Button>(Resource.Id.submitButton);

            button.Click += (o, e) => {
                if (_previousActivity == "QuickMenu")
                {
                    Toast.MakeText (this, "Back to homepage", ToastLength.Short).Show ();
                    var childMenu = new Intent(this, typeof(MainActivity));
                    StartActivity(childMenu);
                }
            };

            Button cancelButton = FindViewById<Button>(Resource.Id.cancelButton);

            cancelButton.Click += (o, e) => {
                if (_previousActivity == "QuickMenu")
                {
                    Toast.MakeText (this, "Back to homepage", ToastLength.Short).Show ();
                    var childMenu = new Intent(this, typeof(MainActivity));
                    StartActivity(childMenu);
                }
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            _camera.DisplayPicture(requestCode, resultCode, this,_camera);

        }

        private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format ("{0} selected", spinner.GetItemAtPosition (e.Position));
            Toast.MakeText (this, toast, ToastLength.Long).Show ();
        }
    }
}