using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using ShareMyDay.Story.StoryFunctions;
using ShareMyDay.UIComponents;
using Uri = Android.Net.Uri;

namespace ShareMyDay.Activities
{
    [Activity(Label = "StoryActivity")]
    public class StoryActivity : Activity
    {
        private string story; 
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StoryView);

            story = Intent.GetStringExtra("Story");

            ImageView pictureButton = FindViewById<ImageView>(Resource.Id.pictureBox);
            Database.Database db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");

            StoryGeneration storyGeneration = new StoryGeneration(db);
            storyGeneration.Create();

            Button button = FindViewById<Button>(Resource.Id.storyClickButton);
            if (story == "Favourite")
            {
                //get the favourite story stuff 
            }
            else
            {
              //find the story 
              //find how many pictures there are in a story 
              //find how many voice recordings are in a story 
               //Create an array with all the parts of the story 
                //count up the number of parts 
                //display the number of parts- counting down at every click
                //for each  click play next item in the array 
                
                button.Click += delegate
                {
                    AlertBoxComponent alert = new AlertBoxComponent(
                        this);
                    alert.Setup("this","clicked");
                    alert.Show();
                };
                var options = new BitmapFactory.Options {InJustDecodeBounds = true};

                var sample = 4;
                options.InSampleSize = sample;
                        
                options.InJustDecodeBounds = false;

                using (var image = await GetImage(options, "PATH"))
                {
                        pictureButton.SetImageBitmap(image);
                        
                }
               

            }

            Button cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
            cancelButton.Click += delegate
            {
                Intent exitIntent = new Intent(this, typeof(MainActivity));
                StartActivity(exitIntent);
            };
        }

        public async Task<Bitmap> GetImage(BitmapFactory.Options options, string path)
        {
            using (var fs = new System.IO.FileStream("storage/emulated/0/Pictures/ShareMyDayDev/imageeab30d8d-f02d-4a2a-88f8-7f4eac55f139.jpg", System.IO.FileMode.Open))
            
            {
                
                var bitmap = await BitmapFactory.DecodeStreamAsync (fs, null, options);
                if (bitmap != null)
                {
                    Toast.MakeText(this, "Images Loading...", ToastLength.Short).Show();
                }

                return bitmap;
            }
            
        }
    }
}