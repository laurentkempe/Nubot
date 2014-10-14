using Nubot.Interfaces.Message;
namespace Nubot.Interfaces
{
    public interface IAdapter
    {
        string Name { get; }

        void Start();

        void Message(IMessage<string> message);

        bool SendNotification(IMessage<Notification> notify);

        string MakeConfigFileName();
    }
}