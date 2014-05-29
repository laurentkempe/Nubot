namespace Nubot.Plugins.Samples.Stash.Models
{
    public class StashModel
    {
        public Repository Repository { get; set; }
        public RefChange[] RefChanges { get; set; }
        public Changesets Changesets { get; set; }
    }
}
