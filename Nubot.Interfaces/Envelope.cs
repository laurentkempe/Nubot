namespace Nubot.Abstractions
{
    public class Envelope
    {
        private readonly Message _message;

        public Envelope(Message message)
        {
            _message = message;
        }

        public User User { get { return _message.User; } }

        public string Room { get { return _message.Room; } }
    }
}