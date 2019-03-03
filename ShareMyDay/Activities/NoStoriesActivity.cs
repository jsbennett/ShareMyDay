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
    [Activity(Label = "NoStoriesActivity")]
    public class NoStoriesActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.NoStoriesView);

            var type = Intent.GetStringExtra("StoryType");
            Button closeButton = FindViewById<Button>(Resource.Id.closeButton);
            closeButton.Click += delegate
            {
                Intent exitIntent = new Intent(this, typeof(MainActivity));
                StartActivity(exitIntent);
            };
            TextView textBox = FindViewById<TextView>(Resource.Id.storyMessage);
            
            if (type == "Favourite")
            {
                textBox.Text = "No stories are your favourite yet!";
            }
            else
            {
                textBox.Text = "No stories have been made yet! Come back soon!";
            }
        }
    }
}