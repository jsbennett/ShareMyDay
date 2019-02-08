using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
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
        private readonly string _audioPath;
        private MediaRecorder _voiceRecorder;
        private MediaPlayer _audioPlayer;

        public VoiceRecording()
        {
            _voiceRecorder = null;
            _audioPlayer = null;  
            _audioPath = Path.Combine (Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath, $"audio{Guid.NewGuid()}.3gp");
        }

        public void Begin(Button button)
        {
            if (_voiceRecorder == null) {
                _voiceRecorder = new MediaRecorder ();
                _voiceRecorder.SetAudioSource (AudioSource.Mic);
                _voiceRecorder.SetOutputFormat (OutputFormat.ThreeGpp);
                _voiceRecorder.SetAudioEncoder (AudioEncoder.Default);
                _voiceRecorder.SetOutputFile (_audioPath);
                _voiceRecorder.Prepare ();
                _voiceRecorder.Start ();
                button.Text = "Stop Recording";
                button.SetBackgroundColor(Color.Red);
            }
            else
            {
                /*try {
                    _voiceRecorder.Stop ();
                } catch (Java.Lang.RuntimeException) {
                    File.Delete (_audioPath);
                }*/
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
            if (_audioPlayer == null) {
                _audioPlayer = new MediaPlayer ();
                _audioPlayer.SetDataSource (_audioPath);
                _audioPlayer.Prepare ();
                _audioPlayer.Start ();
                button.Text = "Stop Playing Recording";
                
                _audioPlayer.Completion += delegate { 
                    _audioPlayer.Stop ();
                    _audioPlayer.Release ();
                    _audioPlayer = null;
                    button.Text = "Play Recording";
                };
            } 
            else 
            {
                _audioPlayer.Stop ();
                _audioPlayer.Release ();
                _audioPlayer = null;
                button.Text = "Play Recording";
            }
        }
        
    }
}