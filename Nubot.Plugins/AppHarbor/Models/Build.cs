namespace Nubot.Plugins.AppHarbor.Models
{
    public class Build
    {
        public Commit Commit { get; set; }
        public string Status { get; set; }
    }
}