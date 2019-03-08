using ShareMyDay.Database.Models;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
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
            CardType[] types = {new CardType{Type = "Leisure Activity"}, new CardType{Type = "Class Activity"},new CardType{Type = "Class"}, new CardType{Type = "Item"},new CardType{Type = "Teacher"},new CardType{Type = "Friend"}, new CardType{Type = "Visitor"}, new CardType{Type = "Admin"} };
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
        public int InsertEvent(Boolean newEvent, StoryEvent storyEvent, Card card, Picture picture, List<Models.VoiceRecording> voiceRecording)
        {
            int count = 0; 
            var db = CreateConnection();
            if (newEvent)
            {
                count += db.Insert(storyEvent);
            }
            
            if (card != null)
            {
                count+= db.Insert(card);

                if (storyEvent.Cards==null || storyEvent.Cards.Count.Equals(0))
                {
                    storyEvent.Cards = new List<Card>{card};
                }
                else
                {
                    storyEvent.Cards.Add(card);
                }
            }

            if (picture != null)
            {
                count += db.Insert(picture);
                if (storyEvent.Pictures==null ||storyEvent.Pictures.Count.Equals(0))
                {
                    storyEvent.Pictures = new List<Picture>{picture};
                }
                else
                {
                    storyEvent.Pictures.Add(picture);
                }
            }

            if (voiceRecording != null)
            {
               
                foreach (var i in voiceRecording)
                {
                    count =+ db.Insert(i);
                }
                if (storyEvent.VoiceRecordings==null ||storyEvent.VoiceRecordings.Count.Equals(0))
                {
                    storyEvent.VoiceRecordings = new List<Models.VoiceRecording>();
                    foreach (var i in voiceRecording)
                    {
                        storyEvent.VoiceRecordings.Add(i);
                    }
                }
                else
                {
                    foreach (var i in voiceRecording)
                    {
                        storyEvent.VoiceRecordings.Add(i);
                    }
                }
            }
            db.UpdateWithChildren(storyEvent);
            
            db.Close();
            return count; 

        }

        public Models.Story FindFavouriteStory()
        {
            var db = CreateConnection();
            var favouriteStory = db.Query<Models.Story>("SELECT * FROM Story WHERE Favourite == ?", true);
            if (favouriteStory.Count == 0)
            {
                return null;
            }
            return favouriteStory[0];

        }

        public bool UpdateFavourite(int storyId)
        {
            var db = CreateConnection();
            var stories = db.Query<Models.Story>("SELECT * FROM Story");
            if (stories != null && stories.Count != 0)
            {
                foreach (var i in stories)
                {
                    if (i.Favourite)
                    {
                        i.Favourite = false; 
                    }
                    db.Update(i);
                }

                var newFavouriteStory = db.GetWithChildren<Models.Story>(storyId);
                newFavouriteStory.Favourite = true; 
                db.Update(newFavouriteStory);
                return true;
            }
            db.Close();
            return false; 
        }

        public bool RemoveFavourite(string id)
        {
            var db = CreateConnection();
            var story = db.GetWithChildren<Models.Story>(id);

            if (story != null)
            {
                story.Favourite = false; 
                db.Update(story);
                return true;
            }
            db.Close();
            return false;  
        }

        public void DeleteAllTableValues()
        {
            var db = CreateConnection();
            db.DeleteAll<Models.Story>();
            db.DeleteAll<StoryEvent>();
            db.DeleteAll<Card>();
            db.DeleteAll<Picture>();
            db.DeleteAll<Models.VoiceRecording>();
            db.Close();
        }

        public void DeleteOldStories()
        {
            var db = CreateConnection();
            List<Models.Story> stories = GetAllStories();
            foreach (var i in stories)
            {
                if (i.Favourite.Equals(false))
                {
                    db.Delete(i);
                }
            }
            db.Close();
        }
        
        public int InsertStories(List<StoryEvent> storyEventList, bool isExtraStory, bool isTextToSpeech)
        {
            int count = 0; 
            var db = CreateConnection();
            var story = new Models.Story();
            count += db.Insert(story);
            bool firstPicture = false;
            bool firstCard = false; 

            foreach (var i in storyEventList)
            {
                if (i.Pictures != null && i.Pictures.Count != 0 && !firstPicture)
                {
                    story.CoverPhoto = i.Pictures[0].Path;
                    firstPicture = true;
                }

                if (i.Cards != null && i.Cards.Count != 0 && !firstCard)
                {
                    story.TitleValue = i.Cards[0].Message;
                    firstCard = true;
                }

                i.StoryId = story.Id;
                i.InStory = true;
                db.UpdateWithChildren(i);
            }

            if (firstCard.Equals(false))
            {
                story.TitleValue = "This happened at school today...";
            }

            story.Events = storyEventList;
            if (isExtraStory)
            {
                story.Extra = true; 
            }

            if (isTextToSpeech)
            {
                story.TextToSpeech = true; 
            }
            story.DateTime = DateTime.Now;
            story.Favourite = false; 
            db.UpdateWithChildren(story);

            db.Close();

            return count;
        }

        public StoryEvent FindEventByValue(string value)
        {
            var db = CreateConnection();
            var result = db.Query<StoryEvent>("SELECT * FROM StoryEvent WHERE Value == ?" , value );
            var storyEvent = db.GetWithChildren<StoryEvent>(result[0].Id);
            db.Close();
            return storyEvent; 
        }
        public List<StoryEvent> FindEventsFromStory(string value)
        {
            var db = CreateConnection();
            var result = db.Query<StoryEvent>("SELECT * FROM StoryEvent WHERE StoryId == ?" , value );
            List<StoryEvent> events = new List<StoryEvent>();
            foreach (var i in result)
            {
               events.Add(db.GetWithChildren<StoryEvent>(i.Id));
            }
            db.Close();
            return events; 
        }

        public Models.Story FindStoryById(string id)
        {
            var db = CreateConnection();
            var story = db.GetWithChildren<Models.Story>(id);
            db.Close();
            return story; 
        }

        public List<Models.Story> GetAllStories()
        {
            var db = CreateConnection();
            var initalStories = db.Query<StoryEvent>("SELECT * FROM Story");
            List<Models.Story> stories = new List<Models.Story>();
            foreach (var i in initalStories)
            {
                stories.Add(db.GetWithChildren<Models.Story>(i.Id));

            }
           db.Close();
           return stories; 
        }

        public int UpdateEvent(StoryEvent storyEvent)
        {
            var db = CreateConnection();
            db.Update(storyEvent); 
            var result = db.Table<StoryEvent>().FirstOrDefault(x => x.Id == storyEvent.Id);
            Console.WriteLine(result.Value + " Pictures: " + result.Pictures.Count + " VCs: " + result.VoiceRecordings.Count);
            db.Close();
            return 1; 
        }

        public List<StoryEvent> GetUnfilteredEvents()
        {
            var db = CreateConnection();
            var events = db.Query<StoryEvent>("SELECT * FROM StoryEvent WHERE DateTime >= ? AND DateTime <= ?" , new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day,0,0,0), new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day,23,59,59) ); 
            db.Close();
            return events;  
        }

        //public List<StoryEvent> GetEvents()
        //{
        //    var events = GetUnfilteredEvents();
        //    if (events.Count != 0)
        //    {
        //        for (int i = 0; i < events.Count; i++)
        //        {
        //            for (int j = 0; j < events.Count; j++)
        //            {
        //                if (j != i)
        //                {
        //                    int limit;
        //                    if (events[i].DateTime.AddHours(1).Hour.Equals(events[i].DateTime.AddMinutes(10).Hour))
        //                    {
        //                        limit = 100 + events[i].DateTime.AddMinutes(10).Minute;
        //                    }
        //                    else
        //                    {
        //                        limit = events[i].DateTime.AddMinutes(10).Minute;
        //                    }
        //                    string[] outerLoopValues = events[i].Value.Split('-');
        //                    string[] innerLoopValues = events[j].Value.Split('-');
        //                    if(outerLoopValues[1].Equals("Card") && innerLoopValues[1].Equals("Card") &&
        //                       events[i].DateTime.Hour.Equals(events[j].DateTime.Hour) && events[j].DateTime.Minute >= events[i].DateTime.Minute && events[j].DateTime.Minute <= limit)
        //                    {
        //                        events.Remove(events[j]);
        //                        j--;
        //                    }
        //                }

        //            }
        //        }
        //    }

        //    foreach (var i in events)
        //    {
        //        Console.WriteLine(i.DateTime + " " + i.Value);
        //    }
        //    return events;
        //}

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