namespace Nubot.Interfaces
{
    public class Message
    {
        public Message(User user, bool done = false)
        {
            User = user;
            Done = done;
        }

        public User User { get; protected set; }

        public string Room
        {
            get { return User.Room; }
        }

        public bool Done { get; set; }
    }

}