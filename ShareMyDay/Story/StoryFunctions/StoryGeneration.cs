using Android.Content;
using ShareMyDay.Database.Models;
using System;
using System.Collections.Generic;

namespace ShareMyDay.Story.StoryFunctions
{
    /*
     * Class name: StoryGeneration
     * Purpose: To control the story generation form events 
     */
    public class StoryGeneration
    {
        private readonly Database.Database _db;
        private readonly Context _context;


        /*
         * Constructor
         * To create a database and context instance 
         */
        public StoryGeneration(Database.Database db, Context context)
        {
            _db = db;
            _context = context;
        }


        /*
         * Method Name: Create
         * Purpose: To create the stories from the events of the day 
         */
        public bool Create()
        {
            var initialStoryEvents = GetEvents();
            if (initialStoryEvents != null && initialStoryEvents.Count > 0)
            {
                List<StoryEvent> storyEvents = GetStoryEvents(initialStoryEvents);

                var extraStories = new List<StoryEvent>();
                
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
                                            int limit = CalculateTimeLimit(storyEvents[i]);
                                           
                                            string[] eventType = storyEvents[j].Value.Split("-");
                                            if ((eventType[1].Equals("Picture Taken") ||
                                                 eventType[1].Equals("Voice Recording Taken")) &&
                                                !eventType[1].Equals("Card") && CheckTime(limit, storyEvents[i], storyEvents[j]))
                                            {
                                                if (!storyEvents[j].InStory.Equals(true))
                                                {
                                                    if (!storyEvents[j].Finished.Equals(true))
                                                    {
                                                        if (eventType[1].Equals("Picture Taken"))
                                                        {
                                                            hasPicture = true;
                                                        }

                                                        finalEvents.Add(storyEvents[j]);
                                                        storyEvents.Remove(storyEvents[j]);
                                                        j--;
                                                    }
                                                    else
                                                    {
                                                        if (eventType[1].Equals("Picture Taken"))
                                                        {
                                                            hasPicture = true;
                                                        }

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
                                            
                                            string defaultPicture = FindDefaultPicture(finalEvents[cardEvent]);

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
                                            int limit = CalculateTimeLimit(storyEvents[i]);

                                            string[] jEvent = storyEvents[j].Value.Split("-");
                                            if ((jEvent[1].Equals("Card") ||
                                                 jEvent[1].Equals("Voice Recording Taken") ||
                                                 jEvent[1].Equals("Picture Taken")) && CheckTime(limit, storyEvents[i], storyEvents[j]))
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
                                            int limit = CalculateTimeLimit(storyEvents[i]);

                                            string[] jEvent = storyEvents[j].Value.Split("-");
                                            if ((jEvent[1].Equals("Card") ||
                                                 jEvent[1].Equals("Voice Recording Taken") ||
                                                 jEvent[1].Equals("Picture Taken")) && CheckTime(limit, storyEvents[i], storyEvents[j]))
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
                                        int limit = CalculateTimeLimit(storyEvents[i]);

                                        string[] jEvent = storyEvents[j].Value.Split("-");
                                        if ((jEvent[1].Equals("Card") ||
                                             jEvent[1].Equals("Voice Recording Taken") ||
                                             jEvent[1].Equals("Picture Taken")) && CheckTime(limit, storyEvents[i], storyEvents[j]))
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
                                        int limit = CalculateTimeLimit(storyEvents[i]);

                                        string[] jEvent = storyEvents[j].Value.Split("-");
                                        if ((jEvent[1].Equals("Voice Recording Taken") ||
                                             jEvent[1].Equals("Picture Taken")) && !jEvent[1].Equals("Card") && CheckTime(limit, storyEvents[i], storyEvents[j]))
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
                                        int limit = CalculateTimeLimit(storyEvents[i]);

                                        string[] jEvent = storyEvents[j].Value.Split("-");
                                        if ((jEvent[1].Equals("Voice Recording Taken") ||
                                             jEvent[1].Equals("Picture Taken")) && !jEvent[1].Equals("Card") && CheckTime(limit, storyEvents[i], storyEvents[j]))
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
                                        int limit = CalculateTimeLimit(storyEvents[i]);

                                        string[] jEvent = storyEvents[j].Value.Split("-");
                                        if ((jEvent[1].Equals("Voice Recording Taken") ||
                                             jEvent[1].Equals("Picture Taken")) && !jEvent[1].Equals("Card") && CheckTime(limit, storyEvents[i], storyEvents[j]))
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
                                    }
                                }

                                finalEvents.Insert(0, storyEvents[i]);
                                _db.InsertStories(finalEvents, false, false, null);
                            }
                        }
                        else
                        {
                            if (storyEvents[i].Finished && !storyEvents[i].InStory)
                            {
                                bool hasPicture = false;
                                bool hasCard = false;
                                bool hasRecording = false; 
                                int extraStoryIndex = 0;
                                
                                List<StoryEvent> finalEvents = new List<StoryEvent>
                                {
                                    storyEvents[i]
                                };
                                
                                if (storyEvents[i].Cards != null && storyEvents[i].Cards.Count != 0)
                                {
                                    hasCard = true;
                                }

                                if (storyEvents[i].Pictures != null && storyEvents[i].Pictures.Count != 0)
                                {
                                    hasPicture = true;
                                }

                                if (storyEvents[i].VoiceRecordings != null && storyEvents[i].VoiceRecordings.Count != 0)
                                {
                                    hasRecording = true;
                                }
                                for (var index = 0; index < storyEvents.Count; index++)
                                {
                                    
                                    var k = storyEvents[index];
                                    if (!k.InStory)
                                    {

                                        if (index != i)
                                        {
                                            if (hasCard)
                                            {
                                                if (k.Finished)
                                                {
                                                    if((index + 1) < storyEvents.Count){
                                                         if(!storyEvents[index+1].Finished)
                                                        {
                                                            if (k.Pictures != null && k.Pictures.Count != 0)
                                                            {
                                                                finalEvents.Add(k);
                                                                hasPicture = true;

                                                            }

                                                            if (k.Cards != null && k.Cards.Count != 0)
                                                            {
                                                                finalEvents.Add(k);
                                                                hasCard = true;
                                                                extraStoryIndex = index;
                                                            }

                                                            if (k.VoiceRecordings != null && k.VoiceRecordings.Count != 0)
                                                            {
                                                                finalEvents.Add(k);
                                                                hasRecording = true;
                                                            }

                                                            break;
                                                        }
                                                    }
                                                }
                                                if (k.Pictures != null && k.Pictures.Count != 0)
                                                {
                                                    finalEvents.Add(k);
                                                    hasPicture = true;

                                                }

                                                if (k.Cards != null && k.Cards.Count != 0)
                                                {
                                                    finalEvents.Add(k);
                                                    hasCard = true;
                                                    extraStoryIndex = index;
                                                }

                                                if (k.VoiceRecordings != null && k.VoiceRecordings.Count != 0)
                                                {
                                                    finalEvents.Add(k);
                                                    hasRecording = true;
                                                }
                                                
                                            }
                                            if (!hasCard)
                                            {


                                                if (k.Finished)
                                                {
                                                    if (k.Pictures != null && k.Pictures.Count != 0)
                                                    {
                                                        finalEvents.Add(k);
                                                        hasPicture = true;

                                                    }

                                                    if (k.Cards != null && k.Cards.Count != 0)
                                                    {
                                                        finalEvents.Add(k);
                                                        hasCard = true;
                                                        extraStoryIndex = index;
                                                    }

                                                    if (k.VoiceRecordings != null && k.VoiceRecordings.Count != 0)
                                                    {
                                                        finalEvents.Add(k);
                                                        hasRecording = true;
                                                    }

                                                    break;
                                                }
                                                else
                                                {
                                                    if (k.Pictures != null && k.Pictures.Count != 0)
                                                    {
                                                        finalEvents.Add(k);
                                                        hasPicture = true;

                                                    }

                                                    if (k.Cards != null && k.Cards.Count != 0)
                                                    {
                                                        finalEvents.Add(k);
                                                        hasCard = true;
                                                        extraStoryIndex = index;
                                                    }

                                                    if (k.VoiceRecordings != null && k.VoiceRecordings.Count != 0)
                                                    {
                                                        finalEvents.Add(k);
                                                        hasRecording = true;
                                                    }
                                                }
                                            }
                                        }
                                    }


                                }
                               

                                if (hasCard)
                                {
                                    InsertStory(finalEvents, hasPicture, hasRecording, hasCard, extraStoryIndex);
                                }
                                else
                                {
                                    InsertStory(finalEvents, hasPicture, hasRecording, hasCard, 0);
                                }

                                hasPicture = false;
                                hasCard = false;
                                hasRecording = false; 
                            }
                        }
                    }
                }

                //also I Did today stories
                if (extraStories.Count != 0)
                {
                    bool hasPicture = false;
                    bool hasRecording = false;
                    var newExtraEvents = new List<StoryEvent>();
                    for (var index = 0; index < extraStories.Count; index++)
                    {
                        bool hasCard = false;
                        var i = extraStories[index];
                        if (i.Cards != null && i.Cards.Count != 0)
                        {
                            hasCard = true; 
                            bool hasPictureSeperate = false;
                            bool hasCardSeperate = false;
                            bool hasRecordingSeperate = false;
                            int extraStoryIndexSeperate = index;
                            bool cardBefore = false; 

                            var seperateStories = new List<Database.Models.StoryEvent>();
                            seperateStories.Add(i);
                            for (var j = 0; j < extraStories.Count; j++)
                            {
                                if (j != index)
                                {
                                    if (extraStories[j].Cards != null && extraStories[j].Cards.Count != 0)
                                    {
                                        if (j > index)
                                        {
                                            
                                            hasCardSeperate = true;
                                            break;
                                        }
                                        else
                                        {
                                            
                                            cardBefore = true;
                                        }
                                    }
                                    else
                                    {
                                        if (!hasCardSeperate)
                                        {
                                            if (extraStories[j].Pictures != null && extraStories[j].Pictures.Count != 0)
                                            {
                                                hasPictureSeperate = true;
                                            }
                                            if (extraStories[j].VoiceRecordings != null && extraStories[j].VoiceRecordings.Count != 0)
                                            {
                                                hasRecordingSeperate = true;
                                            }

                                            if (!cardBefore)
                                            {
                                                seperateStories.Add(extraStories[j]);
                                            }
                                            
                                        }
                                    }
                                }
                            }

                            InsertStory(seperateStories, hasPictureSeperate, hasRecordingSeperate, hasCard, 0);
                        }
                        else
                        {
                            if (i.Pictures != null && i.Pictures.Count != 0)
                            {
                                hasPicture = true;
                                newExtraEvents.Add(i);
                            }

                            if (i.VoiceRecordings != null && i.VoiceRecordings.Count != 0)
                            {
                                hasRecording = true;
                                newExtraEvents.Add(i);
                            }
                        }
                    }

                    if (newExtraEvents.Count != 0)
                    {
                        InsertStory(newExtraEvents, hasPicture, hasRecording, false, 0);
                    }
                }
                return true; 
            }
            return false;
        }

        /*
         * Method name: CheckTime
         * Purpose: To check whether the event occurs within 10 minutes of the event being made into a story 
         */
        public bool CheckTime(int limit, StoryEvent currentStory, StoryEvent storyToCheck)
        {
            if (currentStory.DateTime.Hour.Equals(storyToCheck.DateTime.Hour) &&
                storyToCheck.DateTime.Minute >= currentStory.DateTime.Minute &&
                storyToCheck.DateTime.Minute <= limit)
            {
                return true;
            }

            return false; 
        }

        /*
         * Method name: InsertStory
         * Purpose: To check what type of story has been made and to insert it
         */
        public void InsertStory(List<StoryEvent> stories, bool hasPicture, bool hasRecording, bool hasCard, int pictureIndex)
        {
            if (!hasPicture && hasCard && hasRecording)
            {
                string cardImage = FindDefaultPicture(stories[pictureIndex]);
                _db.InsertStories(stories, false, false, cardImage);
            }
            else if (!hasPicture && hasCard && !hasRecording)
            {
                string cardImage = FindDefaultPicture(stories[pictureIndex]);
                _db.InsertStories(stories, true, true, cardImage);
            }
            else if (hasPicture && !hasCard && !hasRecording)
            {

                _db.InsertStories(stories, true, true, null);
            }
            else if (hasPicture && hasCard && !hasRecording)
            {
                _db.InsertStories(stories, false, true, null);
            }else if (!hasPicture && !hasCard && hasRecording)
            {
                string cardImage = FindDefaultPicture(null);
                _db.InsertStories(stories, false, true, cardImage);
            }
            else
            {
                _db.InsertStories(stories, false, false, null);
            }
        }
        
        /*
         * Method name: GetStoryEvents
         * Purpose: To return the events which make up a story 
         */
        public List<StoryEvent> GetStoryEvents(List<StoryEvent> initialStoryEvents)
        {
            List<StoryEvent> storyEvents = new List<StoryEvent>();
            foreach (var i in initialStoryEvents)
            {
                storyEvents.Add(_db.FindEventByValue(i.Value));
            }

            return storyEvents; 

        }

        /*
         * Method name: CalculateTimeLimit
         * Purpose: To Calculate the 10 minute time from the event to be created as a story
         */
        public int CalculateTimeLimit(StoryEvent story)
        {
            int limit; 

            if (story.DateTime.AddHours(1).Hour
                .Equals(story.DateTime.AddMinutes(10).Hour))
            {
                limit = 100 + story.DateTime.AddMinutes(10).Minute;
            }
            else
            {
                limit = story.DateTime.AddMinutes(10).Minute;
            }

            return limit; 

        }

        /*
         *
         * Method name: GetEvents
         * Purpose: To return the events 
         */
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

            return events;
        }

        /*
         * Method Name: FindDefaultPicture
         * Purpose: To determine the source of the picture to use when there is no picture within the events 
         */
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

            }
            else
            {
                cardTitle = "defaultPicture";
            }

            return cardTitle;
        }
    }
}