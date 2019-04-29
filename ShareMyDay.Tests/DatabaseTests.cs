using NUnit.Framework;
using ShareMyDay.Database.Models;
using ShareMyDay.Story.StoryFunctions;
using System;
using System.Collections.Generic;

namespace ShareMyDay.Tests
{
    /*
     * This Class is used to test the database functionality  of the application - Please note that only the main parts of the functionality is tested, with more time the author would have done more comprehensive testing 
     */
    [TestFixture]
    public class DatabaseTests
    {
        private Database.Database _db;

        [SetUp]
        public void Setup()
        {
            _db = new Database.Database(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
        }

        /*
         * Method Name: CreateConnection_CreatesDatabaseObject_ReturnsObject
         * Purpose: This test is used to test whether a connection is made with the connection details
         * Passes if the value returned from CreateConnection() is not null
         */
        [Test]
        public void A_CreateConnection_CreatesDatabaseObject_ReturnsObject()
        {
            Assert.IsNotNull(_db.CreateConnection(),"CreateConnection_CreatesDatabaseObject_ReturnsObject: Null Returned");
         
        }

       /*
       * Method Name: InsertEvent_InsertEventsCorrectly_ReturnsACountOf1
       * Purpose: To test that it inserts events into the database
       */
        [Test]
        public void B_InsertEvent_InsertEventsCorrectly_ReturnsACountOf1()
        {
            StoryEvent storyEvent = new StoryEvent
            {
                Value = DateTime.Now.ToLongTimeString() +  "-" + "Picture Taken",
                DateTime = DateTime.Now,
                Finished = true
            };

            Card card = new Card
            {
                Message = "test",
                StoryEventId = storyEvent.Id,
                Type = "1"
            };
            
            Picture picture =  new Picture
            {
                Path="Test",
                EventId = storyEvent.Id
            };

            List<Database.Models.VoiceRecording> voiceRecordings = new List<Database.Models.VoiceRecording>
            {
                new Database.Models.VoiceRecording
                {
                    Path="Test",
                    EventId = storyEvent.Id
                }
            };

            _db.CreateConnection();
            _db.Create();
            var expected = 1;
            var actual = _db.InsertEvent(true, storyEvent, card, picture, voiceRecordings); 
            Assert.AreEqual(expected, actual, "InsertEvent_InsertsPictureEventCorrectly_ReturnsACountOf1: Did not return a 1 to indicate a picture was added");
        }

       /*
       * Method Name: GetAllStories_GeneratesStory_ReturnsNotNull
       * Purpose: To test that it makes and returns not null i.e it found stories 
       */
        [Test]
        public void C_GetAllStories_GeneratesStory_ReturnsNotNull()
        {
            StoryEvent storyEvent = new StoryEvent
            {
                Value = DateTime.Now.ToLongTimeString() +  "-" + "Picture Taken",
                DateTime = DateTime.Now,
                Finished = true
            };

            Card card = new Card
            {
                Message = "test",
                StoryEventId = storyEvent.Id,
                Type = "1"
            };
            
            Picture picture =  new Picture
            {
                Path="Test",
                EventId = storyEvent.Id
            };

            List<Database.Models.VoiceRecording> voiceRecordings = new List<Database.Models.VoiceRecording>
            {
                new Database.Models.VoiceRecording
                {
                    Path="Test",
                    EventId = storyEvent.Id
                }
            };

            _db.CreateConnection();
            _db.Create();
            _db.InsertEvent(true, storyEvent, card, picture, voiceRecordings); 
            StoryGeneration generator = new StoryGeneration(_db, null);
            generator.Create();
            Assert.IsNotNull(_db.GetAllStories(), "GetAllStories_GeneratesStory_ReturnsOneStory: Returned Null - No stories were made");
        }

      /*
      * Method Name: FindStoryById_FindStoryWithId1_ReturnsNotNull
      * Purpose: To test that it returns not null i.e it found story with  id 1
      */
        [Test]
        public void D_FindStoryById_FindStoryWithId1_ReturnsNotNull()
        {
            StoryEvent storyEvent = new StoryEvent
            {
                Value = DateTime.Now.ToLongTimeString() +  "-" + "Picture Taken",
                DateTime = DateTime.Now,
                Finished = true
            };

            Card card = new Card
            {
                Message = "test",
                StoryEventId = storyEvent.Id,
                Type = "1"
            };
            
            Picture picture =  new Picture
            {
                Path="Test",
                EventId = storyEvent.Id
            };

            List<Database.Models.VoiceRecording> voiceRecordings = new List<Database.Models.VoiceRecording>
            {
                new Database.Models.VoiceRecording
                {
                    Path="Test",
                    EventId = storyEvent.Id
                }
            };

            _db.CreateConnection();

            _db.Create();
            _db.InsertEvent(true, storyEvent, card, picture, voiceRecordings); 
            StoryGeneration generator = new StoryGeneration(_db, null);
            generator.Create();
            Assert.IsNotNull(_db.FindStoryById("1"), "FindStoryById_FindStoryWithId1_ReturnsNotNull: Returned Null - No story with Id 1 not found");
        }
        
      /*
      * Method Name: FindEventsFromStory_FindEventsWhichAreInStoryWithId0_ReturnsListOfStoryEvents
      * Purpose: To test that it returns events from story with id 1
      */
        [Test]
        public void E_FindEventsFromStory_FindEventsWhichAreInStoryWithId1_ReturnsListOfStoryEvents()
        {
            _db.CreateConnection();
            _db.Create();
            Assert.IsInstanceOfType(typeof(List<StoryEvent>),_db.FindEventsFromStory("1"), "FindEventsFromStory_FindEventsWhichAreInStoryWithId1_ReturnsListOfStoryEvents: Did not return a list of events for story with id 1");
        }

        /*
        * Method Name: GetUnfilteredEvents_GetAll_ReturnsObjectOfStoryEventType
        * Purpose: To test that it returns a list of all unfiltered story events
         */
        [Test]
        public void F_GetUnfilteredEvents_GetAll_ReturnsAListOfStoryEventType()
        {
            _db.CreateConnection();
            _db.Create();
            Assert.IsInstanceOfType(typeof(List<StoryEvent>), _db.GetUnfilteredEvents(),
                "GetUnfilteredEvents_GetAll_ReturnsAListOfStoryEventType: Did not return List of all StoryEvents");
            foreach (var x in _db.GetUnfilteredEvents())
            {
                Console.WriteLine("Event Value:" + x.Value);
            }
        }

        /*
        * Method Name: UpdateFavourite_SetStoryWithId1ToFavourite_ReturnsTrue
        * Purpose: To test that it sets story with Id 1 to be the favourite story 
         */
        [Test]
        public void G_UpdateFavourite_SetStoryWithId1ToFavourite_ReturnsTrue()
        {
            _db.CreateConnection();
            Assert.IsTrue(_db.UpdateFavourite(1), "UpdateFavourite_SetStoryWithId1ToFavourite_ReturnsTrue: Did not set story with Id 1 To favourite");
        }

        /*
        * Method Name: FindFavouriteStory_ReturnsStoryWithId1_ReturnsStoryWithId1
        * Purpose: To test that it returns the story with Id 1 since its the favourite story 
         */
        [Test]
        public void H_FindFavouriteStory_FindsTheFavouriteStory_ReturnsStoryWithId1()
        {
            _db.CreateConnection();
            int expected = 1;
            var actual = _db.FindFavouriteStory().Id;
            Assert.AreEqual(expected, actual, "FindFavouriteStory_ReturnsStoryWithId1_ReturnsStoryWithId1: Did not return story with Id 1");
        }

        /*
        * Method Name: RemoveFavourite_RemovesCurrentFavouriteStory_ReturnsTrue
        * Purpose: To test that it removes the current favourite story and returns true when it does so
        */
        [Test]
        public void I_RemoveFavourite_RemovesCurrentFavouriteStory_ReturnsTrue()
        {
            _db.CreateConnection();
            Assert.IsTrue(_db.RemoveFavourite("1"), "RemoveFavourite_RemovesCurrentFavouriteStory_ReturnsTrue: Did not remove favourite story with id 1");
        }

        /*
        * Method Name: FindEventByValue_FindsASpecificEvent_ReturnsMathchingEvent
        * Purpose: To test that it finds a specific event and returns it correctly 
        */
        [Test]
        public void J_FindEventByValue_FindsASpecificEvent_ReturnsMathchingEvent()
        {
            StoryEvent storyEvent = new StoryEvent
            {
                Value = DateTime.Now.ToLongTimeString() +  "-" + "Card Tapped",
                DateTime = DateTime.Now,
                Finished = true
            };
            
            Card card = new Card
            {
                Message = "test",
                StoryEventId = storyEvent.Id,
                Type = "1"
            };

            var value = storyEvent.Value;
            _db.InsertEvent(true, storyEvent, card, null, null);
            var foundEvent = _db.FindEventByValue(value);
            Assert.AreEqual(value,foundEvent.Value, "FindEventByValue_FindsASpecificEvent_ReturnsMathchingEvent: Did not find event");
        }
        


    }
}
