using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShareMyDay.Database.Models
{
    /*
     * Class Name: Picture
     * Purpose: To be the blueprint of a picture 
     */
    public class Picture
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
       
        [ForeignKey(typeof(StoryEvent))]
        public int EventId { get; set; }

        public string Path { get; set; }
    }
}