using System;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Nfc;

namespace ShareMyDay.NFC.NFC_Functions
{
    public class NFcRead
    {
        public string CardType { get; set; }
        public string CardValue { get; set; }

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

        public string[] SplitValue()
        {
            string[] valueSections = CardValue.Split(':');
            return valueSections;
        }
    }
}