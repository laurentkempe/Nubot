namespace Nubot.Plugins.Samples.Stash.Models
{
    public class Project
    {
        public string Key { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Public { get; set; }
        public string Type { get; set; }
        public bool IsPersonal { get; set; }
    }
}