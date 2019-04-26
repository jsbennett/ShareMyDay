using Android.App;
using Android.Content;
using ShareMyDay.Activities;

namespace ShareMyDay.UIComponents
{
    /*
     * Class name: AlertBoxComponent
     * Purpose: To hold the logic for an alert box UI element 
     */
    public class AlertBoxComponent
    {
        private readonly AlertDialog.Builder _alertBox;
        private Dialog _dialog;  

        /*
         * Constructor
         * Sets the context for the alert box to be displayed on
         */
        public AlertBoxComponent(Context context)
        {
            _alertBox = new AlertDialog.Builder (context);
        }

        /*
         * Method name: Setup
         * Purpose: To set up the alert box with a title and message with basic ok and cancel buttons 
         */
        public void Setup(string title, string message)
        {
            _alertBox.SetTitle (title);
            _alertBox.SetMessage (message);
            _alertBox.SetPositiveButton ("OK", (senderAlert, args) => {
            });
            _alertBox.SetNegativeButton ("Cancel", (senderAlert, args) => {
            });
        }

        /*
         * Method name: MenuOptionSetup
         * Purpose: To set up the quick access menu options on the alert box 
         */
        public void MenuOptionSetup(string title, string message, Context context, Activity activity)
        {
            _alertBox.SetTitle (title);
            _alertBox.SetMessage (message);
            _alertBox.SetPositiveButton ("OK", (senderAlert, args) => {
                Intent mainMenu = new Intent(context, typeof(TeacherMainMenuActivity));
                activity.StartActivity(mainMenu);
            });
        }

        /*
         * Method name: RepeateFunctionSetup
         * Purpose: Used for the suggestion of a voice recording or picture alert box 
         */
        public void RepeateFunctionSetup<T>(string title, string message, Context context, Activity activity, string previousActivity)
        {
            _alertBox.SetTitle (title);
            _alertBox.SetMessage (message);
            _alertBox.SetPositiveButton ("Yes", (senderAlert, args) => {
                Intent repeatedActivity = new Intent(context, typeof(T));
                repeatedActivity.PutExtra("PreviousActivity", previousActivity);
                activity.StartActivity(repeatedActivity);
            });
            _alertBox.SetNegativeButton ("No", (senderAlert, args) => {});
            
        }

        /*
         * Method name: Show
         * Purpose: Used to display the alert box to the user 
         */
        public void Show()
        {
            _dialog = _alertBox.Create();
            _dialog.Show();
        }

        /*
         * Method name: GetDialog
         * Purpose:  to return the dialog for the alert box
         */
        public Dialog GetDialog()
        {
            return _dialog;
        }

    }
}