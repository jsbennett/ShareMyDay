using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShareMyDay.Database.Models
{
    /*
     * Class Name: Type
     * Purpose: To be the blueprint of a type of card 
     * Created 31/01/2019
     */
    class CardType
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }

        public string Type { get; set; }

        [OneToMany]	        
        public List<StoryEvent> NFcEvents { get; set; }
        
    }
}