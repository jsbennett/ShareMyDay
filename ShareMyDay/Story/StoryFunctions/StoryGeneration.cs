using Android.App;
using Android.Content;
using ShareMyDay.Database.Models;
using System;
using System.Collections.Generic;
using Android.Media;

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
                                bool hasPicture = false;
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
                                                        if (jEvent[1].Equals("Picture Taken"))
                                                        {
                                                            hasPicture = true;
                                                        }

                                                        finalEvents.Add(storyEvents[j]);
                                                        storyEvents.Remove(storyEvents[j]);
                                                        j--;
                                                    }
                                                    else
                                                    {
                                                        if (jEvent[1].Equals("Picture Taken"))
                                                        {
                                                            hasPicture = true;
                                                        }

                                                        finalEvents.Add(storyEvents[j]);
                                                        storyEvents.Remove(storyEvents[j]);

                                                        break;
                                                    }
                                                }
                                            }

                                            //if (jEvent[1].Equals("Card"))
                                            //{
                                            //    break;
                                            //}
                                        }
                                    }

                                    if (finalEvents.Count != 0)
                                    {
                                        finalEvents.Insert(0, storyEvents[i]);
                                        if (!hasPicture)
                                        {
                                            int cardEvent = 0;
                                            for (var index = 0; index < finalEvents.Count; index++)
                                            {
                                                var k = finalEvents[index];
                                                if (k.Cards != null && k.Cards.Count != 0)
                                                {
                                                    cardEvent = index;
                                                    break;
                                                }
                                            }

                                            string defaultPicture;

                                            defaultPicture = FindDefaultPicture(finalEvents[cardEvent]);

                                            _db.InsertStories(finalEvents, false, false, defaultPicture);
                                        }
                                        else
                                        {
                                            bool hasRecording = false;
                                            foreach (var k in finalEvents)
                                            {
                                                if (k.VoiceRecordings != null && k.VoiceRecordings.Count != 0)
                                                {
                                                    hasRecording = true;
                                                }
                                            }

                                            if (hasRecording)
                                            {
                                                _db.InsertStories(finalEvents, false, false, null);
                                            }
                                            else
                                            {
                                                _db.InsertStories(finalEvents, false, true, null);
                                            }
                                        }

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
                                            _db.InsertStories(finalEvents, false, false, null);
                                        }
                                        else
                                        {
                                            _db.InsertStories(finalEvents, false, true, null);
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
                                        bool hasPictures = false;
                                        bool hasCard = false;
                                        int cardEvent = 0;
                                        finalEvents.Insert(0, storyEvents[i]);
                                        for (var j = 0; j < finalEvents.Count; j++)
                                        {
                                            var k = finalEvents[j];
                                            if (k.Pictures != null && k.Pictures.Count != 0)
                                            {
                                                hasPictures = true;
                                            }

                                            if (k.Cards != null && k.Cards.Count != 0)
                                            {
                                                hasCard = true;
                                                cardEvent = j;
                                            }
                                        }

                                        if (hasPictures)
                                        {
                                            _db.InsertStories(finalEvents, false, false, null);
                                        }
                                        else
                                        {
                                            string defaultPicture;
                                            if (hasCard)
                                            {
                                                defaultPicture = FindDefaultPicture(finalEvents[cardEvent]);
                                            }
                                            else
                                            {
                                                defaultPicture = FindDefaultPicture(null);
                                            }

                                            _db.InsertStories(finalEvents, false, false, defaultPicture);
                                        }
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

                                _db.InsertStories(finalEvents, false, false, null);
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
                                    int cardEvent = 0;

                                    for (var index = 0; index < finalEvents.Count; index++)
                                    {
                                        var k = finalEvents[index];
                                        if (k.Cards != null && k.Cards.Count != 0)
                                        {
                                            cardEvent = index;
                                            break;
                                        }
                                    }

                                    string defaultPicture;

                                    defaultPicture = FindDefaultPicture(finalEvents[cardEvent]);

                                    _db.InsertStories(finalEvents, false, false, defaultPicture);
                                }
                                else
                                {
                                    _db.InsertStories(finalEvents, false, false, null);
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
                                    _db.InsertStories(finalEvents, false, true, null);
                                }
                                else
                                {

                                    _db.InsertStories(finalEvents, false, false, null);
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
                                _db.InsertStories(finalEvents, false, false, null);
                            }
                        }
                        else
                        {
                            if (storyEvents[i].Finished)
                            {
                                bool hasPicture = false;
                                bool hasCard = false;
                                int extraStoryIndex = 0;
                                for (var index = 0; index < storyEvents.Count; index++)
                                {
                                    var k = storyEvents[index];
                                    if (k.Pictures != null && k.Pictures.Count != 0)
                                    {
                                        hasPicture = true;
                                    }

                                    if (k.Cards != null && k.Cards.Count != 0)
                                    {
                                        hasCard = true;
                                        extraStoryIndex = index;
                                    }
                                }

                                if (!hasPicture && hasCard)
                                {
                                    List<StoryEvent> finalEvents = new List<StoryEvent>
                                    {
                                        storyEvents[i]
                                    };
                                    string cardImage = FindDefaultPicture(storyEvents[extraStoryIndex]);
                                    _db.InsertStories(finalEvents, false, false, cardImage);
                                }
                                else if (!hasPicture && !hasCard)
                                {
                                    List<StoryEvent> finalEvents = new List<StoryEvent>
                                    {
                                        storyEvents[i]
                                    };
                                    string cardImage = FindDefaultPicture(null);
                                    _db.InsertStories(finalEvents, false, false, cardImage);
                                }
                                else
                                {
                                    List<StoryEvent> finalEvents = new List<StoryEvent>
                                    {
                                        storyEvents[i]
                                    };
                                    _db.InsertStories(finalEvents, false, false, null);
                                }

                            }
                        }
                    }
                }

                //also I Did today stories
                if (extraStories.Count != 0)
                {
                    bool hasPicture = false;
                    bool hasCard = false;
                    bool hasRecording = false;
                    int extraStoryIndex = 0;
                    for (var index = 0; index < extraStories.Count; index++)
                    {
                        var i = extraStories[index];
                        if (i.Pictures != null && i.Pictures.Count != 0)
                        {
                            hasPicture = true;
                        }

                        if (i.Cards != null && i.Cards.Count != 0)
                        {
                            hasCard = true;
                            extraStoryIndex = index;
                        }

                        if (i.VoiceRecordings != null && i.VoiceRecordings.Count != 0)
                        {
                            hasRecording = true;
                        }
                    }

                    if (!hasPicture && hasCard && hasRecording)
                    {
                        string cardImage = FindDefaultPicture(extraStories[extraStoryIndex]);
                        _db.InsertStories(extraStories, false, false, cardImage);
                    }
                    else if (!hasPicture && hasCard && !hasRecording)
                    {
                        string cardImage = FindDefaultPicture(extraStories[extraStoryIndex]);
                        _db.InsertStories(extraStories, true, true, cardImage);
                    }
                    else if (hasPicture && !hasCard && !hasRecording)
                    {

                        _db.InsertStories(extraStories, true, true, null);
                    }
                    else if (hasPicture && hasCard && !hasRecording)
                    {
                        string cardImage = FindDefaultPicture(extraStories[extraStoryIndex]);
                        _db.InsertStories(extraStories, false, true, cardImage);
                    }
                    else
                    {
                        _db.InsertStories(extraStories, false, false, null);
                    }
                }
            }
            else
            {
                AlertDialog.Builder alertBox = new AlertDialog.Builder(_context);
                alertBox.SetTitle("No Stories");
                alertBox.SetMessage(
                    "No events have been recorded to be able to make stories yet. Please try adding events first.");
                alertBox.SetNeutralButton("OK", (senderAlert, args) => { });
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
            List<Database.Models.Story> stories = _db.GetAllStories();
            return stories;
        }

        public string FindDefaultPicture(StoryEvent cardStory)
        {
            string cardTitle = " ";
            if (cardStory != null)
            {
                var type = cardStory.Cards[0].Type;
                switch (type)
                {
                    case "1":
                        cardTitle = "leisureCard";
                        break;
                    case "2":
                        cardTitle = "classActivityCard";
                        break;
                    case "3":
                        cardTitle = "classCard";
                        break;
                    case "4":
                        cardTitle = "itemCard";
                        break;
                    case "5":
                        cardTitle = "teacherCard";
                        break;
                    case "6":
                        cardTitle = "friendCard";
                        break;
                    case "7":
                        cardTitle = "visitorCard";
                        break;
                }
                //find out the type 
                //make new picture event 
                //insert it into db

            }
            else
            {
                cardTitle = "defaultPicture";
            }

            return cardTitle;
        }
    }
}