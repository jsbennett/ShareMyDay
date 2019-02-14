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
     *
     */
    [Activity(Label = "AddNFcCardActivity")]
    public class AddNFcCardActivity : Activity
    {
        private NFC.NFC _nfc;

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
           await _nfc.Write( this, this, intent);
        }

     
    }
}