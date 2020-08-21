namespace Skrawl.API.Data.Models
{
    public class Note
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; } 

        public long UserId { get; set; }

        public User User { get; set; }
    }
}