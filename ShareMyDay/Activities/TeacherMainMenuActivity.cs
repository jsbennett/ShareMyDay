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
            Button editEventButton = FindViewById<Button>(Resource.Id.editEvents);
            Button takePictureButton = FindViewById<Button>(Resource.Id.takePicture);
            Button makeVoiceRecordingButton = FindViewById<Button>(Resource.Id.makeVoiceRecording);
            Button closeButton = FindViewById<Button>(Resource.Id.cancelButton);

            addNfcButton.Click += delegate
            {
                addNfcButton.SetBackgroundResource(Resource.Drawable.AddUpdateCardClicked);
                Intent addNfcIntent = new Intent(this, typeof(AddUpdateNFcCardActivity));
                StartActivity(addNfcIntent);
            };

            editEventButton.Click += delegate
            {
                editEventButton.SetBackgroundResource(Resource.Drawable.ViewEventsClicked);
                Intent editEventIntent = new Intent(this, typeof(EventListActivity));
                StartActivity(editEventIntent);
            };

            takePictureButton.Click += delegate
            {
                takePictureButton.SetBackgroundResource(Resource.Drawable.TakeAPictureButtonClicked);
                Intent takePictureIntent = new Intent(this, typeof(CameraActivity));
                takePictureIntent.PutExtra("PreviousActivity", "MainMenu");
                StartActivity(takePictureIntent);
            };

            makeVoiceRecordingButton.Click += delegate
            {
                makeVoiceRecordingButton.SetBackgroundResource(Resource.Drawable.MakeARecordingClicked);
                Intent makeVoiceRecordingIntent = new Intent(this, typeof(VoiceRecordingActivity));
                makeVoiceRecordingIntent.PutExtra("PreviousActivity", "MainMenu");
                StartActivity(makeVoiceRecordingIntent);
            };

            CancelButtonComponent cancelButton = new CancelButtonComponent(this);
            cancelButton.Get().Click += (o, e) => { 
                closeButton.SetBackgroundResource(Resource.Drawable.TeacherCloseClicked);
                cancelButton.Functionality("QuickMenu", this);
            };
        }

        /*
        * Method Name: OnResume
        * Purpose: This is for when the app has reloaded
        */
        protected override void OnResume()
        {
            base.OnResume();
            Button addNfcButton = FindViewById<Button> (Resource.Id.addNewCard);
            Button editEventButton = FindViewById<Button>(Resource.Id.editEvents);
            Button takePictureButton = FindViewById<Button>(Resource.Id.takePicture);
            Button makeVoiceRecordingButton = FindViewById<Button>(Resource.Id.makeVoiceRecording); 
            Button closeButton = FindViewById<Button>(Resource.Id.cancelButton);

            takePictureButton.SetBackgroundResource(Resource.Drawable.TakeAPictureButton);
            addNfcButton.SetBackgroundResource(Resource.Drawable.AddUpdateCard);
            editEventButton.SetBackgroundResource(Resource.Drawable.ViewEvents);
            makeVoiceRecordingButton.SetBackgroundResource(Resource.Drawable.MakeARecording);
            closeButton.SetBackgroundResource(Resource.Drawable.TeacherClose);
          
        }

    }
}