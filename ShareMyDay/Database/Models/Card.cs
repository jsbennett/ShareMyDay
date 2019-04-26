using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShareMyDay.Database.Models
{
    /*
     * Class Name: Card
     * Purpose: To be the blueprint of a card
     */
    public class Card
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(StoryEvent))]
        public int StoryEventId { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }
    }
}