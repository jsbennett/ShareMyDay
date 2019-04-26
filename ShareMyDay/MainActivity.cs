using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using ShareMyDay.Activities;
using ShareMyDay.Scheduling;
using System;

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

            //scheduling time logic adapted from https://forums.xamarin.com/discussion/121773/scheduling-repeating-alarms
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day; 
            DateTime deleteStoriesTime = new DateTime(year, month, day, 08, 01,30);
            if (DateTime.Now > deleteStoriesTime)
            {
                deleteStoriesTime = deleteStoriesTime.AddDays(1); 
            }

            DateTime deleteStoriesUtcTime = TimeZoneInfo.ConvertTimeToUtc(deleteStoriesTime);
            double epochDate = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            long deleteTime = deleteStoriesUtcTime.AddSeconds(-epochDate).Ticks / 10000;
           
            var deleteIntent = new Intent(this, typeof(DeleteStoriesReceiver));
            var deletePendingIntent =
                PendingIntent.GetBroadcast(this, 0, deleteIntent, PendingIntentFlags.UpdateCurrent);
            var deleteAlertManager = (AlarmManager)GetSystemService(AlarmService);
            deleteAlertManager.SetInexactRepeating(AlarmType.RtcWakeup,deleteTime, AlarmManager.IntervalDay,deletePendingIntent);

            DateTime generateStoriesTime = new DateTime(year, month, day, 15, 01, 30);
            if (DateTime.Now > generateStoriesTime)
            {
                generateStoriesTime = generateStoriesTime.AddDays(1);
            }

            DateTime generateStoriesUtcTime = TimeZoneInfo.ConvertTimeToUtc(generateStoriesTime);

            long notifyTimeInInMilliseconds = generateStoriesUtcTime.AddSeconds(-epochDate).Ticks / 10000;

            var generateIntent = new Intent(this, typeof(StoryGenerationReceiver));
            var generatePendingIntent =
                PendingIntent.GetBroadcast(this, 0, generateIntent, PendingIntentFlags.UpdateCurrent);
            var generateAlertManager = (AlarmManager)GetSystemService(AlarmService);
            generateAlertManager.SetInexactRepeating(AlarmType.RtcWakeup, notifyTimeInInMilliseconds, AlarmManager.IntervalDay, generatePendingIntent);


            _nfc = new NFC.NFC(this);

            Button todayStory = FindViewById<Button>(Resource.Id.latestStoryButton);
            Button favouriteStory = FindViewById<Button>(Resource.Id.favouriteStoryButton);

            todayStory.SetBackgroundResource(Resource.Drawable.TodayStoryButton);
             todayStory.Click += delegate {
                todayStory.SetBackgroundResource(Resource.Drawable.TodayStoryButtonPressed);
                Intent storyIntent = new Intent(this, typeof(TodayStoryActivity));
                StartActivity(storyIntent);
                
            };

            favouriteStory.Click += delegate
            {
                favouriteStory.SetBackgroundResource(Resource.Drawable.FaveButtonPressed);
                Intent storyIntent = new Intent(this, typeof(StoryActivity));
                storyIntent.PutExtra("Story", "Favourite");
                StartActivity(storyIntent);
            };
        }

        /*
         * Method Name: OnResume
         * Purpose: This is for when the app has reloaded
         */
        protected override void OnResume()
        {
            base.OnResume();
            _nfc.Detection(this,this);
            Button todayStory = FindViewById<Button>(Resource.Id.latestStoryButton);
            todayStory.SetBackgroundResource(Resource.Drawable.TodayStoryButton);
            Button favouriteStory = FindViewById<Button>(Resource.Id.favouriteStoryButton);
            favouriteStory.SetBackgroundResource(Resource.Drawable.FaveButton);
        
        }

        /*
         * Method Name: OnNewIntent
         * Purpose: the action to be carried out when a NFC card is detected 
         */
        protected override void OnNewIntent(Intent intent)
        {
            _nfc.CheckCard(intent, this, this, _db);
            
        }
    }
}

