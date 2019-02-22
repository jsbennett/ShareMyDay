using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShareMyDay.Database.Models
{
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