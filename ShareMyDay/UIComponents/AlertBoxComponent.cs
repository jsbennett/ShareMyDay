using Android.App;
using Android.Content;
using ShareMyDay.Activities;

namespace ShareMyDay.UIComponents
{
    public class AlertBoxComponent
    {
        private readonly AlertDialog.Builder _alertBox;
        private Dialog _dialog;  

        public AlertBoxComponent(Context context)
        {
            _alertBox = new AlertDialog.Builder (context);
        }

        public void Setup(string title, string message)
        {
            _alertBox.SetTitle (title);
            _alertBox.SetMessage (message);
            _alertBox.SetPositiveButton ("OK", (senderAlert, args) => {
            });
            _alertBox.SetNegativeButton ("Cancel", (senderAlert, args) => {
            });
        }

        public void MenuOptionSetup(string title, string message, Context context, Activity activity)
        {
            _alertBox.SetTitle (title);
            _alertBox.SetMessage (message);
            _alertBox.SetPositiveButton ("OK", (senderAlert, args) => {
                Intent mainMenu = new Intent(context, typeof(TeacherMainMenuActivity));
                activity.StartActivity(mainMenu);
            });
        }

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

        public void Show()
        {
            _dialog = _alertBox.Create();
            _dialog.Show();
        }

        public Dialog GetDialog()
        {
            return _dialog;
        }

    }
}