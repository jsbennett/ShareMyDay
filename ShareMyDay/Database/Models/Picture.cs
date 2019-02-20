using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShareMyDay.Database.Models
{
    /*
     * Class Name: Picture
     * Purpose: To be the blueprint of a picture 
     * Created 31/01/2019
     */
    public class Picture
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
       
        [ForeignKey(typeof(StoryEvent))]
        public int NfcEventId { get; set; }

        public string Path { get; set; }
    }
}