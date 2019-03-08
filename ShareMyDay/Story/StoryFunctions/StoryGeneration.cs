using Android.App;
using Android.Content;
using ShareMyDay.Database.Models;
using System;
using System.Collections.Generic;

namespace ShareMyDay.Story.StoryFunctions
{
    public class StoryGeneration
    {
        private readonly Database.Database _db;
        private readonly Context _context; 

        public StoryGeneration(Database.Database db, Context context)
        {
            _db = db;
           _context = context;
        }

        public void Create()
        {
            var initialStoryEvents = GetEvents();
            if (initialStoryEvents != null && initialStoryEvents.Count > 0)
            {


                List<StoryEvent> storyEvents = new List<StoryEvent>();

                var extraStories = new List<StoryEvent>();
                foreach (var i in initialStoryEvents)
                {
                    storyEvents.Add(_db.FindEventByValue(i.Value));
                }


                for (var i = 0; i < storyEvents.Count; i++)
                {
                    //if it has not been added to a story or if the story has not been marked as finished 
                    if (!storyEvents[i].InStory.Equals(true))
                    {
                        if (!storyEvents[i].Finished.Equals(true))
                        {
                            //if only cards
                            //need to loop through the rest to find if there are any voice recordings or pictures events for the card to join with within 10 minutes from i.datetime
                            //if there is none then it gets added as its own story 
                            if ((storyEvents[i].Pictures == null || storyEvents[i].Pictures.Count.Equals(0)) &&
                                (storyEvents[i].VoiceRecordings == null ||
                                 storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
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
                                            if ((jEvent[1].Equals("Picture Taken") ||
                                                 jEvent[1].Equals("Voice Recording Taken")) &&
                                                !jEvent[1].Equals("Card") &&
                                                storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                                storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                                storyEvents[j].DateTime.Minute <= limit)
                                            {
                                                if (!storyEvents[j].InStory.Equals(true))
                                                {
                                                    if (!storyEvents[j].Finished.Equals(true))
                                                    {

                                                        finalEvents.Add(storyEvents[j]);
                                                        storyEvents.Remove(storyEvents[j]);
                                                        j--;

                                                    }
                                                    else
                                                    {
                                                        finalEvents.Add(storyEvents[j]);
                                                        storyEvents.Remove(storyEvents[j]);

                                                        break;
                                                    }
                                                }
                                            }

                                            if (jEvent[1].Equals("Card"))
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    if (finalEvents.Count != 0)
                                    {
                                        finalEvents.Insert(0, storyEvents[i]);
                                        _db.InsertStories(finalEvents, false, false);
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
                                (storyEvents[i].VoiceRecordings == null ||
                                 storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
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
                                            if ((jEvent[1].Equals("Card") ||
                                                 jEvent[1].Equals("Voice Recording Taken") ||
                                                 jEvent[1].Equals("Picture Taken")) &&
                                                storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                                storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                                storyEvents[j].DateTime.Minute <= limit)
                                            {
                                                if (!storyEvents[j].InStory.Equals(true))
                                                {
                                                    if (!storyEvents[j].Finished.Equals(true))
                                                    {
                                                        //check if is a card and if the story being created already has a card 
                                                        finalEvents.Add(storyEvents[j]);
                                                        storyEvents.Remove(storyEvents[j]);
                                                        j--;
                                                    }
                                                    else
                                                    {
                                                        finalEvents.Add(storyEvents[j]);
                                                        storyEvents.Remove(storyEvents[j]);

                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (finalEvents.Count != 0)
                                    {
                                        bool hasRecordings = false; 
                                        finalEvents.Insert(0, storyEvents[i]);
                                        foreach (var k in finalEvents)
                                        {
                                            if (k.VoiceRecordings != null && k.VoiceRecordings.Count != 0)
                                            {
                                                hasRecordings = true;
                                                break; 
                                            }
                                        }

                                        if (hasRecordings)
                                        {
                                            _db.InsertStories(finalEvents, false, false);
                                        }
                                        else
                                        {
                                            _db.InsertStories(finalEvents, false, true);
                                        }
                                        
                                    }
                                    else
                                    {
                                        extraStories.Add(storyEvents[i]);
                                    }

                                }

                            }

                            //if only voice recordings
                            //need to loop through the rest to find if there are any pictures or cards for the card to join with within 10 minutes from i.datetime
                            //if there is none then it gets added as its own story 
                            if ((storyEvents[i].Cards == null || storyEvents[i].Cards.Count.Equals(0)) &&
                                (storyEvents[i].Pictures == null || storyEvents[i].Pictures.Count.Equals(0)) &&
                                storyEvents[i].VoiceRecordings
                                != null)
                            {
                                List<StoryEvent> finalEvents = new List<StoryEvent>();

                                if (storyEvents[i].VoiceRecordings.Count != 0)
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
                                            if ((jEvent[1].Equals("Card") ||
                                                 jEvent[1].Equals("Voice Recording Taken") ||
                                                 jEvent[1].Equals("Picture Taken")) &&
                                                storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                                storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                                storyEvents[j].DateTime.Minute <= limit)
                                            {
                                                if (!storyEvents[j].InStory.Equals(true))
                                                {
                                                    if (!storyEvents[j].Finished.Equals(true))
                                                    {
                                                        //check if is a card and if the story being created already has a card 
                                                        finalEvents.Add(storyEvents[j]);
                                                        storyEvents.Remove(storyEvents[j]);
                                                        j--;
                                                    }
                                                    else
                                                    {
                                                        finalEvents.Add(storyEvents[j]);
                                                        storyEvents.Remove(storyEvents[j]);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (finalEvents.Count != 0)
                                    {
                                        finalEvents.Insert(0, storyEvents[i]);
                                        _db.InsertStories(finalEvents, false, false);
                                    }
                                    else
                                    {
                                        extraStories.Add(storyEvents[i]);
                                    }
                                }


                            }

                            //if only pictures and voice recording
                            //make into story as it has enough information 
                            //check if there are other parts though 
                            if ((storyEvents[i].Pictures != null && !storyEvents[i].Pictures.Count.Equals(0)) &&
                                (storyEvents[i].VoiceRecordings != null &&
                                 !storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
                                (storyEvents[i].Cards == null || storyEvents[i].Cards.Count.Equals(0)))
                            {
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
                                        if ((jEvent[1].Equals("Card") ||
                                             jEvent[1].Equals("Voice Recording Taken") ||
                                             jEvent[1].Equals("Picture Taken")) &&
                                            storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                            storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                            storyEvents[j].DateTime.Minute <= limit)
                                        {
                                            if (!storyEvents[j].InStory.Equals(true))
                                            {
                                                if (!storyEvents[j].Finished.Equals(true))
                                                {
                                                    //check if is a card and if the story being created already has a card 
                                                    finalEvents.Add(storyEvents[j]);
                                                    storyEvents.Remove(storyEvents[j]);
                                                    j--;
                                                }
                                                else
                                                {
                                                    finalEvents.Add(storyEvents[j]);
                                                    storyEvents.Remove(storyEvents[j]);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (finalEvents.Count != 0)
                                {
                                    finalEvents.Insert(0, storyEvents[i]);
                                }
                                else
                                {
                                    finalEvents.Add(storyEvents[i]);
                                }

                                _db.InsertStories(finalEvents, false, false);
                            }

                            //if only voice recording and card
                            if ((storyEvents[i].Pictures == null || storyEvents[i].Pictures.Count.Equals(0)) &&
                                (storyEvents[i].VoiceRecordings != null &&
                                 !storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
                                (storyEvents[i].Cards != null && !storyEvents[i].Cards.Count.Equals(0)))
                            {
                                var hasPicture = false;
                                //make into story as it has enough information - create sentence from card information to be used as a voice recording 
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
                                        if ((jEvent[1].Equals("Voice Recording Taken") ||
                                             jEvent[1].Equals("Picture Taken")) && !jEvent[1].Equals("Card") &&
                                            storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                            storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                            storyEvents[j].DateTime.Minute <= limit)
                                        {
                                            if (jEvent[1].Equals("Picture Taken"))
                                            {
                                                hasPicture = true;
                                            }

                                            if (!storyEvents[j].InStory.Equals(true))
                                            {
                                                if (!storyEvents[j].Finished.Equals(true))
                                                {
                                                    finalEvents.Add(storyEvents[j]);
                                                    storyEvents.Remove(storyEvents[j]);
                                                    j--;
                                                }
                                                else
                                                {
                                                    finalEvents.Add(storyEvents[j]);
                                                    storyEvents.Remove(storyEvents[j]);
                                                    break;
                                                }
                                            }
                                        }

                                        //if(jEvent[1].Equals("Card"))
                                        //{
                                        //    break;
                                        //}
                                    }
                                }

                                if (finalEvents.Count != 0)
                                {
                                    finalEvents.Insert(0, storyEvents[i]);
                                }
                                else
                                {
                                    finalEvents.Add(storyEvents[i]);
                                }

                                if (!hasPicture)
                                {
                                    var eventValue = DateTime.Now.ToLongTimeString() + "-" + "Picture Taken";
                                    StoryEvent storyEvent = new StoryEvent
                                    {
                                        Value = eventValue,
                                        DateTime = DateTime.Now
                                    };

                                    Picture picture = new Picture
                                    {
                                        EventId = storyEvent.Id,
                                        Path =
                                            "storage/emulated/0/Pictures/ShareMyDayDev/imageeab30d8d-f02d-4a2a-88f8-7f4eac55f139.jpg"
                                    };
                                    _db.InsertEvent(true, storyEvent, null, picture, null);
                                    var newEvent = _db.FindEventByValue(eventValue);
                                    finalEvents.Add(newEvent);
                                    _db.InsertStories(finalEvents, false, false);
                                }
                                else
                                {
                                    _db.InsertStories(finalEvents, false, false);
                                }



                            }

                            //if only pictures and card
                            //make into story as it has enough information - create sentence from card information to be used as a voice recording 
                            if ((storyEvents[i].Pictures != null && !storyEvents[i].Pictures.Count.Equals(0)) &&
                                (storyEvents[i].VoiceRecordings == null ||
                                 storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
                                (storyEvents[i].Cards != null && !storyEvents[i].Cards.Count.Equals(0)))
                            {
                                var hasVoiceRecording = false;

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
                                        if ((jEvent[1].Equals("Voice Recording Taken") ||
                                             jEvent[1].Equals("Picture Taken")) && !jEvent[1].Equals("Card") &&
                                            storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                            storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                            storyEvents[j].DateTime.Minute <= limit)
                                        {
                                            if (jEvent[1].Equals("Voice Recording Taken"))
                                            {
                                                hasVoiceRecording = true;
                                            }

                                            if (!storyEvents[j].InStory.Equals(true))
                                            {
                                                if (!storyEvents[j].Finished.Equals(true))
                                                {
                                                    finalEvents.Add(storyEvents[j]);
                                                    storyEvents.Remove(storyEvents[j]);
                                                    j--;
                                                }
                                                else
                                                {
                                                    finalEvents.Add(storyEvents[j]);
                                                    storyEvents.Remove(storyEvents[j]);
                                                    break;
                                                }
                                            }
                                        }

                                        //if(jEvent[1].Equals("Card"))
                                        //{
                                        //    break;
                                        //}
                                    }
                                }

                                if (finalEvents.Count != 0)
                                {
                                    finalEvents.Insert(0, storyEvents[i]);
                                }
                                else
                                {
                                    finalEvents.Add(storyEvents[i]);
                                }

                                if (!hasVoiceRecording)
                                {
                                    _db.InsertStories(finalEvents, false, true);
                                }
                                else
                                {
                                    _db.InsertStories(finalEvents, false, false);
                                }
                            }


                            //if pictures, voice recordings and cards 
                            //make into story as it has enough information 
                            if ((storyEvents[i].Pictures != null && !storyEvents[i].Pictures.Count.Equals(0)) &&
                                (storyEvents[i].VoiceRecordings != null &&
                                 !storyEvents[i].VoiceRecordings.Count.Equals(0)) &&
                                (storyEvents[i].Cards != null && !storyEvents[i].Cards.Count.Equals(0)))
                            {

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
                                        if ((jEvent[1].Equals("Voice Recording Taken") ||
                                             jEvent[1].Equals("Picture Taken")) && !jEvent[1].Equals("Card") &&
                                            storyEvents[i].DateTime.Hour.Equals(storyEvents[j].DateTime.Hour) &&
                                            storyEvents[j].DateTime.Minute >= storyEvents[i].DateTime.Minute &&
                                            storyEvents[j].DateTime.Minute <= limit)
                                        {
                                            if (!storyEvents[j].InStory.Equals(true))
                                            {
                                                if (!storyEvents[j].Finished.Equals(true))
                                                {
                                                    finalEvents.Add(storyEvents[j]);
                                                    storyEvents.Remove(storyEvents[j]);
                                                    j--;
                                                }
                                                else
                                                {
                                                    finalEvents.Add(storyEvents[j]);
                                                    storyEvents.Remove(storyEvents[j]);
                                                    break;
                                                }

                                            }
                                        }

                                        //if(jEvent[1].Equals("Card"))
                                        //{
                                        //    break;
                                        //}
                                    }
                                }

                                finalEvents.Insert(0, storyEvents[i]);
                                _db.InsertStories(finalEvents, false, false);
                            }


                        }
                        else
                        {
                            if (storyEvents[i].Finished)
                            {
                                List<StoryEvent> finalEvents = new List<StoryEvent>
                                {
                                    storyEvents[i]
                                };
                                _db.InsertStories(finalEvents, false, true);
                            }

                        }
                    }
                }

                //also I Did today stories
                if (extraStories.Count != 0)
                {
                    _db.InsertStories(extraStories, false, false);
                }
            }
            else
            {
                AlertDialog.Builder alertBox = new AlertDialog.Builder (_context);
                alertBox.SetTitle ("No Stories");
                alertBox.SetMessage ("No events have been recorded to be able to make stories yet. Please try adding events first.");
                alertBox.SetNeutralButton ("OK", (senderAlert, args) => {
                });
                alertBox.Create();
                alertBox.Show();
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
                            if (outerLoopValues[1].Equals("Card") && innerLoopValues[1].Equals("Card") &&
                                outerLoopValues[2].Equals(innerLoopValues[2]) &&
                                events[i].DateTime.Hour.Equals(events[j].DateTime.Hour) &&
                                events[j].DateTime.Minute >= events[i].DateTime.Minute &&
                                events[j].DateTime.Minute <= limit)
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

        public List<Database.Models.Story> GetStories()
        {
            List<Database.Models.Story> stories =_db.GetAllStories();
            return stories;
        }
    }
}