using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Speech.Tts;
using Android.Widget;
using Java.Lang;
using Java.Util;
using ShareMyDay.Database.Models;
using ShareMyDay.Story.StoryFunctions;
using ShareMyDay.UIComponents;
using Picture = ShareMyDay.Database.Models.Picture;
using String = System.String;
using Uri = Android.Net.Uri;

namespace ShareMyDay.Activities
{
    [Activity(Label = "StoryActivity")]
    public class StoryActivity : Activity, Android.Speech.Tts.TextToSpeech.IOnInitListener
    {
        private string storyId;
        private Android.Speech.Tts.TextToSpeech tts;
        private string text; 
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
                        else
                        {
                            bool cardFound = false; 
                            foreach (var j in storyEvents)
                            {
                                if (j.Cards != null && j.Cards.Count != 0)
                                {
                                    text = CreateSentence(j.Cards[0]);
                                    cardFound = true; 
                                }

                            }

                            if (!cardFound)
                            {
                                text = "Never guess what else happened in school today! Have a look at all of this!";
                            }
                            
                          }

                        done = true;
                    }

                }
       
                if (story.Extra && !done)
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
                        else
                        {
                           
                            bool cardFound = false; 
                            foreach (var j in storyEvents)
                            {
                                if (j.Cards != null && j.Cards.Count != 0)
                                {
                                    text = CreateSentence(j.Cards[0]);
                                    cardFound = true; 
                                }

                            }

                            if (!cardFound)
                            {
                                text = "Never guess what else happened in school today! Have a look at all of this!";
                            }
                        }
                    }
                }

                int pictureCount = pictures.Count;
                int recordingCount = voiceRecordings.Count;
                
                int totalSteps = 0;
                if (pictureCount > recordingCount)
                {

                    totalSteps = pictureCount;
                }else if (recordingCount > pictureCount)
                {
                    totalSteps = recordingCount;
                }
                else
                {
                    if (pictureCount != 0)
                    {
                        totalSteps = pictureCount;
                    }
                    else
                    {
                        if (recordingCount != 0)
                        {
                            totalSteps = recordingCount;
                        }
                    }
                    
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
                        if (story.TextToSpeech)
                        {
                            if (tts == null)
                            {
                                tts = new Android.Speech.Tts.TextToSpeech(this,this);
                            }
                            else
                            {
                                tts.Speak(text, QueueMode.Flush, null, null);
                            }
                        
                        }
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

        public string CreateSentence(Card card)
        {
            string sentence = " "; 
            switch (card.Type)
            {
                    case "1":
                        sentence = "Today, during school I did " + card.Message;
                        break;
                    case "2":
                        sentence = "Today, during school I took part in  " + card.Message + " in class.";
                        break;
                    case "3":
                        sentence = "Today, during school I was in  " + card.Message + " class.";
                        break;
                    case "4":
                        sentence = "Today, during school I used  " + card.Message;
                        break;
                    case "5":
                        sentence = "Today, during school I was with  " + card.Message;
                        break;
                    case "6":
                        sentence = "Today, during school I was with  " + card.Message;
                        break;
                    case "7":
                        sentence = "Today, during school I was visited by " + card.Message;
                        break;
            }

            return sentence; 
        }

        public void OnInit(OperationResult status)
        {
            if (status.Equals(OperationResult.Success))
            {
               Speak(text);
            }
           
        }

        private void Speak(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                tts.Speak(text, QueueMode.Flush, null, null);
            }
        }
    }
}