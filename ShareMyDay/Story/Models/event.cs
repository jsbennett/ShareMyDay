using System;
using System.Collections.Generic;

namespace ShareMyDay.Story.Models
{
    public struct Event
    {
        public int Id;

        public string Type;

        public string Value;

        public DateTime DateTime;


        public List<EventPicture> Pictures;

        public List<EventRecording> VoiceRecordings;

        public Event(int id, string type, string value, DateTime dateTime, List<EventPicture> pictures, List<EventRecording> voiceRecordings)
        {
            Id = id;
            Type = type;
            Value = value;
            DateTime = dateTime;
            Pictures = pictures;
            VoiceRecordings = voiceRecordings; 
        }
    }
}