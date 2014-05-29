namespace Nubot.Plugins.Samples.Stash.Models
{
    public class ToCommit
    {
        public string Id { get; set; }
        public string DisplayId { get; set; }
        public Author Author { get; set; }
        public long AuthorTimestamp { get; set; }
        public string Message { get; set; }
        public Parent[] Parents { get; set; }
    }
}