using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using ShareMyDay.UIComponents;

namespace ShareMyDay.Activities
{
    [Activity(Label = "TeacherMainMenuActivity")]
    public class TeacherMainMenuActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TeacherMainMenu);

            Button addNfcButton = FindViewById<Button> (Resource.Id.addNewCard);
            Button updateNfcButton = FindViewById<Button>(Resource.Id.updateCard);
            Button takePictureButton = FindViewById<Button>(Resource.Id.takePicture);
            Button makeVoiceRecordingButton = FindViewById<Button>(Resource.Id.makeVoiceRecording); 
            

            addNfcButton.Click += delegate
            {
                Intent addNfcIntent = new Intent(this, typeof(AddUpdateNFcCardActivity));
                StartActivity(addNfcIntent);
            };

            updateNfcButton.Click += delegate
            {
                Intent updateNfcIntent = new Intent(this, typeof(AddUpdateNFcCardActivity));
                StartActivity(updateNfcIntent);
            };

            takePictureButton.Click += delegate
            {
                Intent takePictureIntent = new Intent(this, typeof(CameraActivity));
                takePictureIntent.PutExtra("PreviousActivity", "MainMenu");
                StartActivity(takePictureIntent);
            };

            makeVoiceRecordingButton.Click += delegate
            {
                Intent makeVoiceRecordingIntent = new Intent(this, typeof(VoiceRecordingActivity));
                makeVoiceRecordingIntent.PutExtra("PreviousActivity", "MainMenu");
                StartActivity(makeVoiceRecordingIntent);
            };

            CancelButton cancelButton = new CancelButton(this);
            cancelButton.Get().Click += (o, e) => { cancelButton.Functionality("QuickMenu", this); };
        }
    }
}