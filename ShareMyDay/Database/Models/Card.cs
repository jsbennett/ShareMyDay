using SQLite;

namespace ShareMyDay.Database.Models
{
    public class Card
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }
    }
}