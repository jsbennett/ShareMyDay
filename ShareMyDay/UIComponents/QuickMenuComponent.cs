using System;
using Android.App;
using Android.Content;
using Android.Widget;
using ShareMyDay.Activities;

namespace ShareMyDay.UIComponents
{
    public class QuickMenuComponent
    {
        private Activity _activity;
        private Context _context;

        public QuickMenuComponent(Activity activity, Context context)
        {
            _activity = activity;
            _context = context;
        }

        public void Show()
        {
            Button button = _activity.FindViewById<Button>(Resource.Id.quickMenuButton);
            PopupMenu quickMenu = new PopupMenu(_context, button);
            quickMenu.Inflate(Resource.Menu.TeacherQuickMenu);

            quickMenu.MenuItemClick += (s1, arg1) =>
            {
                Console.WriteLine("{0} selected", arg1.Item.TitleFormatted);
                switch (arg1.Item.TitleFormatted.ToString())
                {
                    case "Record Interaction":
                        Console.WriteLine("Case 1");
                        break;
                    case "Take A Picture":
                        var cameraIntent = new Intent(_context, typeof(CameraActivity));
                        cameraIntent.PutExtra("PreviousActivity", "QuickMenu");
                        _activity.StartActivity(cameraIntent);
                        break;
                    case "Take A Voice Recording":
                        var voiceRecordingIntent = new Intent(_context, typeof(VoiceRecordingActivity));
                        voiceRecordingIntent.PutExtra("PreviousActivity", "QuickMenu");
                        _activity.StartActivity(voiceRecordingIntent);
                        break;
                    case "Go To Main Menu":
                        var mainMenuIntent = new Intent(_context, typeof(TeacherMainMenuActivity));
                        _activity.StartActivity(mainMenuIntent);
                        break;
                }

            };

            quickMenu.DismissEvent += (s2, arg2) => { Console.WriteLine("menu dismissed"); };
            quickMenu.Show();
        }
    }
}