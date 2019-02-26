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
            var initialStoryEvents = GetEvents();

            List<StoryEvent> storyEvents = new List<StoryEvent>();

            var extraStories = new List<StoryEvent>();
            foreach (var i in initialStoryEvents)
            {
                storyEvents.Add(_db.FindByValue(i.Value));
            }

            for (var i = 0; i < storyEvents.Count; i++)
            {
                //if it has not been added to a story or if the story has not been marked as finished 
                if (storyEvents[i].InStory != true || storyEvents[i].Finished != true)
                {
                    //if only cards
                    //need to loop through the rest to find if there are any voice recordings or pictures events for the card to join with within 10 minutes from i.datetime
                    //if there is none then it gets added as its own story 
                    if ((storyEvents[i].Pictures == null || storyEvents[i].Pictures.Count.Equals(0)) &&
                        (storyEvents[i].VoiceRecordings == null || storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
                        storyEvents[i].Cards != null)
                    {
                        List<StoryEvent> finalEvents = new List<StoryEvent>();
                       
                        if (storyEvents[i].Cards.Count != 0)
                        {
                            for (int j = 0; j < storyEvents.Count; j++)
                            {
                                if (i != j)
                                {
                                    //split it
                                    int limit;
                                    if (storyEvents[i].DateTime.AddHours(1).Hour
                                        .Equals(storyEvents[i].DateTime.AddMinutes(10).Hour))
                                    {
                                        limit = 100 + storyEvents[i].DateTime.AddMinutes(10).Minute;
                                    }
                                    else
                                    {
                                        limit = storyEvents[i].DateTime.AddMinutes(10).Minute;
                                    }

                                    string[] jEvent = storyEvents[j].Value.Split("-");
                                    if (jEvent[1].Equals("Picture Taken") ||
                                        jEvent[1].Equals("Voice Recording Taken") && !jEvent[1].Equals("Card") &&
                                        storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                        storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                        storyEvents[j].DateTime.Minute <= limit)
                                    {
                                        finalEvents.Add(storyEvents[j]);
                                        storyEvents.Remove(storyEvents[j]);
                                        j--;
                                    }
                                }
                            }

                            if (finalEvents.Count != 0)
                            {
                                finalEvents.Insert(0,storyEvents[i]);
                                _db.InsertStories(finalEvents);
                            }
                            else
                            {
                                extraStories.Add(storyEvents[i]);
                            }
                            
                        }
                    }

                    //if only pictures
                    //need to loop through the rest to find if there are any voice recordings or cards for the card to join with within 10 minutes from i.datetime
                    //if there is none then it gets added as its own story 
                    if ((storyEvents[i].Cards == null || storyEvents[i].Cards.Count.Equals(0)) &&
                        (storyEvents[i].VoiceRecordings == null || storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
                        storyEvents[i].Pictures
                        != null)
                    {
                        List<StoryEvent> finalEvents = new List<StoryEvent>();
                        
                        if (storyEvents[i].Pictures.Count != 0)
                        {
                            for (int j = 0; j < storyEvents.Count; j++)
                            {
                                if (i != j)
                                {
                                    int limit;
                                    if (storyEvents[i].DateTime.AddHours(1).Hour
                                        .Equals(storyEvents[i].DateTime.AddMinutes(10).Hour))
                                    {
                                        limit = 100 + storyEvents[i].DateTime.AddMinutes(10).Minute;
                                    }
                                    else
                                    {
                                        limit = storyEvents[i].DateTime.AddMinutes(10).Minute;
                                    }

                                    string[] jEvent = storyEvents[j].Value.Split("-");
                                    if (jEvent[1].Equals("Card") ||
                                        jEvent[1].Equals("Voice Recording Taken") ||
                                        jEvent[1].Equals("Picture Taken") &&
                                        storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                        storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                        storyEvents[j].DateTime.Minute <= limit)
                                    {
                                        finalEvents.Add(storyEvents[j]);
                                        storyEvents.Remove(storyEvents[j]);
                                        j--;
                                    }
                                }
                            }

                            if (finalEvents.Count != 0)
                            {
                                finalEvents.Insert(0,storyEvents[i]);
                                _db.InsertStories(finalEvents);
                            }
                            else
                            {
                                extraStories.Add(storyEvents[i]);
                            }

                        }

                    }

                    //if only voice recordings
                    if ((storyEvents[i].Cards == null || storyEvents[i].Cards.Count.Equals(0)) &&
                        (storyEvents[i].Pictures == null || storyEvents[i].Pictures.Count.Equals(0)) &&
                        storyEvents[i].VoiceRecordings
                        != null)
                    {
                        List<StoryEvent> finalEvents = new List<StoryEvent>();
                        
                        if (storyEvents[i].VoiceRecordings.Count != 0)
                        {
                            //need to loop through the rest to find if there are any pictures or cards for the card to join with within 10 minutes from i.datetime
                            //if there is none then it gets added as its own story 
                            
                        
                            for (int j = 0; j < storyEvents.Count; j++)
                            {
                                if (i != j)
                                {
                                    int limit;
                                    if (storyEvents[i].DateTime.AddHours(1).Hour
                                        .Equals(storyEvents[i].DateTime.AddMinutes(10).Hour))
                                    {
                                        limit = 100 + storyEvents[i].DateTime.AddMinutes(10).Minute;
                                    }
                                    else
                                    {
                                        limit = storyEvents[i].DateTime.AddMinutes(10).Minute;
                                    }

                                    string[] jEvent = storyEvents[j].Value.Split("-");
                                    if (jEvent[1].Equals("Card") ||
                                        jEvent[1].Equals("Voice Recording Taken") ||
                                        jEvent[1].Equals("Picture Taken") &&
                                        storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                        storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                        storyEvents[j].DateTime.Minute <= limit)
                                    {
                                        finalEvents.Add(storyEvents[j]);
                                        storyEvents.Remove(storyEvents[j]);
                                        j--;
                                    }
                                }
                            }

                            if (finalEvents.Count != 0)
                            {
                                finalEvents.Insert(0,storyEvents[i]);
                                _db.InsertStories(finalEvents);
                            }
                            else
                            {
                                extraStories.Add(storyEvents[i]);
                            }
                        }
                        

                    }

                    //if only pictures and voice recording
                    if ((storyEvents[i].Pictures != null && !storyEvents[i].Pictures.Count.Equals(0)) &&
                        (storyEvents[i].VoiceRecordings != null && !storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
                        (storyEvents[i].Cards == null || storyEvents[i].Cards. Count.Equals(0)))
                    {

                        //make into story as it has enough information 
                        //check if there are other parts though 
                        List<StoryEvent> finalEvents = new List<StoryEvent>();
                        for (int j = 0; j < storyEvents.Count; j++)
                            {
                                if (i != j)
                                {
                                    int limit;
                                    if (storyEvents[i].DateTime.AddHours(1).Hour
                                        .Equals(storyEvents[i].DateTime.AddMinutes(10).Hour))
                                    {
                                        limit = 100 + storyEvents[i].DateTime.AddMinutes(10).Minute;
                                    }
                                    else
                                    {
                                        limit = storyEvents[i].DateTime.AddMinutes(10).Minute;
                                    }

                                    string[] jEvent = storyEvents[j].Value.Split("-");
                                    if (jEvent[1].Equals("Card") ||
                                        jEvent[1].Equals("Voice Recording Taken") ||
                                        jEvent[1].Equals("Picture Taken") &&
                                        storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                        storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                        storyEvents[j].DateTime.Minute <= limit)
                                    {
                                        finalEvents.Add(storyEvents[j]);
                                        storyEvents.Remove(storyEvents[j]);
                                        j--;
                                    }
                                }
                        }

                            if (finalEvents.Count != 0)
                            {
                                finalEvents.Insert(0,storyEvents[i]);
                            }
                            else
                            {
                                finalEvents.Add(storyEvents[i]);
                            }
                        _db.InsertStories(finalEvents);
                    }

                    //if only picture and card
                    if ((storyEvents[i].Pictures != null && !storyEvents[i].Pictures.Count.Equals(0)) &&
                        (storyEvents[i].VoiceRecordings == null || storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
                        (storyEvents[i].Cards != null && !storyEvents[i].Cards.Count.Equals(0)))
                    {

                        //make into story as it has enough information - create sentence from card information to be used as a voice recording 

                    }

                    //if only voice recordings and card
                    if ((storyEvents[i].Pictures == null || storyEvents[i].Pictures.Count.Equals(0)) &&
                        (storyEvents[i].VoiceRecordings != null && !storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
                        (storyEvents[i].Cards != null && !storyEvents[i].Cards.Count.Equals(0)))
                    {

                        //make into story as it has enough information - chose picture from picture pool 
                    }


                    //if pictures, voice recordings and cards 
                    if ((storyEvents[i].Pictures != null && !storyEvents[i].Pictures.Count.Equals(0)) &&
                        (storyEvents[i].VoiceRecordings != null && !storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
                        (storyEvents[i].Cards != null && !storyEvents[i].Cards.Count.Equals(0)))
                    {

                        //make into story as it has enough information 
                    }


                }
                //also I Did today stories
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