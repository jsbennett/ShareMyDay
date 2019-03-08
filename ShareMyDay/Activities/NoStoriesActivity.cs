using Android.App;
using Android.Content;
using Android.OS;
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
                textBox.Text = "No stories are your favourite yet! Click on a story, play it to the end and then click the Favourite button for the story to be your favourite story.";
            }
            else
            {
                textBox.Text = "No stories have been made yet! Your teacher needs to click the generate story button or wait till home time. Come back soon!";
            }
        }
    }
}