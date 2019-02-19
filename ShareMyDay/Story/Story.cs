using System.Collections.Generic;

namespace ShareMyDay.Story
{
    public class Story
    {
        public string Title { get; set; }

        public List<string> Pictures { get; set; }

        public List<string> VoiceRecordings { get; set; }

        public string IntroductionMessage { get; set; }
    }
}