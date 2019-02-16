using Android.App;
using Android.Content;
using Android.Widget;
using ShareMyDay.Activities;

namespace ShareMyDay.UIComponents
{
    public class CancelButtonComponent
    {
        private Button cancelButton;

        public CancelButtonComponent(Activity activity)
        {
            cancelButton = activity.FindViewById<Button>(Resource.Id.cancelButton);
        }

        public Button Get()
        {
            return cancelButton;
        }

        public void Functionality(string previousActivity, Context context)
        {
            if (previousActivity == "QuickMenu")
            {
                Toast.MakeText (context, "Back to homepage", ToastLength.Short).Show ();
                var childMenu = new Intent(context, typeof(MainActivity));
                context.StartActivity(childMenu);
            }
            else
            {
                Toast.MakeText (context, "Back to main menu", ToastLength.Short).Show ();
                var mainMenu = new Intent(context, typeof(TeacherMainMenuActivity));
                context.StartActivity(mainMenu);
            }
        } 
    }
}