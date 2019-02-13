using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ShareMyDay.Activities
{
    /*
     * Code adapted from Xamarin Mobile Development for Android Cookbook by Matthew Leibowitz page 157-159
     */
    [Activity(Label = "AddNFcCardActivity")]
    public class AddNFcCardActivity : Activity
    {
        private NFC.NFC _nfc;
        public Dialog dialog1;

        public static string inputMessage; 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddNfcCardView);
            
            _nfc = new NFC.NFC(this);

            Button submitButton = FindViewById<Button>(Resource.Id.submitButton);
            EditText inputBox = FindViewById<EditText>(Resource.Id.messageInputBox);

            submitButton.Click += delegate
            {
                _nfc.WriteDetection(inputBox.Text,this,this);

            };
        }

        protected override async void OnNewIntent(Intent intent)
        {
           await _nfc.Write(intent, this, this);
        }

     
    }
}