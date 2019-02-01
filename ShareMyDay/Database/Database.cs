using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ShareMyDay.Database.Models;
using SQLite;
using Environment = System.Environment;
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

        private readonly string _folderLocation = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private readonly string _dbName = "ShareMyDay.db3";
        
        /*
         * Method Name: CreateDatabase
         * Purpose: To create the database on the phone 
         */
        public void CreateDatabase()
        {
            string dbLocation = System.IO.Path.Combine (
                _folderLocation,_dbName);

            var db = new SQLiteConnection(dbLocation);
            db.CreateTable<CardType>();
            db.CreateTable<NFCEvent>();
            db.CreateTable<Picture>();
            db.CreateTable<VoiceRecording>();
            db.Close();
        }

        public void DatabaseDefaultSetup()
        {
            string dbLocation = System.IO.Path.Combine (
                _folderLocation,_dbName);

            Console.WriteLine("Location:" + dbLocation);
            var db = new SQLiteConnection(dbLocation);
            var type = new CardType();
            type.Type = "Item";
            Console.WriteLine(db.Insert(type));
            db.Close();
        }
    }
}