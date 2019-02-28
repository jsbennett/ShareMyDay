using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ShareMyDay.Story.StoryFunctions;

namespace ShareMyDay.Activities
{
    [Activity(Label = "TodayStoryActivity")]
    public class TodayStoryActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TodayStoryView);
           //Get all stories from today 
           
            var db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            var stories = db.GetAllStories(); 
            bool eventsFromToday = stories.Count != 0;
            
            if (eventsFromToday)
            { 
                ImageView latestStory = FindViewById<ImageView>(Resource.Id.storyButton);
                TextView titleBox = FindViewById<TextView>(Resource.Id.storyTitle);
                int storyIndex = 0;
                int limit = stories.Count; //this is the number of stories for a day 
                
                var options = new BitmapFactory.Options {InJustDecodeBounds = true};

                var sample = 4;
                options.InSampleSize = sample;
                        
                options.InJustDecodeBounds = false;
                using (var image = GetImage(options, stories[storyIndex].CoverPhoto))
                {
                    latestStory.SetImageBitmap(image);


                }

                titleBox.Text = "Story: " + stories[storyIndex].TitleValue;
                latestStory.Click += delegate
                {
                    Intent storyIntent = new Intent(this, typeof(StoryActivity));
                    storyIntent.PutExtra("Story", stories[storyIndex].Id.ToString());
                    StartActivity(storyIntent);
                };

                Button next = FindViewById<Button>(Resource.Id.changeViewButton);
                if (limit.Equals(1))
                {
                    next.Text = "Close";
                }

                next.Click += delegate
                {
                    if (next.Text.Equals("Close"))
                    {
                        Intent exitIntent = new Intent(this, typeof(MainActivity));
                        StartActivity(exitIntent);
                    }
                    storyIndex++;
                    if (storyIndex < limit-1)
                    {
                        using (var image = GetImage(options, stories[storyIndex].CoverPhoto))
                        {
                            latestStory.SetImageBitmap(image);


                        }
                        titleBox.Text = "Story: " + stories[storyIndex].TitleValue;
                    }
                    
                    if (storyIndex == limit-1)
                    {
                        next.Text = "Close";
                        using (var image = GetImage(options, stories[storyIndex].CoverPhoto))
                        {
                            latestStory.SetImageBitmap(image);


                        }
                        titleBox.Text = "Story: " + stories[storyIndex].TitleValue;

                    } 
                };
            }
            else
            {

            }

          }

        public Bitmap GetImage(BitmapFactory.Options options, string path)
        {
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            
            {
                
                var bitmap = BitmapFactory.DecodeStream(fs, null, options);
                
                if (bitmap != null)
                {
                    Toast.MakeText(this, "Images Loading...", ToastLength.Short).Show();
                }

                return bitmap;
            }
            
        }
    }
}