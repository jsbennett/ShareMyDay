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
using ShareMyDay.Story.StoryFunctions;

namespace ShareMyDay.Activities
{
    [Activity(Label = "EventListActivity")]
    public class EventListActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            StoryGeneration story = new StoryGeneration(db);
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
            buttonLayout.AddView(title);
            foreach (var i in events)
            {
                Button eventButton = new Button(this) {Text = i.Value};

                eventButton.Click += delegate
                {
                    Intent eventClickIntent = new Intent(this, typeof(EditEventsActivity));
                    eventClickIntent.PutExtra("Event", eventButton.Text);
                    StartActivity(eventClickIntent);
                };


                buttonLayout.AddView( eventButton,     
                    ViewGroup.LayoutParams.MatchParent,    
                    300);

                
            }

            innerLayout.AddView( buttonLayout,     
                ViewGroup.LayoutParams.MatchParent,    
                ViewGroup.LayoutParams.WrapContent);

            outerLayout.AddView(innerLayout);

            SetContentView(outerLayout);
            
        }
    }
}