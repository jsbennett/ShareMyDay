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
                if (inputBox.Text == "")
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder (this);
                    alert.SetTitle ("Blank Card Value");
                    alert.SetMessage ("Please enter a value for the card in the input box.");
                    alert.SetPositiveButton ("OK", (senderAlert, args) => {
                    });

                    alert.SetNegativeButton ("Cancel", (senderAlert, args) => {
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                else
                {
                    AlertDialog.Builder alert1 = new AlertDialog.Builder (this);
                    alert1.SetTitle ("Begin Writing Card");
                    alert1.SetMessage ("Please tap and hold the card on the back of the phone.");
                    alert1.SetPositiveButton ("OK", (senderAlert, args) => {
                    });

                    alert1.SetNegativeButton ("Cancel", (senderAlert, args) => {
                    });
                    dialog1 = alert1.Create();
                    dialog1.Show();

                    inputMessage = inputBox.Text;
                    _nfc.Detection(this,this);
                }

            };
        }

        protected override async void OnNewIntent(Intent intent)
        {
            dialog1.Dismiss();
            AlertDialog.Builder alert2 = new AlertDialog.Builder (this);
            alert2.SetTitle ("Card Detected");
            alert2.SetMessage ("A card has been detected. Please hold whilst the application attempts to write to the card. You might need to wiggle the card around the back of the phone for it to work.");
            alert2.SetPositiveButton ("OK", (senderAlert, args) => {
                Toast.MakeText(this ,"Deleted!" , ToastLength.Short).Show();
            });

            alert2.SetNegativeButton ("Cancel", (senderAlert, args) => {
            });

            Dialog dialog = alert2.Create();
            dialog.Show();
            var cardTag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
            if (cardTag != null)
            {
                var message1 = CreateCardContent();

                bool writeResult = await WriteMessage(cardTag, message1);

                // the tag was not written to, try formatting it with the message
                if (!writeResult)
                {
                    await FormatCard(cardTag, message1);
                }
            }
        }

        private static NdefMessage CreateCardContent()
        {
            var messageBytes = Encoding.UTF8.GetBytes(inputMessage);
            var mimeBytes = Encoding.UTF8.GetBytes("ShareMyDay");
            
            var ndefRecord = new NdefRecord(NdefRecord.TnfMimeMedia, mimeBytes, new byte[0], messageBytes);
            var ndefMessage = new NdefMessage(new[] {ndefRecord});

            return ndefMessage;
        }

        private async Task<bool> WriteMessage(Tag cardTag, NdefMessage cardContent)
        {
            try
            {
                AlertDialog.Builder alert2 = new AlertDialog.Builder (this);
                alert2.SetTitle ("Please Tap Card");
                alert2.SetMessage ("Please tap the card again.");
                alert2.SetPositiveButton ("OK", (senderAlert, args) => {
                    Toast.MakeText(this ,"Deleted!" , ToastLength.Short).Show();
                });

                alert2.SetNegativeButton ("Cancel", (senderAlert, args) => {
                    Toast.MakeText(this ,"Cancelled!" , ToastLength.Short).Show();
                });

                Dialog dialog = alert2.Create();
                dialog.Show();
                var ndefTagConnection = Ndef.Get(cardTag);
                if (ndefTagConnection == null)
                {
                    AlertDialog.Builder alert3 = new AlertDialog.Builder (this);
                    alert3.SetTitle ("Card Not Found");
                    alert3.SetMessage ("Please try tapping the card again.");
                    alert3.SetPositiveButton ("OK", (senderAlert, args) => {
                    });

                    alert3.SetNegativeButton ("Cancel", (senderAlert, args) => {
                    });

                    Dialog dialog3 = alert3.Create();
                    dialog3.Show();
                }

                await ndefTagConnection.ConnectAsync();
                if (!ndefTagConnection.IsWritable)
                {
                    AlertDialog.Builder alert4 = new AlertDialog.Builder (this);
                    alert4.SetTitle ("Card Unwritable");
                    alert4.SetMessage ("This card is read only. Please try a different card. ");
                    alert4.SetPositiveButton ("OK", (senderAlert, args) => {
                    });

                    alert4.SetNegativeButton ("Cancel", (senderAlert, args) => {
                    });

                    Dialog dialog4 = alert4.Create();
                    dialog4.Show();
                }
                var messageSize = cardContent.ToByteArray().Length;
                if (ndefTagConnection.MaxSize < messageSize)
                {
                    AlertDialog.Builder alert4 = new AlertDialog.Builder (this);
                    alert4.SetTitle ("Card Storage Exceeded");
                    alert4.SetMessage ("This card does not have enough storage. Please try a different card.");
                    alert4.SetPositiveButton ("OK", (senderAlert, args) => {
                    });

                    alert4.SetNegativeButton ("Cancel", (senderAlert, args) => {
                    });

                    Dialog dialog4 = alert4.Create();
                    dialog4.Show();
                }

                await ndefTagConnection.WriteNdefMessageAsync(cardContent);

                ndefTagConnection.Close();
                AlertDialog.Builder alert = new AlertDialog.Builder (this);
                alert.SetTitle ("Card Write Was Successful");
                alert.SetMessage ("The card was sucessfully written to. You will now be taken back to the main menu. ");
                alert.SetPositiveButton ("OK", (senderAlert, args) => {
                    Toast.MakeText(this ,"Deleted!" , ToastLength.Short).Show();
                    Intent mainMenu = new Intent(this, typeof(TeacherMainMenuActivity));
                    StartActivity(mainMenu);
                });

                Dialog dialog2 = alert.Create();
                dialog2.Show();
                
                return true;
            }
            catch (Exception ex)
            {
                AlertDialog.Builder alert4 = new AlertDialog.Builder (this);
                alert4.SetTitle ("Error Writing To Card");
                alert4.SetMessage ("An error has occured. Please try again");
                alert4.SetPositiveButton ("OK", (senderAlert, args) => {
                });

                alert4.SetNegativeButton ("Cancel", (senderAlert, args) => {
                });

                Dialog dialog4 = alert4.Create();
                dialog4.Show();
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
                    AlertDialog.Builder alert2 = new AlertDialog.Builder (this);
                    alert2.SetTitle ("Incorrect Card Type");
                    alert2.SetMessage ("This card does not support Ndef format. Please use a different card.");
                    alert2.SetPositiveButton ("OK", (senderAlert, args) => {
                    });

                    alert2.SetNegativeButton ("Cancel", (senderAlert, args) => {
                    });

                    Dialog dialog = alert2.Create();
                    dialog.Show();
                }
                await formatter.ConnectAsync();
                
                await formatter.FormatAsync(cardContent);
               
                formatter.Close();
                AlertDialog.Builder alert = new AlertDialog.Builder (this);
                alert.SetTitle ("Card Write Was Successful");
                alert.SetMessage ("The card was sucessfully written to. You will now be taken back to the main menu. ");
                alert.SetPositiveButton ("OK", (senderAlert, args) => {
                    Toast.MakeText(this ,"Deleted!" , ToastLength.Short).Show();
                    Intent mainMenu = new Intent(this, typeof(TeacherMainMenuActivity));
                    StartActivity(mainMenu);
                });

                Dialog dialog2 = alert.Create();
                dialog2.Show();
            }
            catch (Exception ex)
            {
                AlertDialog.Builder alert4 = new AlertDialog.Builder (this);
                alert4.SetTitle ("Error Writing To Card");
                alert4.SetMessage ("An error has occured. Please try again");
                alert4.SetPositiveButton ("OK", (senderAlert, args) => {
                });

                alert4.SetNegativeButton ("Cancel", (senderAlert, args) => {
                });

                Dialog dialog4 = alert4.Create();
                dialog4.Show();
            }
        }
    }
}