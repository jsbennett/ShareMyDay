using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Net;

namespace ShareMyDay.Activities
{
    [Activity(Label = "EditEventsActivity")]
    public class EditEventsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var previousActivity = Intent.GetStringExtra("Event");
            var db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");

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

            if (eventInformation!=null)
            {
                TextView title = new TextView(this)
                {
                    Text = previousActivity
                };

                informationLayout.AddView(title);
                if (eventInformation.Cards != null && eventInformation.Cards.Count != 0)
                {
                    TextView cardTitle = new TextView(this)
                    {
                        Text = "Card Tapped: " + eventInformation.Cards[0].Message

                    };
                    informationLayout.AddView(cardTitle);
                }
                else
                {
                    TextView cardTitle = new TextView(this)
                    {
                        Text = "No cards tapped for this event"
                    };
                    informationLayout.AddView(cardTitle);
                }

                if (eventInformation.VoiceRecordings != null && eventInformation.VoiceRecordings.Count != 0)
                {
                    TextView recordingTitle = new TextView(this)
                    {
                        Text = "Event Recordings: "
                    };
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

                        informationLayout.AddView(voiceRecording);
      
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
                        informationLayout.AddView(groupedVoiceRecordings);

                    }
                }
                else
                {
                    TextView recordingTitle = new TextView(this)
                    {
                        Text = "No voice recordings have been made for this event"
                    };
                    informationLayout.AddView(recordingTitle);
                }

                if (eventInformation.Pictures != null && eventInformation.Pictures.Count != 0)
                {
                    TextView imageTitle = new TextView(this)
                    {
                        Text = "Event Pictures:"
                    };
                    informationLayout.AddView(imageTitle);
                    foreach (var i in eventInformation.Pictures)
                    {
                        ImageView imageViewer = new ImageView(this);
                        imageViewer.SetImageURI(Uri.Parse(i.Path));

                        informationLayout.AddView(imageViewer);
                    }
                }
                else
                {
                    TextView imageTitle = new TextView(this)
                    {
                        Text = "No pictures have been taken for this event"
                        
                    };
                    informationLayout.AddView(imageTitle);
                }

            }
            innerLayout.AddView( informationLayout,     
                ViewGroup.LayoutParams.MatchParent,    
                ViewGroup.LayoutParams.WrapContent);

            outerLayout.AddView(innerLayout);

            SetContentView(outerLayout);
         
        }
    }
}