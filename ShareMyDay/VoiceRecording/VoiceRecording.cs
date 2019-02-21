using Android.Graphics;
using Android.Media;
using Android.Widget;
using System;
using System.Collections.Generic;
using ShareMyDay.Database.Models;
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
      
        //private List<string> copy; 

        public VoiceRecording()
        {
            _audioPaths = new List<string>();
            _voiceRecorder = null;
            _audioPlayer = null; 
        }

        public void Begin(Button button, Button submitButton, Button playButton, bool redo)
        {
            if (redo)
            {
                _audioPaths = new List<string>();
                button.Text = "Start Recording";
            }
            if (_voiceRecorder == null) {
                submitButton.Enabled = false;
                playButton.Enabled = false; 
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
                playButton.Enabled = true; 
                _audioPaths.Add(_audioPath);
                _voiceRecorder.Stop ();
                _voiceRecorder.Reset ();
                _voiceRecorder.Release ();
                _voiceRecorder = null;
                button.Text = "Start Recording";
                button.SetBackgroundColor(Color.Green);
            }
        }

        public void Play(Button button)
        {
            //send it to db class 
            List<string> copy = new List<string>();
            for (int i = 0; i < _audioPaths.Count; i++)
            {
                copy.Add(_audioPaths[i]);
            }
            
        
            PlayRecordings(copy);
        }

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

        public StoryEvent save()
        {
            List<Database.Models.VoiceRecording> recordings = new List<Database.Models.VoiceRecording>();
            foreach (var i in _audioPaths)
            {
                recordings.Add(new Database.Models.VoiceRecording
                {
                    Path = i
                });
            }

            StoryEvent storyEvent = new StoryEvent
            {
                Value = DateTime.Now.ToLongTimeString() + " Voice Recording",
                DateTime = DateTime.Now, 
                VoiceRecordings = recordings
            };
            return storyEvent;
        }
        
    }
}