using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using ShareMyDay.Activities;

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
        private Database.Database _db;

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

            _db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            _db.Create();
            _db.Setup();
            

            _nfc = new NFC.NFC(this);

            Button todayStory = FindViewById<Button>(Resource.Id.latestStoryButton);
            Button favouriteStory = FindViewById<Button>(Resource.Id.favouriteStoryButton);

            todayStory.Click += delegate {
                Intent storyIntent = new Intent(this, typeof(TodayStoryActivity));
                StartActivity(storyIntent);
            };

            favouriteStory.Click += delegate
            {
                Intent storyIntent = new Intent(this, typeof(StoryActivity));
                storyIntent.PutExtra("Story", "Favourite");
                StartActivity(storyIntent);
            };
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
            _nfc.CheckCard(intent, this, this, _db);
            _db.FilterEvents();
        }
    }
}

