using System;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using ShareMyDay.UIComponents;

namespace ShareMyDay.NFC.NFC_Functions
{
    public class NfcWrite
    {
        private readonly Context _context;

        private readonly Activity _activity;

        private readonly Dialog _tapCardAlertBox;

        private static string _inputMessage;

        private static string _typeSelected; 

        public NfcWrite(Context context, Activity activity, Dialog tapCardAlertBox, string inputMessage, string type)
        {
            _context = context;
            _activity = activity;
            _tapCardAlertBox = tapCardAlertBox;
            _inputMessage = inputMessage;
            _typeSelected = type; 
        }

         public async Task Write(Intent intent)
        {
            _tapCardAlertBox.Dismiss();
            AlertBoxComponent cardDetectionAlertBox = new AlertBoxComponent(_context);
            cardDetectionAlertBox.Setup("Card Detected", "A card has been detected. Please hold whilst the application attempts to write to the card. You might need to wiggle the card around the back of the phone for it to work.");
            cardDetectionAlertBox.Show();

            if (intent.GetParcelableExtra(NfcAdapter.ExtraTag) is Tag cardTag)
            {
                var cardContent = CreateCardContent();

                bool writeResult = await WriteMessage(cardTag, cardContent);

                if (!writeResult)
                {
                    await FormatCard(cardTag, cardContent);
                }
            }
        }

        private static NdefMessage CreateCardContent()
        {
            switch (_typeSelected)
            {
                case "Leisure Activity":
                    _typeSelected = "1";
                    break; 
                case "Class Activity":
                    _typeSelected = "2";
                    break; 
                case "Class":
                    _typeSelected = "3";
                    break; 
                case "Item":
                    _typeSelected = "4";
                    break; 
                case "Teacher":
                    _typeSelected = "5";
                    break; 
                case "Friend":
                    _typeSelected = "6";
                    break; 
                case "Visitor":
                    _typeSelected = "7";
                    break; 
            }
            var messageBytes = Encoding.UTF8.GetBytes(_typeSelected + ":" +_inputMessage);
            var mimeBytes = Encoding.UTF8.GetBytes("ShareMyDayTest");
            
            var ndefRecord = new NdefRecord(NdefRecord.TnfMimeMedia, mimeBytes, new byte[0], messageBytes);
            var ndefMessage = new NdefMessage(new[] {ndefRecord});

            return ndefMessage;
        }

        private async Task<bool> WriteMessage(Tag cardTag, NdefMessage cardContent)
        {
            try
            {
                var ndefTagConnection = Ndef.Get(cardTag);
                if (ndefTagConnection == null)
                {
                    AlertBoxComponent cardTagMissingAlertBox = new AlertBoxComponent(_context);
                    cardTagMissingAlertBox.Setup("Card Not Found", "Please try tapping the card again.");
                    cardTagMissingAlertBox.Show();
                }

                await ndefTagConnection.ConnectAsync();
                if (!ndefTagConnection.IsWritable)
                {
                    AlertBoxComponent cardUnwritableAlertBox = new AlertBoxComponent(_context);
                    cardUnwritableAlertBox.Setup("Card Unwritable", "This card is read only. Please try a different card.");
                    cardUnwritableAlertBox.Show();
                }
                
                var messageSize = cardContent.ToByteArray().Length;
                if (ndefTagConnection.MaxSize < messageSize)
                {
                    AlertBoxComponent cardFullAlertBox = new AlertBoxComponent(_context);
                    cardFullAlertBox.Setup("Card Storage Exceeded", "This card does not have enough storage. Please try a different card.");
                    cardFullAlertBox.Show();
                }

                await ndefTagConnection.WriteNdefMessageAsync(cardContent);
                ndefTagConnection.Close();
                
                AlertBoxComponent writeSuccessfulAlertBox = new AlertBoxComponent(_context);
                writeSuccessfulAlertBox.MenuOptionSetup("Card Write Was Successful", "The card was successfully written to. You will now be taken back to the main menu.", _context, _activity);
                writeSuccessfulAlertBox.Show();
                
                return true;
            }
            catch (Exception e)
            {
                AlertBoxComponent cardFullAlertBox = new AlertBoxComponent(_context);
                string alertMessage = "An error has occured. Please try again. Error Message:"+ e;
                cardFullAlertBox.Setup("Error Writing To Card", alertMessage);
                cardFullAlertBox.Show();
            }

            return false;
        }

        private async Task FormatCard(Tag cardTag, NdefMessage cardContent)
        {
            try
            {
                var formatter = NdefFormatable.Get(cardTag);
                if (formatter == null)
                {
                    AlertBoxComponent cardFullAlertBox = new AlertBoxComponent(_context);
                    cardFullAlertBox.Setup("Incorrect Card Type", "This card does not support Ndef format. Please use a different card.");
                    cardFullAlertBox.Show();
                }
                await formatter.ConnectAsync();
                await formatter.FormatAsync(cardContent);
                formatter.Close();
                
                AlertBoxComponent writeSuccessfulAlertBox = new AlertBoxComponent(_context);
                writeSuccessfulAlertBox.MenuOptionSetup("Card Write Was Successful", "The card was successfully written to. You will now be taken back to the main menu.", _context, _activity);
                writeSuccessfulAlertBox.Show();
            }
            catch (Exception e)
            {
                AlertBoxComponent cardFullAlertBox = new AlertBoxComponent(_context);
                string alertMessage = "An error has occured. Please try again. Error Message:"+ e;
                cardFullAlertBox.Setup("Error Writing To Card", alertMessage);
                cardFullAlertBox.Show();
            }
        }



    }
}