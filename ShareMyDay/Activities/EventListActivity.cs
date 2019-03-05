using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ShareMyDay.Story.StoryFunctions;

namespace ShareMyDay.Activities
{
    [Activity(Label = "EventListActivity")]
    public class EventListActivity : Activity
    {
        private Button _close;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Typeface buttonFont = Typeface.CreateFromAsset (Assets, "Kano.otf");
           
            var db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            StoryGeneration story = new StoryGeneration(db,this);
            var events = story.GetEvents();

            LinearLayout outerLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };

            ScrollView innerLayout = new ScrollView(this);
            
            LinearLayout buttonLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };
            
            TextView title = new TextView(this)
            {
                Text = "Today's Events", 
                TextSize = 40, 
                TextAlignment = TextAlignment.Center
                
            };
            title.SetTextColor(Color.Black);
            buttonLayout.AddView(title);
            foreach (var i in events)
            {
                Button eventButton = new Button(this) {Text = i.Value};
                eventButton.SetTypeface(buttonFont,TypefaceStyle.Bold);
                eventButton.Click += delegate
                {
                    Intent eventClickIntent = new Intent(this, typeof(EditEventsActivity));
                    eventClickIntent.PutExtra("Event", eventButton.Text);
                    StartActivity(eventClickIntent);
                };
                
                eventButton.SetBackgroundResource(Resource.Drawable.ButtonGenerator);
                eventButton.SetTextColor(Color.ParseColor("#ffffff"));
                
                eventButton.TextSize = 15;
                

                buttonLayout.AddView( eventButton,     
                    1415,    
                    450);

                
            }

            _close = new Button(this);
            _close.SetBackgroundResource(Resource.Drawable.Back);

            _close.Click += delegate
            {
                _close.SetBackgroundResource(Resource.Drawable.BackClicked);
                Intent back = new Intent(this, typeof(TeacherMainMenuActivity));
                StartActivity(back);
            };

            buttonLayout.AddView(_close, ViewGroup.LayoutParams.MatchParent,    
                450);

            innerLayout.AddView( buttonLayout,     
                ViewGroup.LayoutParams.MatchParent,    
                ViewGroup.LayoutParams.WrapContent);

            outerLayout.AddView(innerLayout);

            SetContentView(outerLayout);
            
        }

        /*
        * Method Name: OnResume
        * Purpose: This is for when the app has loaded
        */
        protected override void OnResume()
        {
            base.OnResume();
           
            _close.SetBackgroundResource(Resource.Drawable.Back);
            
        }
    }
}