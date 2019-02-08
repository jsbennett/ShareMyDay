using System;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using Android.Nfc.Tech;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ShareMyDay.Activities;
using AlertDialog = Android.App.AlertDialog;

namespace ShareMyDay
{
    /*
     * Class Name: Main Activity
     * Purpose: To be the main entry point for the app which controls the flow
     */
    
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private NFC.NFC _nfc;

        /*
         * Method Name: OnCreate
         * Purpose: It is used for when the app is loading 
         */
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Database.Database db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            db.Create();
            db.Setup();

            _nfc = new NFC.NFC(this);
        }

        /*
         * Method Name: OnResume
         * Purpose: This is for when the app has loaded
         */
        protected override void OnResume()
        {
            base.OnResume();
            _nfc.Detection(this,this);
        }

        /*
         * Method Name: OnNewIntent
         * Purpose: the action to be carried out when a NFC card is detected 
         */
        protected override void OnNewIntent(Intent intent)
        {
            
            //TO DO- move into own class + methods 
            Button button = FindViewById<Button>(Resource.Id.quickMenuButton);
            PopupMenu quickMenu = new PopupMenu (this,button);
            quickMenu.Inflate (Resource.Menu.TeacherQuickMenu);

            quickMenu.MenuItemClick += (s1, arg1) => {
                Console.WriteLine ("{0} selected", arg1.Item.TitleFormatted);
                switch (arg1.Item.TitleFormatted.ToString())
                {
                    case "Record Interaction":
                        Console.WriteLine("Case 1");
                        break;
                    case "Take A Picture":
                        var cameraIntent = new Intent(this, typeof(CameraActivity));
                        cameraIntent.PutExtra("PreviousActivity", "QuickMenu");
                        StartActivity(cameraIntent);
                        break;
                    case "Take A Voice Recording":
                        var voiceRecordingIntent = new Intent(this, typeof(VoiceRecordingActivity));
                        voiceRecordingIntent.PutExtra("PreviousActivity", "QuickMenu");
                        StartActivity(voiceRecordingIntent);
                        break;
                    case "Go To Main Menu":
                        Console.WriteLine("Case 3");
                        break;
                }
                
            };

            quickMenu.DismissEvent += (s2, arg2) => {
                Console.WriteLine ("menu dismissed");
            };
            quickMenu.Show ();
         
        }
    }
}

