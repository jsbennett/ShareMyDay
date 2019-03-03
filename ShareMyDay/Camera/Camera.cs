using Android.App;
using Android.Content;
using Android.Provider;
using Android.Widget;
using Java.IO;
using System;
using ShareMyDay.Activities;
using ShareMyDay.Database.Models;
using ShareMyDay.UIComponents;
using Console = System.Console;
using Uri = Android.Net.Uri;

namespace ShareMyDay.Camera
{
    /*
     * Class Name: Camera
     * Purpose: To provide camera functionality
     * NFC Code adapted from Xamarin Mobile Development for Android Cookbook by Matthew Leibowitz page 272 - 275
     */
    class Camera
    {
        private readonly int _photoCode;
        private File _image;
        private ImageView _imageViewer;
        private string _url;
        private string _previousActivity; 

        /*
         * Constructor 
         */
        public Camera()
        {
            _photoCode = 123; 
        }

        /*
         * Method Name: GetImageViewer
         * Purpose: To find the image view element on the UI
         */
        public ImageView GetImageViewer(int id, Activity activity)
        {
           return activity.FindViewById<ImageView>(id);

        }

        /*
         * Method Name: GetFileLocation
         * Purpose: To find the place to store the pictures on the phone 
         */
        public File GetFileLocation()
        {
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
        }

        /*
         * Method Name: CreateFolder
         * Purpose: To find the location of the photo storage folder. If there is not one made, then make the folder. 
         */
        public File CreateFolder(File location)
        {
            location = new File(location, "ShareMyDayDev");
            if (!location.Exists())
            {
                location.Mkdirs();
            }

            return location; 
        }

        /*
         * Method name: CreateImageFile
         * Purpose: To create the file for the image to be store in with a unique name
         */
        public File CreateImageFile(File location)
        {
            return new File(location, $"image{Guid.NewGuid()}.jpg");
        }

        /*
         * Method Name: GetUri
         * Purpose: To find the URI for the image 
         */
        public Uri GetUri(File image)
        {
            return Uri.FromFile(image); 
        }

        /*
         * Method name: StartActivity
         * Purpose: To begin the camera activity 
         */
        public void StartActivity(Uri imageUri, Activity activity)
        {
            var intent = new Intent(MediaStore.ActionImageCapture); 
            intent.PutExtra(MediaStore.ExtraOutput, imageUri); 
            activity.StartActivityForResult(intent, _photoCode); 
        }

        /*
         * Method name: GetImage
         * Purpose: To return the path to the image 
         */
        public File GetImage()
        {
            return _image;
        }

        /*
         * Method name: Start
         * Purpose: To setup and start the camera activity 
         */
        public void Start(ImageView imageViewer, Activity activity, string previousActivity)
        {
            _imageViewer = imageViewer;
            var location = GetFileLocation();
            location = CreateFolder(location);
            _image = CreateImageFile(location);
            var imageUri = GetUri(_image); 
            StartActivity(imageUri, activity);
            _previousActivity = previousActivity;
        }

        /*
         * Method name: DisplayPicture
         * Purpose: To get and display the image 
         */
        public void DisplayPicture(int requestCode, Result resultCode, Activity activity, Camera camera, Context context)
        {
            if (requestCode == _photoCode) {
                
                if (resultCode == Result.Ok) {
                    Console.WriteLine(Uri.Parse (camera.GetImage().AbsolutePath));
                    _imageViewer.SetImageURI (Uri.Parse (camera.GetImage().AbsolutePath));
                    _url = camera.GetImage().AbsolutePath;
                } else {
                    if (_previousActivity == "QuickMenu")
                    {
                        Toast.MakeText (context, "Back to homepage", ToastLength.Short).Show ();
                        var childMenu = new Intent(context, typeof(MainActivity));
                        context.StartActivity(childMenu);
                    }
                    else
                    {
                        Toast.MakeText (context, "Back to main menu", ToastLength.Short).Show ();
                        var mainMenu = new Intent(context, typeof(TeacherMainMenuActivity));
                        context.StartActivity(mainMenu);
                    }
                }
            }
        }

        public string GetImageURL()
        {
            return _url;

        }
        public bool SaveNewEvent(bool ticked)
        {
            Database.Database db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            StoryEvent storyEvent = new StoryEvent
            {
                Value = DateTime.Now.ToLongTimeString() +  "-" + "Picture Taken",
                DateTime = DateTime.Now,
                Finished = ticked
            };

            var picture = new Picture {
                EventId = storyEvent.Id,
                Path = GetImageURL()

            };
                 
            return db.InsertEvent(true, storyEvent, null, picture, null) != 0;

        }

        public bool SaveExistingEvent(SpinnerComponent spinner, bool ticked)
        {
            
            Database.Database db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            var storyEvent = db.FindEventByValue(spinner.GetSelected());
            storyEvent.Finished = ticked;

            var picture = new Picture {
                EventId = storyEvent.Id,
                Path = GetImageURL(),


            };

            return db.InsertEvent(false, storyEvent, null, picture, null) != 0;
        }
    }
}