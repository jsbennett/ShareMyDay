using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
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
            var type = Intent.GetStringExtra("StoryType");
            Typeface buttonFont = Typeface.CreateFromAsset (Assets, "Kano.otf");
            var db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                "ShareMyDay.db3");
            
            if (type == "Favourite")
            {
                SetContentView(Resource.Layout.NoStoriesViewFavourite);
                
                Button closeButton = FindViewById<Button>(Resource.Id.closeButton);
                closeButton.SetTypeface(buttonFont,TypefaceStyle.Bold);
                closeButton.Click += delegate
                {
                    Intent exitIntent = new Intent(this, typeof(MainActivity));
                    StartActivity(exitIntent);
                };
                TextView textBox = FindViewById<TextView>(Resource.Id.storyMessage);

                textBox.Text = "No stories are your favourite yet! Click on the favourite button at the end of a story or one of the buttons below!";
                Button mostPlayed = FindViewById<Button>(Resource.Id.mostPlayedButton);
                mostPlayed.SetTypeface(buttonFont,TypefaceStyle.Bold);
                var mostPlayedStory = db.GetMostPlayed();
                if (mostPlayedStory.TimesPlayed.Equals(0))
                {
                    mostPlayed.Text = "You have not played any stories yet!";
                    mostPlayed.SetBackgroundResource(Resource.Drawable.ButtonDimmedGenerator);
                    mostPlayed.Enabled = false;
                }
                else
                {
                    mostPlayed.Text = "Set " + mostPlayedStory.TitleValue + " as favourite";
                }

                mostPlayed.Click += async delegate
                {
                    AlertDialog.Builder favouriteCheckBox = new AlertDialog.Builder(this);
                    favouriteCheckBox.SetTitle("New Favourite");
                    favouriteCheckBox.SetMessage(
                        "If you make this story your new favourite, the current favourite story will be lost. Do you want to continue?");
                    favouriteCheckBox.SetPositiveButton("Yes", (senderAlert, args) =>
                    {
                        if (db.UpdateFavourite(mostPlayedStory.Id))
                        {
                            AlertDialog.Builder favouriteChanged = new AlertDialog.Builder(this);
                            favouriteChanged.SetTitle("Favourite Story Saved");
                            favouriteChanged.SetMessage("This story is now your favourite story. If you do not click ok, this page will refresh automatically in 5 seconds.");
                            favouriteChanged.SetNeutralButton("Ok", (senderAlerts, argss) => { 
                                Intent storyIntent = new Intent(this, typeof(StoryActivity));
                                storyIntent.PutExtra("Story", "Favourite");
                                StartActivity(storyIntent);});
                            
                            favouriteChanged.Create();
                            favouriteChanged.Show();
                        }

                    });
            
                    favouriteCheckBox.SetNegativeButton("No",
                        (senderAlert, args) =>{});
                    favouriteCheckBox.Create();
                    favouriteCheckBox.Show();
                    await Task.Delay(5000);
                    Intent refresh = new Intent(this, typeof(StoryActivity));
                    refresh.PutExtra("Story", "Favourite");
                    StartActivity(refresh);
                };


                Button lastPlayed = FindViewById<Button>(Resource.Id.lastPlayedButton);
                lastPlayed.SetTypeface(buttonFont,TypefaceStyle.Bold);
                var stories = db.GetAllStories();
                int storyIndex = 0; 
                bool hasPlayed = false;
                for (var index = 0; index < stories.Count; index++)
                {
                    var i = stories[index];
                    if (i.LastPlayed)
                    {
                        lastPlayed.Text = "Set " + i.TitleValue + " as favourite";
                        storyIndex = index;
                        hasPlayed = true;
                        break;
                    }
                }

                if (!hasPlayed)
                {
                    lastPlayed.Text = "You have not played a story!";
                    lastPlayed.SetBackgroundResource(Resource.Drawable.ButtonDimmedGenerator);
                    lastPlayed.Enabled = false; 
                }

                lastPlayed.Click += async delegate {  
                    AlertDialog.Builder favouriteCheckBox = new AlertDialog.Builder(this);
                    favouriteCheckBox.SetTitle("New Favourite");
                    favouriteCheckBox.SetMessage(
                        "If you make this story your new favourite, the current favourite story will be lost. Do you want to continue?");
                    favouriteCheckBox.SetPositiveButton("Yes", (senderAlert, args) =>
                    {
                        if (db.UpdateFavourite(stories[storyIndex].Id))
                        {
                            AlertDialog.Builder favouriteChanged = new AlertDialog.Builder(this);
                            favouriteChanged.SetTitle("Favourite Story Saved");
                            favouriteChanged.SetMessage("This story is now your favourite story. If you do not click ok, this page will refresh automatically in 5 seconds.");
                            favouriteChanged.SetNeutralButton("Ok", (senderAlerts, argss) => { 
                                Intent storyIntent = new Intent(this, typeof(StoryActivity));
                                storyIntent.PutExtra("Story", "Favourite");
                                StartActivity(storyIntent);});
                       
                            favouriteChanged.Create();
                            favouriteChanged.Show();
                        }

                        });
            
                    favouriteCheckBox.SetNegativeButton("No",
                        (senderAlert, args) =>{});
                    favouriteCheckBox.Create();
                    favouriteCheckBox.Show();
                    await Task.Delay(5000);
                    Intent refresh = new Intent(this, typeof(StoryActivity));
                    refresh.PutExtra("Story", "Favourite");
                    StartActivity(refresh);
                };
            }
            else
            {
                SetContentView(Resource.Layout.NoStoriesViewStory);
               Button closeButton = FindViewById<Button>(Resource.Id.closeButton);
                closeButton.SetTypeface(buttonFont,TypefaceStyle.Bold);
                closeButton.Click += delegate
                {
                    Intent exitIntent = new Intent(this, typeof(MainActivity));
                    StartActivity(exitIntent);
                };
                TextView textBox = FindViewById<TextView>(Resource.Id.storyMessage);
                textBox.Text = "No stories have been made yet! Your teacher needs to click the generate story button or wait till home time. Come back soon!";
            }
        }
    }
}