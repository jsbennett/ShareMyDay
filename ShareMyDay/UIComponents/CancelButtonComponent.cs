using Android.App;
using Android.Content;
using Android.Widget;
using ShareMyDay.Activities;

namespace ShareMyDay.UIComponents
{
    /*
     * Class name: CancelButtonComponent
     */
    public class CancelButtonComponent
    {
        private Button cancelButton;

        /*
         * Constructor
         * Used to find the cancel element on the UI to be able to attach functionality to 
         */
        public CancelButtonComponent(Activity activity)
        {
            cancelButton = activity.FindViewById<Button>(Resource.Id.cancelButton);
        }

        /*
         * Method name: Get
         * Purpose: To return the cancel button component
         */
        public Button Get()
        {
            return cancelButton;
        }

        /*
         * Method name: Functionality
         * Purpose: To be able to determine which activity to go back to when the cancel button is pressed
         */
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