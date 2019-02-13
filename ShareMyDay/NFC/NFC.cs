using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Nfc;
using Android.Nfc.Tech;
using ShareMyDay.Activities;
using ShareMyDay.UIComponents;


namespace ShareMyDay.NFC
{
    /*
     * Class Name: NFC
     * Purpose: To set up and control NFC card interactions with the phone
     *
     */
    class NFC
    {
        private readonly NfcAdapter _nfcAdapter;
        private static string _inputMessage;
        private AlertBoxComponent _tapCardAlertBox;

        /*
         * Constructor
         * Used to get the adapter of the current Activity context
         */
        public NFC(Context context)
        {
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(context);
        }

        /*
         * Method name: NfcCardDetection
         * Purpose: To detect when an NFC card is tapped on the phone
         * NFC Code adapted from https://www.patrickvankleef.com/2017/01/08/xamarin-near-field-communication/, https://github.com/patkleef/XamarinNFC/blob/master/MainActivity.cs
         * and Xamarin Mobile Development for Android Cookbook by Matthew Leibowitz page 154 - 159
         */
        public void Detection(Context context, Activity activity)
        {
            //because we do not know what tag is discovered, an implicit intent is used 
            var detectedTag = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            
            var intentTagFilters = new[] { detectedTag };
            
            var contextIntent = new Intent(context, context.GetType()).AddFlags(ActivityFlags.SingleTop);

            //create an intent to return to activity 
            var pendingContextIntent = PendingIntent.GetActivity(context, 0, contextIntent, 0);
            
            //used to listen for nfc cards
            _nfcAdapter.EnableForegroundDispatch(activity, pendingContextIntent, intentTagFilters, null);
        }

        public void WriteDetection(string input, Context context, Activity activity)
        {
            if (input == "")
            {
                AlertBoxComponent emptyInputBox = new AlertBoxComponent(context);
                emptyInputBox.Setup("Blank Card Value", "Please enter a value for the card in the input box.");
                emptyInputBox.Show();
            }
            else
            {
                _tapCardAlertBox = new AlertBoxComponent(context);
                _tapCardAlertBox.Setup("Begin Writing Card", "Please tap and hold the card on the back of the phone.");
                _tapCardAlertBox.Show();
                _inputMessage = input;
                 Detection(context,activity);
            }
        }

        public async Task Write(Intent intent, Context context, Activity activity)
        {
            _tapCardAlertBox.GetDialog().Dismiss();
            AlertBoxComponent cardDetectionAlertBox = new AlertBoxComponent(context);
            cardDetectionAlertBox.Setup("Card Detected", "A card has been detected. Please hold whilst the application attempts to write to the card. You might need to wiggle the card around the back of the phone for it to work.");
            cardDetectionAlertBox.Show();

            if (intent.GetParcelableExtra(NfcAdapter.ExtraTag) is Tag cardTag)
            {
                var message1 = CreateCardContent();

                bool writeResult = await WriteMessage(cardTag, message1, context, activity);

                if (!writeResult)
                {
                    await FormatCard(cardTag, message1,context, activity);
                }
            }
        }

        private static NdefMessage CreateCardContent()
        {
            var messageBytes = Encoding.UTF8.GetBytes(_inputMessage);
            var mimeBytes = Encoding.UTF8.GetBytes("ShareMyDayTest");
            
            var ndefRecord = new NdefRecord(NdefRecord.TnfMimeMedia, mimeBytes, new byte[0], messageBytes);
            var ndefMessage = new NdefMessage(new[] {ndefRecord});

            return ndefMessage;
        }

        private async Task<bool> WriteMessage(Tag cardTag, NdefMessage cardContent, Context context, Activity activity)
        {
            try
            {
                var ndefTagConnection = Ndef.Get(cardTag);
                if (ndefTagConnection == null)
                {
                    AlertBoxComponent cardTagMissingAlertBox = new AlertBoxComponent(context);
                    cardTagMissingAlertBox.Setup("Card Not Found", "Please try tapping the card again.");
                    cardTagMissingAlertBox.Show();
                }

                await ndefTagConnection.ConnectAsync();
                if (!ndefTagConnection.IsWritable)
                {
                    AlertBoxComponent cardUnwriteableAlertBox = new AlertBoxComponent(context);
                    cardUnwriteableAlertBox.Setup("Card Unwritable", "This card is read only. Please try a different card.");
                    cardUnwriteableAlertBox.Show();
                }
                
                var messageSize = cardContent.ToByteArray().Length;
                if (ndefTagConnection.MaxSize < messageSize)
                {
                    AlertBoxComponent cardFullAlertBox = new AlertBoxComponent(context);
                    cardFullAlertBox.Setup("Card Storage Exceeded", "This card does not have enough storage. Please try a different card.");
                    cardFullAlertBox.Show();
                }

                await ndefTagConnection.WriteNdefMessageAsync(cardContent);
                ndefTagConnection.Close();
                
                AlertBoxComponent writeSuccessfulAlertBox = new AlertBoxComponent(context);
                writeSuccessfulAlertBox.OnlyOkOptionSetup("Card Write Was Successful", "The card was successfully written to. You will now be taken back to the main menu.", context, activity);
                writeSuccessfulAlertBox.Show();
                
                return true;
            }
            catch (Exception e)
            {
                AlertBoxComponent cardFullAlertBox = new AlertBoxComponent(context);
                string alertMessage = "An error has occured. Please try again. Error Message:"+ e;
                cardFullAlertBox.Setup("Error Writing To Card", alertMessage);
                cardFullAlertBox.Show();
            }

            return false;
        }

        private async Task FormatCard(Tag cardTag, NdefMessage cardContent, Context context, Activity activity)
        {
            try
            {
                var formatter = NdefFormatable.Get(cardTag);
                if (formatter == null)
                {
                    AlertBoxComponent cardFullAlertBox = new AlertBoxComponent(context);
                    cardFullAlertBox.Setup("Incorrect Card Type", "This card does not support Ndef format. Please use a different card.");
                    cardFullAlertBox.Show();
                }
                await formatter.ConnectAsync();
                await formatter.FormatAsync(cardContent);
                formatter.Close();
                
                AlertBoxComponent writeSuccessfulAlertBox = new AlertBoxComponent(context);
                writeSuccessfulAlertBox.OnlyOkOptionSetup("Card Write Was Successful", "The card was successfully written to. You will now be taken back to the main menu.", context, activity);
                writeSuccessfulAlertBox.Show();
            }
            catch (Exception e)
            {
                AlertBoxComponent cardFullAlertBox = new AlertBoxComponent(context);
                string alertMessage = "An error has occured. Please try again. Error Message:"+ e;
                cardFullAlertBox.Setup("Error Writing To Card", alertMessage);
                cardFullAlertBox.Show();
            }
        }

    }
}