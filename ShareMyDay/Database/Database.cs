using System;
using ShareMyDay.Database.Models;
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
            db.CreateTable<NFCEvent>();
            db.CreateTable<Picture>();
            db.CreateTable<VoiceRecording>();
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
            foreach (var i in types)
            {
                count += db.Insert(i);
            }
            db.Close();
            return count; 
        }
    }
}