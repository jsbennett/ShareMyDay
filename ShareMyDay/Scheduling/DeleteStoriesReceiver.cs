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

namespace ShareMyDay.Scheduling
{
    [BroadcastReceiver(Enabled = true)]
    class DeleteStoriesReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
           var _db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            _db.DeleteOldStories();
        }
    }
}