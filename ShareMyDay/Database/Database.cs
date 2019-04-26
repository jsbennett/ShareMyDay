using ShareMyDay.Database.Models;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Picture = ShareMyDay.Database.Models.Picture;

namespace ShareMyDay.Database
{
    /*
     * Class Name: Database
     * Purpose: To Control the SQLite database functionality

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
            if (!db.Table<CardType>().Any())
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

        /*
         * Method Name: FindFavouriteStory
         * Purpose: To find the story which has been indicated as the favourite story  
         */
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

        /*
         * Method Name: UpdateFavourite 
         * Purpose: To set a story to be the only favourite
         */
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

        /*
         * Method Name: RemoveFavourite
         * Purpose: To find the current fsvourite and remove it as the favourite story 
         */
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

        /*
         * Method Name: DeleteOldStories
         * Purpose: To delete the stories from the Story table 
         */
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
        
        /*
         * Method Name: InsertStories
         * Purpose: To insert a story into the story table 
         */
        public int InsertStories(List<StoryEvent> storyEventList, bool isExtraStory, bool isTextToSpeech, string defaultPicture)
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

            if (defaultPicture != null)
            {
                story.DefaultPicture = defaultPicture;
            }
            else
            {
                story.DefaultPicture = null; 
            }

            story.DateTime = DateTime.Now;
            story.Favourite = false; 
            db.UpdateWithChildren(story);

            db.Close();

            return count;
        }

        /*
         * Method Name: FindEventByValue
         * Purpose: To find an event by the value field
         */
        public StoryEvent FindEventByValue(string value)
        {
            var db = CreateConnection();
            var result = db.Query<StoryEvent>("SELECT * FROM StoryEvent WHERE Value == ?" , value );
            var storyEvent = db.GetWithChildren<StoryEvent>(result[0].Id);
            db.Close();
            return storyEvent; 
        }

        /*
         * Method Name: FindEventsFromStory
         * Purpose: To find the events which are assigned to a single story
         */
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

        /*
         * Method Name: FindStoryById
         * Purpose: To find a story by the id field
         */
        public Models.Story FindStoryById(string id)
        {
            var db = CreateConnection();
            var story = db.GetWithChildren<Models.Story>(id);
            db.Close();
            return story; 
        }

        /*
         * Method Name: GetAllStories
         * Purpose: To return all the stories from the story table 
         */
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

        /*
         * Method Name: GetUnfilteredEvents
         * Purpose: To get all the events from the table, including duplicates 
         */
        public List<StoryEvent> GetUnfilteredEvents()
        {
            var db = CreateConnection();
            var events = db.Query<StoryEvent>("SELECT * FROM StoryEvent WHERE DateTime >= ? AND DateTime <= ?" , new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day,0,0,0), new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day,23,59,59) ); 
            db.Close();
            return events;  
        }

        /*
         * Method Name: UpdateStories
         * Purpose: To update a list of stories  
         */
        public void UpdateStories(List<Models.Story> stories)
        {
            var db = CreateConnection();
            foreach (var i in stories)
            {
                db.Update(i);
            }
            db.Close();
        }

        /*
         * Method Name: UpdateStory
         * Purpose: To update a single story
         */
        public void UpdateStory(Models.Story story)
        {
            var db = CreateConnection();
            db.Update(story);
            db.Close();
        }

        /*
         * Method Name: GetMostPlayed
         * Purpose: To return the story that has been the most played
         */
        public Models.Story GetMostPlayed()
        {
            var stories = GetAllStories();
            Models.Story story = new Models.Story(); 
            if (stories != null && !stories.Count.Equals(0))
            {
                int mostPlayed = stories.Max(j => j.TimesPlayed);
                story = stories.FirstOrDefault(i => i.TimesPlayed.Equals(mostPlayed));
            }
            return story; 
        }
    }
}