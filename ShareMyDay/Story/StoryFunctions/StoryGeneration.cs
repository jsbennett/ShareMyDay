using System;
using System.Collections.Generic;
using Android.Views.Accessibility;
using ShareMyDay.Database.Models;

namespace ShareMyDay.Story.StoryFunctions
{
    public class StoryGeneration
    {
        private Database.Database _db; 
        public StoryGeneration(Database.Database db)
        {
            _db = db;
        }


        public void Create()
        {
            var storyEvents = GetEvents();
            List<Database.Models.Story> stories = new List<Database.Models.Story>();
            List<StoryEvent> finalEvents = new List<StoryEvent>();
            for (var i = 0; i < storyEvents.Count; i++)
            {
                //if only cards
                if ((storyEvents[i].Pictures == null || storyEvents[i].Pictures.Count.Equals(0)) && (storyEvents[i].VoiceRecordings == null || storyEvents[i].VoiceRecordings.Count.Equals(0)) && storyEvents[i].Cards!=null)
                {
                    if (storyEvents[i].Cards.Count != 0)
                    {
                        //need to loop through the rest to find if there are any voice recordings or pictures for the card to join with within 10 minutes from i.datetime
                        //if there is none then it gets added as its own story 
                    }

                }

                //if only pictures
                if ((storyEvents[i].Cards == null || storyEvents[i].Cards.Count.Equals(0)) && (storyEvents[i].VoiceRecordings == null || storyEvents[i].VoiceRecordings.Count.Equals(0)) && storyEvents[i].Pictures
                    !=null)
                {
                    if (storyEvents[i].Pictures.Count != 0)
                    {
                        //need to loop through the rest to find if there are any voice recordings or cards for the card to join with within 10 minutes from i.datetime
                        //if there is none then it gets added as its own story 
                    }

                }

                //if only voice recordings
                if ((storyEvents[i].Cards == null || storyEvents[i].Cards.Count.Equals(0)) && (storyEvents[i].Pictures == null || storyEvents[i].Pictures.Count.Equals(0)) && storyEvents[i].VoiceRecordings
                    !=null)
                {
                    if (storyEvents[i].VoiceRecordings.Count != 0)
                    {
                        //need to loop through the rest to find if there are any pictures or cards for the card to join with within 10 minutes from i.datetime
                        //if there is none then it gets added as its own story 
                    }

                }

                //if only pictures and voice recording
                if ((storyEvents[i].Pictures != null && !storyEvents[i].Pictures.Count.Equals(0)) && (storyEvents[i].VoiceRecordings != null && !storyEvents[i].VoiceRecordings.Count.Equals(0)) && (storyEvents[i].Cards ==null || storyEvents[i].Cards.Count.Equals(0)))
                {
                    
                    //make into story as it has enough information 
                }

                //if only picture and card
                if ((storyEvents[i].Pictures != null && !storyEvents[i].Pictures.Count.Equals(0)) && (storyEvents[i].VoiceRecordings == null || storyEvents[i].VoiceRecordings.Count.Equals(0)) && (storyEvents[i].Cards !=null && !storyEvents[i].Cards.Count.Equals(0)))
                {
                    
                    //make into story as it has enough information - create sentence from card information to be used as a voice recording 
                }

                //if only voice recordings and card
                if ((storyEvents[i].Pictures == null || storyEvents[i].Pictures.Count.Equals(0)) && (storyEvents[i].VoiceRecordings != null && !storyEvents[i].VoiceRecordings.Count.Equals(0)) && (storyEvents[i].Cards !=null && !storyEvents[i].Cards.Count.Equals(0)))
                {
                    
                    //make into story as it has enough information - chose picture from picture pool 
                }


                //if pictures, voice recordings and cards 
                if ((storyEvents[i].Pictures != null && !storyEvents[i].Pictures.Count.Equals(0)) && (storyEvents[i].VoiceRecordings != null && !storyEvents[i].VoiceRecordings.Count.Equals(0)) && (storyEvents[i].Cards !=null && !storyEvents[i].Cards.Count.Equals(0)))
                {
                    
                    //make into story as it has enough information 
                }

               
            }
        }

        public List<StoryEvent> GetEvents()
        {
            var events = _db.GetUnfilteredEvents();
            if (events.Count != 0)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    for (int j = 0; j < events.Count; j++)
                    {
                        if (j != i)
                        {
                            int limit;
                            if (events[i].DateTime.AddHours(1).Hour.Equals(events[i].DateTime.AddMinutes(10).Hour))
                            {
                                limit = 100 + events[i].DateTime.AddMinutes(10).Minute;
                            }
                            else
                            {
                                limit = events[i].DateTime.AddMinutes(10).Minute;
                            }
                            string[] outerLoopValues = events[i].Value.Split('-');
                            string[] innerLoopValues = events[j].Value.Split('-');
                            if(outerLoopValues[1].Equals("Card") && innerLoopValues[1].Equals("Card") &&  outerLoopValues[2].Equals(innerLoopValues[2]) &&
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
                Console.WriteLine(i.DateTime + " " + i.Value);
            }
            return events;
        }
    }
}