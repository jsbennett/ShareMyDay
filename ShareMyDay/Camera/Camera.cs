using Android.App;
using Android.Content;
using Android.Provider;
using Android.Widget;
using Java.IO;
using System;
using Console = System.Console;
using Uri = Android.Net.Uri;

namespace ShareMyDay.Camera
{
    /*
     * Class Name: Camera
     * Purpose: To provide camera functionality
     * Code adapted from 
     */
    class Camera
    {
        private readonly int _photoCode;
        private File _image;
        private ImageView _imageViewer;

        public Camera()
        {
            _photoCode = 123; 
        }

        public int GetPhotoCode()
        {
            return _photoCode;
        }

        public ImageView GetImageViewer(int id, Activity activity)
        {
           return activity.FindViewById<ImageView>(id);

        }

        public File GetFileLocation()
        {
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
        }

        public File CreateFolder(File location)
        {
            location = new File(location, "ShareMyDayDev");
            if (!location.Exists())
            {
                location.Mkdirs();
            }

            return location; 
        }

        public File CreateImageFile(File location)
        {
            return new File(location, $"image{Guid.NewGuid()}.jpg");
        }

        public Uri GetUri(File image)
        {
            return Uri.FromFile(image); 
        }

        public void StartActivity(Uri imageUri, Activity activity)
        {
            var intent = new Intent(MediaStore.ActionImageCapture); 
            intent.PutExtra(MediaStore.ExtraOutput, imageUri); 
            activity.StartActivityForResult(intent, _photoCode); 
        }

        public File GetImage()
        {
            return _image;
        }

        public void Start(ImageView imageViewer, Activity activity)
        {
            _imageViewer = imageViewer;
            var location = GetFileLocation();
            location = CreateFolder(location);
            _image = CreateImageFile(location);
            var imageUri = GetUri(_image); 
            StartActivity(imageUri, activity);
        }

        public void DisplayPicture(int requestCode, Result resultCode, Activity activity, Camera camera)
        {
            if (requestCode == _photoCode) {
                
                if (resultCode == Result.Ok) {
                    Console.WriteLine(Uri.Parse (camera.GetImage().AbsolutePath));
                    _imageViewer.SetImageURI (Uri.Parse (camera.GetImage().AbsolutePath));
                } else {
                    Toast.MakeText (activity, "Canceled photo.", ToastLength.Short).Show ();
                }
            }
        }
    }
}