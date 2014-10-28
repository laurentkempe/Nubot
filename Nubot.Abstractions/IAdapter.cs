namespace Nubot.Abstractions
{
    public interface IAdapter
    {
        string Name { get; }

        void Start();

        void Send(IEventMessage<Envelope> eventMessage);

        bool SendNotification(IEventMessage<Notification> eventMessage);

        string MakeConfigFileName();
    }
}