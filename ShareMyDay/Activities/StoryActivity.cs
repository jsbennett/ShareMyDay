using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using ShareMyDay.Database.Models;
using ShareMyDay.Story.StoryFunctions;
using ShareMyDay.UIComponents;
using Picture = ShareMyDay.Database.Models.Picture;
using Uri = Android.Net.Uri;

namespace ShareMyDay.Activities
{
    [Activity(Label = "StoryActivity")]
    public class StoryActivity : Activity
    {
        private string storyId; 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StoryView);

            storyId = Intent.GetStringExtra("Story");

            ImageView pictureButton = FindViewById<ImageView>(Resource.Id.pictureBox);
            Database.Database db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");

            StoryGeneration storyGeneration = new StoryGeneration(db);
            storyGeneration.Create();
            var options = new BitmapFactory.Options {InJustDecodeBounds = true};

            var sample = 4;
            options.InSampleSize = sample;            
            options.InJustDecodeBounds = false;

            if (storyId == "Favourite")
            {
                //get the favourite story stuff 
            }
            else
            {
                var story = db.FindStoryById(storyId);

                using (var image = GetImage(options, story.CoverPhoto))
                {
                    pictureButton.SetImageBitmap(image);
                        
                }

                TextView title = FindViewById<TextView>(Resource.Id.titleBox);
                title.Text = story.TitleValue;

                List<StoryEvent> storyEvents = db.FindEventsFromStory(storyId);
                List<Picture> pictures = new List<Picture>();
                List<Database.Models.VoiceRecording> voiceRecordings = new List<Database.Models.VoiceRecording>();

                if (story.Extra.Equals(false) && story.TextToSpeech.Equals(false))
                {
                    foreach (var i in storyEvents)
                    {
                        if (i.Pictures != null && i.Pictures.Count != 0)
                        {
                            foreach (var picture in i.Pictures)
                            {
                                pictures.Add(picture);
                            }
                        }

                        if (i.VoiceRecordings != null && i.VoiceRecordings.Count != 0)
                        {
                            foreach (var recording in i.VoiceRecordings)
                            {
                                voiceRecordings.Add(recording);
                            }
                        }
                    }
                }

                bool done = false; 
                if (story.TextToSpeech)
                {
                    
                    foreach (var i in story.Events)
                    {
                        if (i.Pictures != null && i.Pictures.Count != 0)
                        {
                            foreach (var picture in i.Pictures)
                            {
                                pictures.Add(picture);
                            }
                        }

                        if (i.VoiceRecordings != null && i.VoiceRecordings.Count != 0)
                        {
                            foreach (var recording in i.VoiceRecordings)
                            {
                                voiceRecordings.Add(recording);
                            }
                        }
                        else
                        {
                            //get card and make into sentence 
                        }

                        done = true;
                    }

                }

                if (story.Extra && !done)
                {
                    foreach (var i in story.Events)
                    {
                        if (i.Pictures != null && i.Pictures.Count != 0)
                        {
                            foreach (var picture in i.Pictures)
                            {
                                pictures.Add(picture);
                            }
                        }

                        if (i.VoiceRecordings != null && i.VoiceRecordings.Count != 0)
                        {
                            foreach (var recording in i.VoiceRecordings)
                            {
                                voiceRecordings.Add(recording);
                            }
                        }
                        else
                        {
                            //get card and make into sentence 
                        }
                    }
                }

                int pictureCount = pictures.Count;
                int recordingCount = voiceRecordings.Count;
                int difference;
                int totalSteps;
                if (pictureCount > recordingCount)
                {
                    difference = pictureCount - recordingCount;
                    totalSteps = pictureCount + difference;
                }else if (recordingCount > pictureCount)
                {
                    difference = recordingCount - pictureCount;
                    totalSteps = recordingCount + difference;
                }
                else
                {
                    totalSteps = pictureCount;
                }

                TextView stepCounter = FindViewById<TextView>(Resource.Id.CountBox);
                stepCounter.Text = totalSteps + " clicks left";

                bool firstClick = false; 
                int counter = 0; 
                pictureButton.Click += delegate
                {
                    
                    if (firstClick.Equals(false))
                    {
                        totalSteps--;
                        firstClick = true; 
                    }
                    else
                    {
                        if (totalSteps.Equals(0))
                        {
                            pictureButton.Enabled = false; 
                        }
                        else
                        {
                            counter++;
                            totalSteps--;
                        }
                        
                    }
                    if (totalSteps.Equals(0))
                    {
                        stepCounter.Text = "No Clicks Left";
                        pictureButton.Enabled = false; 
                    }
                    else if (totalSteps.Equals(1))
                    {
                        stepCounter.Text = totalSteps + " click left";
                    }
                    else
                    {
                        stepCounter.Text = totalSteps + " clicks left";
                    }
                   

                    if (pictureCount > counter)
                    {
                        using (var image = GetImage(options, pictures[counter].Path))
                        {
                            pictureButton.SetImageBitmap(image);
                        
                        }
                    }
                    
                    if (recordingCount > counter)
                    {
                        List<string> recording = new List<string>();
                        recording.Add(voiceRecordings[counter].Path);
                        VoiceRecording.VoiceRecording audioPlayer = new VoiceRecording.VoiceRecording();
                        audioPlayer.PlayRecordings(recording);
                    }

                };

            }

            Button cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
            cancelButton.Click += delegate
            {
                Intent exitIntent = new Intent(this, typeof(MainActivity));
                StartActivity(exitIntent);
            };
        }

        public Bitmap GetImage(BitmapFactory.Options options, string path)
        {
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            
            {
                
                var bitmap = BitmapFactory.DecodeStream (fs, null, options);
                if (bitmap != null)
                {
                    Toast.MakeText(this, "Images Loading...", ToastLength.Short).Show();
                }

                return bitmap;
            }
            
        }
    }
}