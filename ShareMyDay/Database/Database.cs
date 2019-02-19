using System;
using System.Collections.Generic;
using System.Diagnostics;
using ShareMyDay.Database.Models;
using ShareMyDay.Story.Models;
using SQLite;
using Picture = ShareMyDay.Database.Models.Picture;

namespace ShareMyDay.Database
{
    /*
     * Class Name: Database
     * Purpose: To Control the SQLite database functionality
     * Created: 31/01/2019
     * Adapted from https://docs.microsoft.com/en-us/xamarin/android/data-cloud/data-access/configuration and https://docs.microsoft.com/en-us/xamarin/android/data-cloud/data-access/using-sqlite-orm
     */
    public class Database
    {
        private readonly string _dbLocation; 

        /*
         * Constructor
         * Takes in a location for the database and a name in order to create the path for the database
         */
        public Database(string location, string name)
        {
            string folderLocation = location;
            string dbName = name;
            _dbLocation = System.IO.Path.Combine (
                folderLocation,dbName);
            Console.WriteLine("Location:" + _dbLocation);
        }

        /*
         * Class Name: Create Connection
         * Purpose: To create the connection to the database
         */
        public SQLiteConnection CreateConnection()
        {
            var db = new SQLiteConnection(_dbLocation);
            return db; 
        }

        /*
         * Method Name: CreateDatabase
         * Purpose: To create the database on the phone 
         */
        public void Create()
        {
            var db = CreateConnection();
            db.CreateTable<CardType>();
            db.CreateTable<NfcEvent>();
            db.CreateTable<Picture>();
            db.CreateTable<Models.VoiceRecording>();
            db.Close();
        }

        /*
         * Method Name: DatabaseDefaultSetup
         * Purpose: To insert the default type values in the Card Type table
         * Return int: Used to check that all 6 items have been added to the database 
         */
        public int Setup()
        {
            var db = CreateConnection();
            CardType[] types = {new CardType{Type = "Item"},new CardType{Type = "Teacher"},new CardType{Type = "Friend"}, new CardType{Type = "Visitor"},new CardType{Type = "Class"},new CardType{Type = "Activity"} };
            var count = 0;
            if (db.Table<CardType>().Count() == 0)
            {
                foreach (var i in types)
                {
                    count += db.Insert(i);
                }
            }
            
            db.Close();
            return count; 
        }

        public int InsertEvent(string type, string value)
        {
            var db = CreateConnection();
            var nfcEvent = new NfcEvent();
            var table = db.Table<CardType>();
            int typeId = 0;
            bool typeFound = false;
            int count = 0; 
            foreach (var record in table)
            {
                if (record.Type == type)
                {
                    typeId = record.Id;
                    typeFound = true;
                    break; 
                }
            }

            if (typeFound)
            {
                nfcEvent.DateTime = DateTime.Now;
                nfcEvent.Value = value;
                nfcEvent.TypeId = typeId;
                count = db.Insert(nfcEvent);
            }

            var eventTable = db.Table<NfcEvent>();
            foreach (var i in eventTable)
            {
                Console.WriteLine(i.Value);
            }
            db.Close();
            return count; 
        }

        public List<NfcEvent> GetEvents()
        {
            var db = CreateConnection();
            var events = db.Query<NfcEvent>("SELECT * FROM NFcEvent WHERE DateTime >= ? AND DateTime <= ?" , new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day,0,0,0), new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day,23,59,59) ); 
            db.Close();
            return events;  
        }

        public List<NfcEvent> FilterEvents()
        {
            var events = GetEvents();
           
            for (int i = 0; i < events.Count; i++)
            {
               
                for (int j = 0; j < events.Count; j++)
                {
                    if (j != i)
                    {
                            if (events[i].Value.Equals(events[j].Value) && events[i].DateTime.Hour.Equals(events[j].DateTime.Hour))
                            {

                                events.Remove(events[j]);
                                j--;

                            }
                    
                    }
                    
                }
            }
            return events; 
        }
    }
}