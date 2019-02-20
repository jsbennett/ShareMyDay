using System;
using System.Collections.Generic;
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
            
            //get all events from todays date - DONE 
            //check if they have pictures and/or voice recordings
            //if they dont have any then it is not to be made into a story 
            //if they have a voice recording but no picture then assign picture from picture bank 
            //check the time of the events - DONE
            //if type of event is class or activity and the value is the same in multiple events within the same hour i.e how long a school period lasts then only take one - DONE
            //if event is of type friend, item, visitor, teacher then get their time and compare it to the times from the events classed as an activity or class
            //if its in the same hour time frame then it is in that class or activity. 
            //then massive if statement to create sentences for activities and classes 
        }

        public List<StoryEvent> GetEvents()
        {
            var events = _db.GetUnfilteredEvents();
            if(events.Count !=0){
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
    }
}