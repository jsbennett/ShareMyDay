using Android.Graphics;
using Android.Media;
using Android.Widget;
using System;
using System.Collections.Generic;
using ShareMyDay.Database.Models;
using ShareMyDay.UIComponents;
using Path = System.IO.Path;

namespace ShareMyDay.VoiceRecording
{
    /*
     * Class Name: VoiceRecording
     * Purpose: To provide voice recording functionality
     * NFC Code adapted from Xamarin Mobile Development for Android Cookbook by Matthew Leibowitz pages 246-250, 260 - 262
     */
    class VoiceRecording
    {
        private string _audioPath;
        private MediaRecorder _voiceRecorder;
        private MediaPlayer _audioPlayer;
        private List<string> _audioPaths;
      
        /*
         * Constructor
         * Used to set up the global variables
         */
        public VoiceRecording()
        {
            _audioPaths = new List<string>();
            _voiceRecorder = null;
            _audioPlayer = null; 
        }

        /*
         * Method Name: Begin
         * Purpose: Used to control the start and stopping of a voice recording 
         */
        public void Begin(Button button, Button submitButton, Button playButton, bool redo)
        {
            if (redo)
            {
                _audioPaths = new List<string>();
                button.Text = "Start Recording";
            }
            if (_voiceRecorder == null) {
                submitButton.Enabled = false;
                submitButton.SetBackgroundResource(Resource.Drawable.ButtonDimmedGenerator);
                submitButton.SetTextColor(Color.ParseColor("#969a90"));
                playButton.Enabled = false; 
                playButton.SetBackgroundResource(Resource.Drawable.ButtonDimmedGenerator);
                playButton.SetTextColor(Color.ParseColor("#969a90"));
               
                _voiceRecorder = new MediaRecorder ();
                _voiceRecorder.SetAudioSource (AudioSource.Mic);
                _voiceRecorder.SetOutputFormat (OutputFormat.ThreeGpp);
                _voiceRecorder.SetAudioEncoder (AudioEncoder.Default);
                _audioPath = Path.Combine (Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath, $"audio{Guid.NewGuid()}.3gp");
                _voiceRecorder.SetOutputFile (_audioPath);
                _voiceRecorder.Prepare ();
                _voiceRecorder.Start ();
               
                button.Text = "Stop Recording";
                button.SetBackgroundColor(Color.Red);
            }
            else
            {
                submitButton.Enabled = true; 
                submitButton.SetBackgroundResource(Resource.Drawable.ButtonGenerator);
                submitButton.SetTextColor(Color.White);
                playButton.Enabled = true; 
                playButton.SetBackgroundResource(Resource.Drawable.ButtonGenerator);
                playButton.SetTextColor(Color.White);
                
                _audioPaths.Add(_audioPath);
                _voiceRecorder.Stop ();
                _voiceRecorder.Reset ();
                _voiceRecorder.Release ();
                _voiceRecorder = null;
                
                button.Text = "Start Recording";
                button.SetBackgroundColor(Color.Green);
            }
        }

        /*
         * Method Name: Play
         * Purpose: To play the voice recording 
         */
        public void Play()
        {
            List<string> copy = new List<string>();
            for (int i = 0; i < _audioPaths.Count; i++)
            {
                copy.Add(_audioPaths[i]);
            }
            
            PlayRecordings(copy);
        }

        /*
         * Method Name: PlayRecordings
         * Purpose: To control the audio player when going through an array of voice recordings 
         */
        public void PlayRecordings(List<string> audio)
        {
            if (audio.Count != 0)
            {
                if (_audioPlayer == null)
                {
                    _audioPlayer = new MediaPlayer();
                    string title = audio[0];
                    _audioPlayer.SetDataSource(title);
                    _audioPlayer.Prepare();
                    _audioPlayer.Start();
                   
                    _audioPlayer.Completion += delegate
                    {
                        _audioPlayer.Stop();
                        _audioPlayer.Release();
                        _audioPlayer = null;
                        audio.Remove(title);
                        PlayRecordings(audio);
                    };
                }
            }
        }
        
        /*
         * Method Name: SaveNewEvent
         * Purpose: To save a voice  recording as a separate event 
         */
        public bool SaveNewEvent(bool ticked)
        {
            Database.Database db = new Database.Database(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            StoryEvent storyEvent = new StoryEvent
            {
                Value = DateTime.Now.ToLongTimeString() +  "-" + "Voice Recording Taken",
                DateTime = DateTime.Now, 
                Finished = ticked
            };
            List<Database.Models.VoiceRecording> recordings = new List<Database.Models.VoiceRecording>();
            foreach (var i in _audioPaths)
            {
                recordings.Add(new Database.Models.VoiceRecording
                {
                    EventId = storyEvent.Id,
                    Path = i
                });
            }
                 
            return db.InsertEvent(true, storyEvent, null, null, recordings) != 0;

        }

        /*
         * Method Name: SaveExistingEvent
         * Purpose: To save the voice recording to an existing event 
         */
        public bool SaveExistingEvent(SpinnerComponent spinner, bool ticked)
        {
            
            Database.Database db = new Database.Database(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            var storyEvent = db.FindEventByValue(spinner.GetSelected());
            if (storyEvent.InStory)
            {
                return false; 
            }
            else
            {
                storyEvent.Finished = ticked; 
                List<Database.Models.VoiceRecording> recordings = new List<Database.Models.VoiceRecording>();
                foreach (var i in _audioPaths)
                {
                    recordings.Add(new Database.Models.VoiceRecording
                    {
                        EventId = storyEvent.Id,
                        Path = i
                    });
                }

                return db.InsertEvent(false, storyEvent, null, null, recordings) != 0;
            }
            
        }
        
    }
}