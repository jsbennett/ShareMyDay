using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareMyDay.Activities
{
    [Activity(Label = "EditEventsActivity")]
    public class EditEventsActivity : Activity
    {
        private Button _close;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var previousActivity = Intent.GetStringExtra("Event");
            var db = new Database.Database(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "ShareMyDay.db3");

            var eventInformation = db.FindEventByValue(previousActivity);
            _close = new Button(this);
            _close.SetBackgroundResource(Resource.Drawable.Back);

            _close.Click += delegate
            {
                _close.SetBackgroundResource(Resource.Drawable.BackClicked);
                Intent back = new Intent(this, typeof(EventListActivity));
                StartActivity(back);
            };
            LinearLayout outerLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };

            ScrollView innerLayout = new ScrollView(this);

            LinearLayout informationLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };
            informationLayout.SetPadding(0,0,0,50);
            if (eventInformation != null)
            {
                TextView title = new TextView(this)
                {
                    Text = previousActivity, 
                    TextSize = 30,
                    TextAlignment = TextAlignment.Center
                };

                title.SetTextColor(Color.White);
                title.SetBackgroundColor(Color.ParseColor("#213f5e"));
                informationLayout.AddView(title);
                if (eventInformation.Cards != null && eventInformation.Cards.Count != 0)
                {
                    TextView cardTitle = new TextView(this)
                    {
                        Text = "Card Tapped: " + eventInformation.Cards[0].Message,
                        TextSize = 20 

                    };
                    cardTitle.SetTextColor(Color.Black);
                    cardTitle.SetPadding(40,50,0,10);
                    informationLayout.AddView(cardTitle);
                }
                else
                {
                    TextView cardTitle = new TextView(this)
                    {
                        Text = "No cards tapped for this event",
                        TextSize = 20
                    };
                    cardTitle.SetTextColor(Color.Black);
                    cardTitle.SetPadding(40,50,0,50);
                    informationLayout.AddView(cardTitle);
                }

                if (eventInformation.VoiceRecordings != null && eventInformation.VoiceRecordings.Count != 0)
                {
                    TextView recordingTitle = new TextView(this)
                    {
                        Text = "Event Recordings: ",
                        TextSize = 20
                    };
                    recordingTitle.SetTextColor(Color.Black);
                    recordingTitle.SetPadding(40,0,0,50);
                    informationLayout.AddView(recordingTitle);

                    int count = 0;
                    foreach (var i in eventInformation.VoiceRecordings)
                    {
                        count++;
                        Button voiceRecording = new Button(this)
                        {
                            Text = "Play recording " + count,
                            
                        };
                        voiceRecording.SetTextColor(Color.White);
                        voiceRecording.SetBackgroundResource(Resource.Drawable.ButtonGenerator);
                        voiceRecording.Click += delegate
                        {
                            List<string> copy = new List<string>();

                            copy.Add(i.Path);


                            VoiceRecording.VoiceRecording audioPlayer = new VoiceRecording.VoiceRecording();

                            audioPlayer.PlayRecordings(copy);
                        };
                        voiceRecording.SetPadding(0,30,0,30);
                        informationLayout.AddView(voiceRecording, ViewGroup.LayoutParams.MatchParent,    
                            250);

                    }

                    if (eventInformation.VoiceRecordings.Count > 1)
                    {
                        Button groupedVoiceRecordings = new Button(this)
                        {
                            Text = "Play All Recordings "
                        };
                        groupedVoiceRecordings.SetTextColor(Color.White);
                        groupedVoiceRecordings.SetBackgroundResource(Resource.Drawable.ButtonGenerator);
                        groupedVoiceRecordings.Click += delegate
                        {

                            List<string> copy = new List<string>();
                            foreach (var j in eventInformation.VoiceRecordings)
                            {
                                copy.Add(j.Path);
                            }

                            VoiceRecording.VoiceRecording audioPlayer = new VoiceRecording.VoiceRecording();

                            audioPlayer.PlayRecordings(copy);

                        };
                        informationLayout.AddView(groupedVoiceRecordings, ViewGroup.LayoutParams.MatchParent,    
                            250);

                    }
                }
                else
                {
                    TextView recordingTitle = new TextView(this)
                    {
                        Text = "No voice recordings have been made for this event",
                        TextSize = 20
                    };
                    recordingTitle.SetTextColor(Color.Black);
                    recordingTitle.SetPadding(40,50,0,50);
                    informationLayout.AddView(recordingTitle);
                }


                if (eventInformation.Pictures != null && eventInformation.Pictures.Count != 0)
                {

                    TextView imageTitle = new TextView(this)
                    {
                        Text = "Event Pictures:",
                        TextSize = 20
                    };
                    imageTitle.SetTextColor(Color.Black);
                    imageTitle.SetPadding(40,50,0,50);
                    informationLayout.AddView(imageTitle);
                    foreach (var i in eventInformation.Pictures)
                    {
                        ImageView imageViewer = new ImageView(this);
                        imageViewer.SetPadding(0,10,0,50);
                        var options = new BitmapFactory.Options {InJustDecodeBounds = true};

                        var sample = 4;
                        options.InSampleSize = sample;
                        
                        options.InJustDecodeBounds = false;
                        using (var image = await GetImage(options, i.Path))
                        {
                            if (image == null)
                            {
                                TextView noImageTitle = new TextView(this)
                                {
                                    Text = "Sorry the image does not exist.",
                                    TextSize = 20

                                };
                                noImageTitle.SetTextColor(Color.Black);
                                noImageTitle.SetPadding(40,50,0,50);
                                informationLayout.AddView(noImageTitle);
                            }
                            else
                            {
                                imageViewer.SetImageBitmap(image);
                                informationLayout.AddView(imageViewer);
                            }
                          
                        }

                    }
                }
                else
                {
                    TextView imageTitle = new TextView(this)
                    {
                        Text = "No pictures have been taken for this event",
                        TextSize = 20
                        
                    };
                    imageTitle.SetTextColor(Color.Black);
                    imageTitle.SetPadding(40,50,0,50);
                    informationLayout.AddView(imageTitle);
                }

                informationLayout.AddView(_close, ViewGroup.LayoutParams.MatchParent,    
                    450);
                innerLayout.AddView(informationLayout,
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent);
                outerLayout.AddView(innerLayout);

                SetContentView(outerLayout);
            }

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
        
        public async Task<Bitmap> GetImage(BitmapFactory.Options options, string path)
        {
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            
            {
                
                var bitmap = await BitmapFactory.DecodeStreamAsync (fs, null, options);
                    if (bitmap != null)
                    {
                        Toast.MakeText(this, "Images Loading...", ToastLength.Short).Show();
                    }

                    return bitmap;
                }
            
        }

    }
}