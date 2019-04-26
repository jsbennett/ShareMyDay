using Android.Content;

namespace ShareMyDay.Scheduling
{
    /*
     * Class Name: DeleteGenerationReceiver 
     * Purpose: When the alarm manager triggers at 8am, this is the functionality to carry out to delete stories
     */
    [BroadcastReceiver(Enabled = true)]
    class DeleteStoriesReceiver : BroadcastReceiver
    {
        /*
         * Method name: OnReceive
         * Purpose: To delete stories when the alarm manager is triggered
         */
        public override void OnReceive(Context context, Intent intent)
        {
           var _db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            _db.DeleteOldStories();
        }
    }
}