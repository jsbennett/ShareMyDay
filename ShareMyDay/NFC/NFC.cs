using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using ShareMyDay.UIComponents;
using System;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using ShareMyDay.NFC.NFC_Functions;


namespace ShareMyDay.NFC
{
    /*
     * Class Name: NFC
     * Purpose: To set up and control NFC card interactions with the phone
     *  NFC Code adapted from https://www.patrickvankleef.com/2017/01/08/xamarin-near-field-communication/, https://github.com/patkleef/XamarinNFC/blob/master/MainActivity.cs
     * and Xamarin Mobile Development for Android Cookbook by Matthew Leibowitz page 154 - 159
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

        public async Task Write(Context context, Activity activity, Intent intent)
        {
            NfcWrite writer = new NfcWrite(context, activity,_tapCardAlertBox.GetDialog(), _inputMessage );
            await writer.Write(intent);
        }

       

    }
}