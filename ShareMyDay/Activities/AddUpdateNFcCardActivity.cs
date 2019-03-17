using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using ShareMyDay.UIComponents;

namespace ShareMyDay.Activities
{
    /*
     *
     */
    [Activity(Label = "AddUpdateNFcCardActivity")]
    public class AddUpdateNFcCardActivity : Activity
    {
        private NFC.NFC _nfc;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddUpdateNfcCardView);
           
            SpinnerComponent spinner = new SpinnerComponent (this, Resource.Id.nfcCardTypeDropDown, this);
            spinner.SetupNFcDropDown();
            
            _nfc = new NFC.NFC(this);
            Typeface buttonFont = Typeface.CreateFromAsset (Assets, "Kano.otf");
            Button submitButton = FindViewById<Button>(Resource.Id.submitButton);
            submitButton.SetTypeface(buttonFont,TypefaceStyle.Bold);
            submitButton.SetBackgroundResource(Resource.Drawable.ButtonGenerator);
            submitButton.SetTextColor(Color.White);
            submitButton.SetTextSize(ComplexUnitType.Dip,15);
            EditText inputBox = FindViewById<EditText>(Resource.Id.messageInputBox);
            

            Button closeButton = FindViewById<Button>(Resource.Id.cancelButton);
            closeButton.SetTypeface(buttonFont,TypefaceStyle.Bold);
            closeButton.SetBackgroundResource(Resource.Drawable.ButtonGenerator);
            closeButton.SetTextColor(Color.White);
            closeButton.SetTextSize(ComplexUnitType.Dip,15);
            closeButton.Click += delegate
            {
                Intent mainMenuIntent = new Intent(this, typeof(TeacherMainMenuActivity));
                StartActivity(mainMenuIntent);
            };

            submitButton.Click += delegate
            {
                if (inputBox.Text.Length >= 25)
                {
                    AlertBoxComponent tooLongAlert = new AlertBoxComponent(this);
                    tooLongAlert.Setup("Value Too Long", "The value you have entered is too long to be put on the card. Please shorten your input.");
                    tooLongAlert.Show();
                }
                else
                {
                    _nfc.WriteDetection(spinner.GetSelected(),inputBox.Text,this,this);
                    closeButton.Text = "Close";
                }
            };
        }

        protected override async void OnNewIntent(Intent intent)
        {
           await _nfc.Write( this, this, intent);
        }

     
    }
}