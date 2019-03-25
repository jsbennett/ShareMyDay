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
using ShareMyDay.Story.StoryFunctions;

namespace ShareMyDay.Scheduling
{
    [BroadcastReceiver]
    class StoryGenerationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var _db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            StoryGeneration storyGeneration = new StoryGeneration(_db, context);
            storyGeneration.Create();
        }
    }
}