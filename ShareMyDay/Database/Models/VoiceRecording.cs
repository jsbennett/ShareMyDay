using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Security;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShareMyDay.Database.Models
{
    /*
     * Class Name: VoiceRecording
     * Purpose: To be the blueprint of a voice recording 
     * Created 31/01/2019
     */
    public class VoiceRecording
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
       
        [ForeignKey(typeof(NFCEvent))]
        public int NfcEventId { get; set; }

        public string Path { get; set; }
    }
}