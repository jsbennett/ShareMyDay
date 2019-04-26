using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareMyDay.Activities
{
    [Activity(Label = "TodayStoryActivity")]

    /*
     * Class name: TodayStoryActivity
     * Purpose: Used to display the list of stories made during the day 
     */
    public class TodayStoryActivity : Activity
    {
        private bool _closeClicked;

        /*
         * Method name: OnCreate
         * Purpose: To set up the moving between the different stories and displaying their cover photo
         */
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TodayStoryView);
            Typeface buttonFont = Typeface.CreateFromAsset (Assets, "Kano.otf");
           
            var db = new Database.Database(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "ShareMyDay.db3");
            var initialStories = db.GetAllStories();
            List<Database.Models.Story> stories = new List<Database.Models.Story>();

            Button buttonOverlay = FindViewById<Button>(Resource.Id.storyButtonOverlay); 
            buttonOverlay.SetTypeface(buttonFont,TypefaceStyle.Bold);
            foreach (var i in initialStories)
            {
                if (i.DateTime.Day.Equals(DateTime.Now.Day))
                {
                    stories.Add(i);
                }
            }

            bool eventsFromToday = stories.Count != 0;
            Button next = FindViewById<Button>(Resource.Id.changeViewButton);
            
            if (eventsFromToday)
            {
                var options = new BitmapFactory.Options {InJustDecodeBounds = true};

                var sample = 4;
                options.InSampleSize = sample;

                options.InJustDecodeBounds = false;

                ImageView latestStory = FindViewById<ImageView>(Resource.Id.storyButton);
                TextView titleBox = FindViewById<TextView>(Resource.Id.storyTitle);
                int storyIndex = 0;
                
                if (Intent.GetStringExtra("StoryIndex") != null)
                {
                    storyIndex = (int) Convert.ToInt64(Intent.GetStringExtra("StoryIndex"));
                }

                int limit = stories.Count; //this is the number of stories for a day 

                if (stories[storyIndex].DefaultPicture != null)
                {
                    int image = (int) typeof(Resource.Drawable).GetField(stories[storyIndex].DefaultPicture)
                        .GetValue(null);
                    latestStory.SetImageResource(image);
                }
                else
                {
                    using (var image = GetImage(options, stories[storyIndex].CoverPhoto))
                    {
                        latestStory.SetImageBitmap(image);
                    }
                }

                titleBox.Text = "Story: " + stories[storyIndex].TitleValue;
                buttonOverlay.Click += delegate
                {
                    Intent storyIntent = new Intent(this, typeof(StoryActivity));
                    storyIntent.PutExtra("Story", stories[storyIndex].Id.ToString());
                    storyIntent.PutExtra("StoryIndex", storyIndex.ToString()); 
                    StartActivity(storyIntent);
                };


                if (limit.Equals(1)||storyIndex.Equals(limit- 1))
                {
                    next.SetBackgroundResource(Resource.Drawable.SmallClose);
                    next.ContentDescription = "Close";
                }
                else
                {
                    next.SetBackgroundResource(Resource.Drawable.Next);
                }

                next.Click += async delegate
                {
                    
                    if (next.ContentDescription.Equals("Close"))
                    {
                        next.SetBackgroundResource(Resource.Drawable.SmallCloseClicked);
                        Intent exitIntent = new Intent(this, typeof(MainActivity));
                        StartActivity(exitIntent);
                    }
                    else
                    {
                        next.SetBackgroundResource(Resource.Drawable.NextClicked);
                        await Task.Delay(1);

                        next.SetBackgroundResource(Resource.Drawable.Next);
                    }

                    
                    storyIndex++;
                    if (storyIndex < limit - 1)
                    {
                        if (stories[storyIndex].DefaultPicture != null)
                        {
                            int image = (int) typeof(Resource.Drawable).GetField(stories[storyIndex].DefaultPicture)
                                .GetValue(null);
                            latestStory.SetImageResource(image);
                        }
                        else
                        {
                            using (var image = GetImage(options, stories[storyIndex].CoverPhoto))
                            {
                                latestStory.SetImageBitmap(image);
                            }
                        }

                        titleBox.Text = "Story: " + stories[storyIndex].TitleValue;
                    }

                    if (storyIndex == limit - 1)
                    {
                        _closeClicked = true;
                        next.SetBackgroundResource(Resource.Drawable.SmallClose);
                        next.ContentDescription = "Close";
                        if (stories[storyIndex].DefaultPicture != null)
                        {
                            int image = (int) typeof(Resource.Drawable).GetField(stories[storyIndex].DefaultPicture)
                                .GetValue(null);
                            latestStory.SetImageResource(image);
                        }
                        else
                        {
                            using (var image = GetImage(options, stories[storyIndex].CoverPhoto))
                            {
                                latestStory.SetImageBitmap(image);
                            }
                        }

                        titleBox.Text = "Story: " + stories[storyIndex].TitleValue;
                    }
                };
            }
            else
            {
                Intent noFavouriteStories = new Intent(this, typeof(NoStoriesActivity));
                noFavouriteStories.PutExtra("StoryType", "Story");
                StartActivity(noFavouriteStories);
            }
        }


        /*
         * Method name: GetImage
         * Purpose: To get the image using a url path 
         */
        public Bitmap GetImage(BitmapFactory.Options options, string path)
        {
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))

            {
                var bitmap = BitmapFactory.DecodeStream(fs, null, options);

                return bitmap;
            }
        }

        /*
        * Method Name: OnResume
        * Purpose: This is for when the app has loaded
        */
        protected override void OnResume()
        {
            base.OnResume();
            if (_closeClicked)
            {
                Button next = FindViewById<Button>(Resource.Id.changeViewButton);
                next.SetBackgroundResource(Resource.Drawable.SmallClose);
            }
        }
    }
}