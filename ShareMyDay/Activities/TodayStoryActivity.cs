using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ShareMyDay.Activities
{
    [Activity(Label = "TodayStoryActivity")]
    public class TodayStoryActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TodayStoryView);
            //check for events from today from the database
            //Get all events from the day which have a story i.e a picture or voice recording 

            bool eventsFromToday = true;
            if (eventsFromToday)
            {
                Button latestStory = FindViewById<Button>(Resource.Id.currentStoryButton); 
                //for the first one in array of stories 
                int i = 1;
                int limit = 4; //this is the number of stories for a day 
                latestStory.Text = "Story: " + i;
                latestStory.Click += delegate
                {
                    Intent storyIntent = new Intent(this, typeof(StoryActivity));
                    storyIntent.PutExtra("Story", i);
                    StartActivity(storyIntent);
                };

                Button next = FindViewById<Button>(Resource.Id.changeViewButton);
                bool limitReached = false; 
                
                next.Click += delegate
                {
                    i++;
                    latestStory.Text = "Story: " + i;
                    if (i == limit)
                    {
                        next.Text = "Close";

                    }else if (i == limit + 1)
                    {
                        i--;
                        latestStory.Text = "Story: " + i;
                        latestStory.Enabled = false;
                        Intent exitIntent = new Intent(this, typeof(MainActivity));
                        StartActivity(exitIntent);
                    }
                  
                };

                

            }
            else
            {

            }

          }
    }
}