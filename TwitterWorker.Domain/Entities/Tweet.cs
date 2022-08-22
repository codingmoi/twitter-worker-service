namespace TwitterWorker.Domain.Entities
{
    public class Tweet
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public ICollection<Hashtag> Hashtags { get; set; } 
    }
}
