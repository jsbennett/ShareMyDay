using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace ShareMyDay.Database.Models
{
    /*
     * Class Name: NFCEvent
     * Purpose: To be the blueprint of a NFC table entry
     * Created 31/01/2019
     */
    public class NfcEvent
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(CardType))]
        public int TypeId { get; set; }
        
        public string Value { get; set; }
        public DateTime DateTime { get; set; }

        [OneToMany]	        
        public List<Picture> Pictures { get; set; }
        [OneToMany]	        
        public List<VoiceRecording> VoiceRecordings { get; set; }
    }
}