using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShareMyDay.Database.Models
{
    /*
     * Class Name: VoiceRecording
     * Purpose: To be the blueprint of a voice recording 
     */
    public class VoiceRecording
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
       
        [ForeignKey(typeof(StoryEvent))]
        public int EventId { get; set; }

        public string Path { get; set; }
    }
}