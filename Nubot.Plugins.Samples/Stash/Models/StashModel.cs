namespace Nubot.Plugins.Samples.Stash.Models
{
    public class StashModel
    {
        public Repository repository { get; set; }
        public RefChange[] refChanges { get; set; }
        public Changesets changesets { get; set; }
    }
}
