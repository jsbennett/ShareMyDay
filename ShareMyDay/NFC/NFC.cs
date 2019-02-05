using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Nfc;


namespace ShareMyDay.NFC
{
    /*
     * Class Name: NFC
     * Purpose: To set up and control NFC card interactions with the phone
     *
     */
    class NFC
    {
        private NfcAdapter NfcAdapter;

        /*
         * Constructor
         * Used to get the adapter of the current Activity context
         */
        public NFC(Context context)
        {
            NfcAdapter = NfcAdapter.GetDefaultAdapter(context);
        }

        /*
         * Method name: NfcCardDetection
         * Purpose: To detect when an NFC card is tapped on the phone
         * NFC Code adapted from https://www.patrickvankleef.com/2017/01/08/xamarin-near-field-communication/, https://github.com/patkleef/XamarinNFC/blob/master/MainActivity.cs
         * and Xamarin Mobile Development for Android Cookbook by Matthew Leibowitz page 154 - 159
         */
        public void NfcCardDetection(Context context, Activity activity)
        {
            //because we do not know what tag is discovered, an implicit intent is used 
            var detectedTag = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            
            var intentTagFilters = new[] { detectedTag };
            
            var contextIntent = new Intent(context, context.GetType()).AddFlags(ActivityFlags.SingleTop);

            //create an intent to return to activity 
            var pendingContextIntent = PendingIntent.GetActivity(context, 0, contextIntent, 0);
            
            //used to listen for nfc cards
            NfcAdapter.EnableForegroundDispatch(activity, pendingContextIntent, intentTagFilters, null);
        }

    }
}