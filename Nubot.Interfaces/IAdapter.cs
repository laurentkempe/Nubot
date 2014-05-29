namespace Nubot.Interfaces
{
    public interface IAdapter
    {
        void Start();

        void Message(string message);

        bool SendNotification(string roomName, string htmlMessage, bool notify = false);
    }
}