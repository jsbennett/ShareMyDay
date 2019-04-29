using System;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Nfc;

namespace ShareMyDay.NFC.NFC_Functions
{
    /*
     * Class Name: NFcRead
     * Purpose: To control the reading of a NFC Card 
     */
    public class NFcRead
    {
        public string CardType { get; set; }
        public string CardValue { get; set; }

        /*
         * Method Name: GetData
         * Purpose: To get the data from the NFC card for processing 
         */
        public void GetData(Intent intent)
        {
            var cardMessage = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
            if (cardMessage != null)
            {
                foreach (var ndefMessage in cardMessage.Cast<NdefMessage>())
                {
                    foreach (var item in ndefMessage.GetRecords())
                    {
                        CardType = Encoding.UTF8.GetString(item.GetTypeInfo());
                        
                        CardValue = Encoding.UTF8.GetString(item.GetPayload());
                    }
                }
            }
        }

        /*
         * Method Name: SplitValue
         * Purpose: To split the value on the card in order to determine what to do with the information 
         */
        public string[] SplitValue()
        {
            string[] valueSections = CardValue.Split(':');
            return valueSections;
        }
    }
}