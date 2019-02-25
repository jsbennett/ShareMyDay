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
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var previousActivity = Intent.GetStringExtra("Event");
            var db = new Database.Database(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "ShareMyDay.db3");

            var eventInformation = db.FindByValue(previousActivity);

            LinearLayout outerLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };

            ScrollView innerLayout = new ScrollView(this);

            LinearLayout informationLayout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };
            informationLayout.SetPadding(0,50,0,50);
            if (eventInformation != null)
            {
                TextView title = new TextView(this)
                {
                    Text = previousActivity, 
                    TextSize = 30,
                    TextAlignment = TextAlignment.Center
                    
                    
                    
                };

                informationLayout.AddView(title);
                if (eventInformation.Cards != null && eventInformation.Cards.Count != 0)
                {
                    TextView cardTitle = new TextView(this)
                    {
                        Text = "Card Tapped: " + eventInformation.Cards[0].Message,
                        TextSize = 20 

                    };
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
                    recordingTitle.SetPadding(40,50,0,50);
                    informationLayout.AddView(recordingTitle);

                    int count = 0;
                    foreach (var i in eventInformation.VoiceRecordings)
                    {
                        count++;
                        Button voiceRecording = new Button(this)
                        {
                            Text = "Play recording " + count
                        };

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
                    
                    imageTitle.SetPadding(40,50,0,50);
                    informationLayout.AddView(imageTitle);
                }


                Button close = new Button(this)
                {
                    Text = "Back"
                };

                close.Click += delegate
                {
                    Intent back = new Intent(this, typeof(EventListActivity));
                    StartActivity(back);
                };

                informationLayout.AddView(close, ViewGroup.LayoutParams.MatchParent,    
                    250);
                innerLayout.AddView(informationLayout,
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent);
                outerLayout.AddView(innerLayout);

                SetContentView(outerLayout);
            }

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