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
            db.CreateTable<Models.Story>();
            db.CreateTable<StoryEvent>();
            db.CreateTable<Card>();
            db.CreateTable<Picture>();
            db.CreateTable<Models.VoiceRecording>();
            db.CreateTable<CardType>();
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
            CardType[] types = {new CardType{Type = "Leisure Activity"}, new CardType{Type = "Class Activity"},new CardType{Type = "Class"}, new CardType{Type = "Item"},new CardType{Type = "Teacher"},new CardType{Type = "Friend"}, new CardType{Type = "Visitor"} };
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

        /*
         * Method Name: InsertEvent
         * Purpose: To insert an event into the event table 
         */
        public int InsertEvent(StoryEvent storyEvent)
        {
            var db = CreateConnection();
            
            //var table = db.Table<CardType>();
            //int typeId = 0;
            //bool typeFound = false;
            //int count = 0; 
            //foreach (var record in table)
            //{
            //    if (record.Type == type)
            //    {
            //        typeId = record.Id;
            //        typeFound = true;
            //        break; 
            //    }
            //}

            //if (typeFound)
            //{
            //    nfcEvent.DateTime = DateTime.Now;
            //    nfcEvent.Value = value;
            //    nfcEvent.TypeId = typeId;
            //    count = db.Insert(nfcEvent);
            //}

            //var eventTable = db.Table<NfcEvent>();
            //foreach (var i in eventTable)
            //{
            //    Console.WriteLine(i.Value  + " "+ i.TypeId);
            //}
            //db.Close();
            var count = db.Insert(storyEvent);
            return count; 
        }

        

        public List<StoryEvent> GetEvents()
        {
            var db = CreateConnection();
            var events = db.Query<StoryEvent>("SELECT * FROM StoryEvent WHERE DateTime >= ? AND DateTime <= ?" , new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day,0,0,0), new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day,23,59,59) ); 
            db.Close();
            return events;  
        }

        public List<StoryEvent> GetFilteredEvents()
        {
           var events = GetEvents();
           if(events.Count !=0){
               for (int i = 0; i < events.Count; i++)
               {
                   for (int j = 0; j < events.Count; j++)
                   {
                       if (j != i)
                       {
                           var timeLimit = events[i].DateTime.AddMinutes(10);
                           int limit;
                           if (events[i].DateTime.AddHours(1).Hour.Equals(timeLimit.Hour))
                           {
                               limit = 100 + timeLimit.Minute; 
                           }
                           else
                           {
                               limit = timeLimit.Minute;
                           }
                           if (events[i].Value.Equals(events[j].Value) &&
                               events[i].DateTime.Hour.Equals(events[j].DateTime.Hour) && events[j].DateTime.Minute >= events[i].DateTime.Minute && events[j].DateTime.Minute <= limit)
                           {
                               events.Remove(events[j]);
                               j--;
                           }
                       }

                   }
               }
           }

            foreach (var i in events)
            {
                Console.WriteLine(i.DateTime+ " " + i.Value);
            }
            return events; 
        }

        //public void EventGrouping()
        //{
        //    var events = GetFilteredEvents();
        //    Dictionary<StoryEvent,List<StoryEvent>> eventGroups = new Dictionary<StoryEvent, List<StoryEvent>>();
            
        //    foreach (var i in events)
        //    {
        //        List<StoryEvent> partsOfEvent = new List<StoryEvent>();
        //        if (i.TypeId.Equals(3))
        //        {
        //            foreach (var j in events)
        //            {
        //                if (j.TypeId.Equals(1) || j.TypeId.Equals(2) || j.TypeId.Equals(4) ||
        //                    j.TypeId.Equals(5) || j.TypeId.Equals(6) || j.TypeId.Equals(7) && j.DateTime.Hour.Equals(i.DateTime.Hour))
        //                {
        //                    partsOfEvent.Add(j);
        //                }
        //            }
        //            eventGroups.Add(i,partsOfEvent);
        //        }
        //    }

        //    foreach (var i in eventGroups)
        //    {
        //        if (i.Key.TypeId.Equals(3))
        //        {
        //            Console.WriteLine("class or activity: " + i.Key.Value);

        //            if (i.Value.Count.Equals(0))
        //            {
        //                Console.WriteLine("No Assisting Activities");
        //            }
        //            else
        //            {
        //                foreach (var j in i.Value)
        //                {
        //                    Console.WriteLine("Has additional events: " + j.Value);
        //                }
        //            }
        //        }
        //    }
        //}
    }
}