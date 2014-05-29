namespace Nubot.Plugins.Samples.Stash.Models
{
    public class RefChange
    {
        public string RefId { get; set; }
        public string FromHash { get; set; }
        public string ToHash { get; set; }
        public string Type { get; set; }
    }
}