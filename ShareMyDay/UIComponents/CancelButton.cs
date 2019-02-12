using Android.App;
using Android.Content;
using Android.Widget;

namespace ShareMyDay.UIComponents
{
    public class CancelButton
    {
        private Button cancelButton;

        public CancelButton(Activity activity)
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
        } 
    }
}