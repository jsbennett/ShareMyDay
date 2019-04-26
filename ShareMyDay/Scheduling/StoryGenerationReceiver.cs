using Android.Content;
using ShareMyDay.Story.StoryFunctions;

namespace ShareMyDay.Scheduling
{
    /*
     * Class Name: StoryGenerationReceiver 
     * Purpose: When the alarm manager triggers at 3pm, this is the functionality to carry out to generate stories
     */
    [BroadcastReceiver]
    class StoryGenerationReceiver : BroadcastReceiver
    {
        /*
         * Method name: OnReceive
         * Purpose: To generate stories when the alarm manager is triggered
         */
        public override void OnReceive(Context context, Intent intent)
        {
            var _db = new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            StoryGeneration storyGeneration = new StoryGeneration(_db, context);
            storyGeneration.Create();
        }
    }
}