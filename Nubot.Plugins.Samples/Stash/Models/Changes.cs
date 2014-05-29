namespace Nubot.Plugins.Samples.Stash.Models
{
    public class Changes
    {
        public int Size { get; set; }
        public int Limit { get; set; }
        public bool IsLastPage { get; set; }
        public Value1[] Values { get; set; }
        public int Start { get; set; }
        public object Filter { get; set; }
    }
}