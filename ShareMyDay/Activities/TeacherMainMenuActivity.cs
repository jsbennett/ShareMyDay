using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ShareMyDay.Activities
{
    [Activity(Label = "TeacherMainMenuActivity")]
    public class TeacherMainMenuActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TeacherMainMenu);

            //button names are closeButton, takePicture, makeVoiceRecording, addNewCard, updateCard

            Button addNfcButton = FindViewById<Button> (Resource.Id.addNewCard);

            addNfcButton.Click += delegate
            {
                Intent addNfcIntent = new Intent(this, typeof(AddNFcCardActivity) );
                StartActivity(addNfcIntent);
            };
        }
    }
}