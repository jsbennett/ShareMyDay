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
using ShareMyDay.Database.Models;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShareMyDay.Database
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
        public List<NFCEvent> NFcEvents { get; set; }
        
    }
}