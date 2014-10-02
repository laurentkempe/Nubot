namespace Nubot.Interfaces
{
    public interface IAdapter
    {
        string Name { get; }

        void Start();

        void Message(string message);

        bool SendNotification(string roomName, string authToken, string htmlMessage, bool notify = false);

        string MakeConfigFileName();
    }
}