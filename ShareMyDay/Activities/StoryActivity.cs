using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Speech.Tts;
using Android.Views;
using Android.Widget;
using ShareMyDay.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Picture = ShareMyDay.Database.Models.Picture;
using String = System.String;

namespace ShareMyDay.Activities
{
    [Activity(Label = "StoryActivity")]
    public class StoryActivity : Activity, TextToSpeech.IOnInitListener
    {
        private string _storyId;
        private TextToSpeech _phoneVoice;
        private string _text;
        public Database.Database _db;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StoryView);

            _storyId = Intent.GetStringExtra("Story");

            ImageView pictureButton = FindViewById<ImageView>(Resource.Id.pictureBox);
            _db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                "ShareMyDay.db3");

            var options = new BitmapFactory.Options {InJustDecodeBounds = true};

            var sample = 4;
            options.InSampleSize = sample;
            options.InJustDecodeBounds = false;

            Database.Models.Story story;
            if (_storyId == "Favourite")
            {
                story = _db.FindFavouriteStory();

                if (story != null)
                {
                    SetupStory(story, options, pictureButton, _db, true);
                }
                else
                {
                    Intent noFavouriteStories = new Intent(this, typeof(NoStoriesActivity));
                    noFavouriteStories.PutExtra("StoryType", "Favourite");
                    StartActivity(noFavouriteStories);
                }
            }
            else
            {
                story = _db.FindStoryById(_storyId);

                SetupStory(story, options, pictureButton, _db, false);
            }
        }

        public Bitmap GetImage(BitmapFactory.Options options, string path)
        {
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))

            {
                var bitmap = BitmapFactory.DecodeStream(fs, null, options);
                if (bitmap != null)
                {
                    Toast.MakeText(this, "Images Loading...", ToastLength.Short).Show();
                }

                return bitmap;
            }
        }

        public void SetupStory(Database.Models.Story story, BitmapFactory.Options options, ImageView pictureButton,
            Database.Database db, bool favourite)
        {
           TextView title = FindViewById<TextView>(Resource.Id.titleBox);
            title.Text = story.TitleValue;

            List<StoryEvent> storyEvents = db.FindEventsFromStory(story.Id.ToString());
            List<Picture> pictures = new List<Picture>();
            List<Database.Models.VoiceRecording> voiceRecordings = new List<Database.Models.VoiceRecording>();
            if (story.DefaultPicture== null)
            {
                using (var image = GetImage(options, story.CoverPhoto))
                {
                    pictureButton.SetImageBitmap(image);
                }

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
                                    _text = CreateSentence(j.Cards[0]);
                                    cardFound = true;
                                }
                            }

                            if (!cardFound)
                            {
                                _text = "Never guess what else happened in school today! Have a look at all of this!";
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
                                    _text = CreateSentence(j.Cards[0]);
                                    cardFound = true;
                                }
                            }

                            if (!cardFound)
                            {
                                _text = "Never guess what else happened in school today! Have a look at all of this!";
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
                }
                else if (recordingCount > pictureCount)
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
                stepCounter.Text = "Click the picture to begin!";

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
                            if (_phoneVoice == null)
                            {
                                _phoneVoice = new TextToSpeech(this, this);
                            }
                            else
                            {
                                _phoneVoice.Speak(_text, QueueMode.Flush, null, null);
                            }
                        }
                    }
                    else
                    {
                        if (totalSteps.Equals(0))
                        {
                            //pictureButton.Enabled = false;
                            //totalSteps++;
                            totalSteps--;
                        }
                        else
                        {
                            counter++;
                            totalSteps--;
                        }
                    }

                    if (totalSteps.Equals(0))
                    {
                        stepCounter.Text = "No Clicks Left. Click the picture to play again.";
                        stepCounter.TextSize = 20;

                    }
                    else if (totalSteps.Equals(1))
                    {
                        stepCounter.Text = totalSteps + " click left";
                    }
                    else if (totalSteps <= -1)
                    {
                        Intent replayStory = new Intent(this, typeof(StoryActivity));
                        replayStory.PutExtra("Story", _storyId);
                        StartActivity(replayStory);
                        stepCounter.Text = "No Clicks Left. Click the picture to play again.";
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

                    if (recordingCount > counter  && totalSteps > -1)
                    {
                        List<string> recording = new List<string>();
                        recording.Add(voiceRecordings[counter].Path);
                        VoiceRecording.VoiceRecording audioPlayer = new VoiceRecording.VoiceRecording();
                        audioPlayer.PlayRecordings(recording);
                    }
                };



                Button closeButton = FindViewById<Button>(Resource.Id.closeButton);
                Button favouriteButton = FindViewById<Button>(Resource.Id.favouriteButton);


                if (story.Favourite)
                {
                    favouriteButton.SetBackgroundResource(Resource.Drawable.MakeFavouriteButton);
                }

                closeButton.Click += async delegate
                {
                    await Task.Delay(1);

                    closeButton.SetBackgroundResource(Resource.Drawable.BigCloseButtonClicked);
                    Intent exitIntent = new Intent(this, typeof(MainActivity));
                    StartActivity(exitIntent);
                };

                favouriteButton.Click += async delegate
                {
                    var favouriteStory = _db.FindStoryById(story.Id.ToString());
                    if (favouriteStory.Favourite)
                    {

                        favouriteButton.SetBackgroundResource(Resource.Drawable.MakeFavouriteButtonClicked);
                        await Task.Delay(10);
                        AlertDialog.Builder alreadyFavourite = new AlertDialog.Builder(this);
                        alreadyFavourite.SetTitle("Are you sure you want to unfavourite?");
                        alreadyFavourite.SetMessage(
                            "This story is your favourite! Are you sure you want to remove it?");
                        alreadyFavourite.SetPositiveButton("Yes",
                            (senderAlerts, argss) =>
                            {
                                _db.RemoveFavourite(story.Id.ToString());

                                favouriteButton.SetBackgroundResource(Resource.Drawable.unFaveButton);
                            });
                        alreadyFavourite.SetNegativeButton("No",
                            (senderAlerts, argss) =>
                            {
                                favouriteButton.SetBackgroundResource(Resource.Drawable.MakeFavouriteButton);
                            });
                        alreadyFavourite.Create();
                        alreadyFavourite.Show();
                    }
                    else
                    {
                        await Task.Delay(10);
                        favouriteButton.SetBackgroundResource(Resource.Drawable.unFaveButtonClicked);
                        AlertDialog.Builder favouriteCheckBox = new AlertDialog.Builder(this);
                        favouriteCheckBox.SetTitle("New Favourite");
                        favouriteCheckBox.SetMessage(
                            "If you make this story your new favourite, the current favourite story will be lost. Do you want to continue?");
                        favouriteCheckBox.SetPositiveButton("Yes", async (senderAlert, args) =>
                        {
                            if (db.UpdateFavourite(story.Id))
                            {
                                AlertDialog.Builder favouriteChanged = new AlertDialog.Builder(this);
                                favouriteChanged.SetTitle("Favourite Story Saved");
                                favouriteChanged.SetMessage("This story is now your favourite story.");
                                favouriteChanged.SetNeutralButton("Ok", (senderAlerts, argss) => { });
                                favouriteChanged.Create();
                                favouriteChanged.Show();


                                await Task.Delay(1);
                                favouriteButton.SetBackgroundResource(Resource.Drawable.MakeFavouriteButton);
                            }
                        });
                        favouriteCheckBox.SetNegativeButton("No",
                            (senderAlert, args) =>
                            {
                                favouriteButton.SetBackgroundResource(Resource.Drawable.unFaveButton);
                            });
                        favouriteCheckBox.Create();
                        favouriteCheckBox.Show();
                    }
                };

                Button cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
                cancelButton.Click += delegate
                {
                    cancelButton.SetBackgroundResource(Resource.Drawable.FinishClicked);
                    if (totalSteps.Equals(0) && !favourite)
                    {
                        title.Visibility = ViewStates.Invisible;
                        stepCounter.Visibility = ViewStates.Invisible;
                        cancelButton.Visibility = ViewStates.Invisible;
                        pictureButton.Visibility = ViewStates.Invisible;
                        closeButton.Visibility = ViewStates.Visible;
                        favouriteButton.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        Intent exitIntent = new Intent(this, typeof(MainActivity));
                        StartActivity(exitIntent);
                    }
                };
            } 
            else
            {
                int image = (int)typeof(Resource.Drawable).GetField(story.DefaultPicture).GetValue(null);
                pictureButton.SetImageResource(image);

                if (story.Extra.Equals(false) && story.TextToSpeech.Equals(false))
                {
                    foreach (var i in storyEvents)
                    {
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
                                    _text = CreateSentence(j.Cards[0]);
                                    cardFound = true;
                                }
                            }

                            if (!cardFound)
                            {
                                _text = "Never guess what else happened in school today! Have a look at all of this!";
                            }
                        }

                        done = true;
                    }
                }

                if (story.Extra && !done)
                {
                    foreach (var i in storyEvents)
                    {
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
                                    _text = CreateSentence(j.Cards[0]);
                                    cardFound = true;
                                }
                            }

                            if (!cardFound)
                            {
                                _text = "Never guess what else happened in school today! Have a look at all of this!";
                            }
                        }
                    }
                }
                
                int recordingCount = voiceRecordings.Count;

                int totalSteps = 0;
                if (recordingCount != 0)
                {
                    totalSteps = recordingCount;
                }
                else
                {
                    totalSteps = 1;
                }
                  
                TextView stepCounter = FindViewById<TextView>(Resource.Id.CountBox);
                stepCounter.Text = "Click the picture to begin!";

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
                            if (_phoneVoice == null)
                            {
                                _phoneVoice = new TextToSpeech(this, this);
                            }
                            else
                            {
                                _phoneVoice.Speak(_text, QueueMode.Flush, null, null);
                            }
                        }
                    }
                    else
                    {
                        if (totalSteps.Equals(0))
                        {
                            //pictureButton.Enabled = false;
                            //totalSteps++;
                            totalSteps--;
                        }
                        else
                        {
                            counter++;
                            totalSteps--;
                        }
                    }

                    if (totalSteps.Equals(0))
                    {
                        stepCounter.Text = "No Clicks Left. Click the picture to play again.";
                        stepCounter.TextSize = 20;

                    }
                    else if (totalSteps.Equals(1))
                    {
                        stepCounter.Text = totalSteps + " click left";
                    }
                    else if (totalSteps <= -1)
                    {
                        Intent replayStory = new Intent(this, typeof(StoryActivity));
                        replayStory.PutExtra("Story", _storyId);
                        StartActivity(replayStory);
                        stepCounter.Text = "No Clicks Left. Click the picture to play again.";
                    }
                    else
                    {
                        stepCounter.Text = totalSteps + " clicks left";
                    }
                    
                   if (recordingCount > counter && totalSteps > -1)
                    {
                        List<string> recording = new List<string>();
                        recording.Add(voiceRecordings[counter].Path);
                        VoiceRecording.VoiceRecording audioPlayer = new VoiceRecording.VoiceRecording();
                        audioPlayer.PlayRecordings(recording);
                    }
                };

                  Button closeButton = FindViewById<Button>(Resource.Id.closeButton);
                Button favouriteButton = FindViewById<Button>(Resource.Id.favouriteButton);


                if (story.Favourite)
                {
                    favouriteButton.SetBackgroundResource(Resource.Drawable.MakeFavouriteButton);
                }

                closeButton.Click += async delegate
                {
                    await Task.Delay(1);

                    closeButton.SetBackgroundResource(Resource.Drawable.BigCloseButtonClicked);
                    Intent exitIntent = new Intent(this, typeof(MainActivity));
                    StartActivity(exitIntent);
                };

                favouriteButton.Click += async delegate
                {
                    var favouriteStory = _db.FindStoryById(story.Id.ToString());
                    if (favouriteStory.Favourite)
                    {

                        favouriteButton.SetBackgroundResource(Resource.Drawable.MakeFavouriteButtonClicked);
                        await Task.Delay(10);
                        AlertDialog.Builder alreadyFavourite = new AlertDialog.Builder(this);
                        alreadyFavourite.SetTitle("Are you sure you want to unfavourite?");
                        alreadyFavourite.SetMessage(
                            "This story is your favourite! Are you sure you want to remove it?");
                        alreadyFavourite.SetPositiveButton("Yes",
                            (senderAlerts, argss) =>
                            {
                                _db.RemoveFavourite(story.Id.ToString());

                                favouriteButton.SetBackgroundResource(Resource.Drawable.unFaveButton);
                            });
                        alreadyFavourite.SetNegativeButton("No",
                            (senderAlerts, argss) =>
                            {
                                favouriteButton.SetBackgroundResource(Resource.Drawable.MakeFavouriteButton);
                            });
                        alreadyFavourite.Create();
                        alreadyFavourite.Show();
                    }
                    else
                    {
                        await Task.Delay(10);
                        favouriteButton.SetBackgroundResource(Resource.Drawable.unFaveButtonClicked);
                        AlertDialog.Builder favouriteCheckBox = new AlertDialog.Builder(this);
                        favouriteCheckBox.SetTitle("New Favourite");
                        favouriteCheckBox.SetMessage(
                            "If you make this story your new favourite, the current favourite story will be lost. Do you want to continue?");
                        favouriteCheckBox.SetPositiveButton("Yes", async (senderAlert, args) =>
                        {
                            if (db.UpdateFavourite(story.Id))
                            {
                                AlertDialog.Builder favouriteChanged = new AlertDialog.Builder(this);
                                favouriteChanged.SetTitle("Favourite Story Saved");
                                favouriteChanged.SetMessage("This story is now your favourite story.");
                                favouriteChanged.SetNeutralButton("Ok", (senderAlerts, argss) => { });
                                favouriteChanged.Create();
                                favouriteChanged.Show();


                                await Task.Delay(1);
                                favouriteButton.SetBackgroundResource(Resource.Drawable.MakeFavouriteButton);
                            }
                        });
                        favouriteCheckBox.SetNegativeButton("No",
                            (senderAlert, args) =>
                            {
                                favouriteButton.SetBackgroundResource(Resource.Drawable.unFaveButton);
                            });
                        favouriteCheckBox.Create();
                        favouriteCheckBox.Show();
                    }
                };

                Button cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
                cancelButton.Click += delegate
                {
                    cancelButton.SetBackgroundResource(Resource.Drawable.FinishClicked);
                    if (totalSteps.Equals(0) && !favourite)
                    {
                        title.Visibility = ViewStates.Invisible;
                        stepCounter.Visibility = ViewStates.Invisible;
                        cancelButton.Visibility = ViewStates.Invisible;
                        pictureButton.Visibility = ViewStates.Invisible;
                        closeButton.Visibility = ViewStates.Visible;
                        favouriteButton.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        Intent exitIntent = new Intent(this, typeof(MainActivity));
                        StartActivity(exitIntent);
                    }
                };
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
                default:
                    sentence = " During school, look at what happened!";
                    break;
            }

            return sentence;
        }

        public void OnInit(OperationResult status)
        {
            if (status.Equals(OperationResult.Success))
            {
                Speak(_text);
            }
        }

        private void Speak(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                _phoneVoice.Speak(text, QueueMode.Flush, null, null);
            }
        }
    }
}