using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShareMyDay.Database.Models
{
    /*
     * Class Name: CardType
     * Purpose: To be the blueprint of a type of card 
     */
    class CardType
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }

        public string Type { get; set; }

        [OneToMany]	        
        public List<StoryEvent> StoryEvents { get; set; }
        
    }
}