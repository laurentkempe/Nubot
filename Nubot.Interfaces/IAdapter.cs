namespace Nubot.Interfaces
{
    public interface IAdapter
    {
        string Name { get; }

        void Start();

        void Send(Envelope envelope, params string[] messages);

        bool SendNotification(string roomName, string authToken, string htmlMessage, bool notify = false);

        string MakeConfigFileName();
    }
}