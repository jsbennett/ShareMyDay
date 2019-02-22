using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShareMyDay.Database.Models
{
    public class Story
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        
        public string Value { get; set; }
        
        public DateTime DateTime { get; set; }
       
        [OneToMany]	        
        public List<StoryEvent> Events { get; set; }

        public bool Favourite { get; set; }
    }
}