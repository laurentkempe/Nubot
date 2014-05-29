namespace Nubot.Plugins.Samples.Stash.Models
{
    public class Value
    {
        public FromCommit FromCommit { get; set; }
        public ToCommit ToCommit { get; set; }
        public Changes Changes { get; set; }
        public Link Link { get; set; }
    }
}