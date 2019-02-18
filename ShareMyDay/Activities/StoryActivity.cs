using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Org.Apache.Http.Impl.IO;
using ShareMyDay.UIComponents;
using Uri = Android.Net.Uri;

namespace ShareMyDay.Activities
{
    [Activity(Label = "StoryActivity")]
    public class StoryActivity : Activity
    {
        private string story; 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StoryView);

            story = Intent.GetStringExtra("Story");

            ImageView pictureButton = FindViewById<ImageView>(Resource.Id.pictureBox);

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
                pictureButton.SetImageURI(Uri.Parse("file:///storage/emulated/0/Pictures/ShareMyDayDev/imagecefc15c6-52ec-42b1-90b0-e9f433476330.jpg"));
               

            }

            Button cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
            cancelButton.Click += delegate
            {
                Intent exitIntent = new Intent(this, typeof(MainActivity));
                StartActivity(exitIntent);
            };
        }
    }
}