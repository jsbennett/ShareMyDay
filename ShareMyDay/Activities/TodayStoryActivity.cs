using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
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
                Button latestStory = FindViewById<Button>(Resource.Id.currentStoryButton); 
                int storyIndex = 0;
                int limit = stories.Count; //this is the number of stories for a day 
                latestStory.Background = Drawable.CreateFromPath(stories[storyIndex].CoverPhoto);
                latestStory.Text = "Story: " + stories[storyIndex].TitleValue;
                latestStory.Click += delegate
                {
                    Intent storyIntent = new Intent(this, typeof(StoryActivity));
                    storyIntent.PutExtra("Story", storyIndex);
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
                        latestStory.Background = Drawable.CreateFromPath(stories[storyIndex].CoverPhoto);
                        latestStory.Text = "Story: " + stories[storyIndex].TitleValue;
                    }
                    
                    if (storyIndex == limit-1)
                    {
                        next.Text = "Close";
                        latestStory.Background = Drawable.CreateFromPath(stories[storyIndex].CoverPhoto);
                        latestStory.Text = "Story: " + stories[storyIndex].TitleValue;

                    } 
                };
            }
            else
            {

            }

          }
    }
}