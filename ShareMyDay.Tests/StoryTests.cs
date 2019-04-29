using NUnit.Framework;
using ShareMyDay.Database.Models;
using ShareMyDay.Story.StoryFunctions;
using System;
using System.Collections.Generic;

namespace ShareMyDay.Tests
{
    /*
     * This Class is used to test the Story functionality  of the application - Please note that only the main parts of the functionality is tested, with more time the author would have done more comprehensive testing 
     */
    [TestFixture]
    public class StoryTests
    {
        private Database.Database _db;
        private StoryGeneration _storyGenerator; 


        [SetUp]
        public void Setup()
        {
            _db = new Database.Database(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3");
            _db.Create();
            _storyGenerator = new StoryGeneration(_db,null);
        }

        /*
         * Method Name: CalculateTimeLimit_CalculateTenMinuteTimePeriod_ReturnsInteger
         * Purpose: This test is used to test whether a 10 minute time period is returned 
         */
        [Test]
        public void CalculateTimeLimit_CalculateTenMinuteTimePeriod_ReturnsInteger()
        {
            StoryEvent storyEvent = new StoryEvent
            {
                Value = DateTime.Now.ToLongTimeString() +  "-" + "Card Tapped",
                DateTime = DateTime.Now,
                Finished = true
            };

            Assert.IsInstanceOfType(typeof(int),_storyGenerator.CalculateTimeLimit(storyEvent),"CalculateTimeLimit_CalculateTenMinuteTimePeriod_ReturnsInteger: Integer returned");
         
        }

        /*
         * Method Name: FindDefaultPicture_FindThePictureFileName_ReturnsCorrectString
         * Purpose: This test is used to test whether it returns the 
         */
        [Test]
        public void FindDefaultPicture_FindThePictureFileName_ReturnsCorrectString()
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
            var expected = "leisureCard";
            var actual = _storyGenerator.FindDefaultPicture(foundEvent);
            Assert.AreEqual(expected,actual,"FindDefaultPicture_FindThePictureFileName_ReturnsCorrectString: Incorrect default picture returned");
         
        }

        /*
       * Method Name: GetAllStories_GeneratesStory_ReturnsNotNull
       * Purpose: To test that it makes stories
       */
        [Test]
        public void GenerateStory_FindsWhetherStoryWasGenerated_ReturnsNotNull()
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
            _db.InsertEvent(true, storyEvent, card, picture, voiceRecordings); 
            _storyGenerator.Create();
            Assert.IsNotNull(_db.GetAllStories(), "GenerateStory_FindsWhetherStoryWasGenerated_ReturnsNotNull: Did not return stories");
        }

       
    }
}
