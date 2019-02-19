namespace ShareMyDay.Story.Models
{
    public struct EventPicture
    {
        public int Id { get; set; }
       
        public int NfcEventId { get; set; }

        public string Path { get; set; }
    }
}