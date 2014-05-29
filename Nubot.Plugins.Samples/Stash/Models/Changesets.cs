namespace Nubot.Plugins.Samples.Stash.Models
{
    public class Changesets
    {
        public int Size { get; set; }
        public int Limit { get; set; }
        public bool IsLastPage { get; set; }
        public Value[] Values { get; set; }
        public int Start { get; set; }
        public object Filter { get; set; }
    }
}