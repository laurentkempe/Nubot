namespace Nubot.Abstractions
{
    public class User
    {
        public User(string id, string name, string room, string adapterId)
        {
            Id = id;
            Name = name;
            Room = room;
            AdapterId = adapterId;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public string Room { get; private set; }

        public string AdapterId  { get; private set; }
    }
}