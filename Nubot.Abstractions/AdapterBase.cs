namespace Nubot.Abstractions
{
    using System.IO;
    using System.Reflection;

    public abstract class AdapterBase : IAdapter
    {
        protected readonly IRobot Robot;

        protected AdapterBase(string adapterName, IRobot robot)
        {
            Name = adapterName;

            Robot = robot;

            Robot.EventEmitter.On<Envelope>("Send", OnSendEvent);
            Robot.EventEmitter.On<Notification>("SendNotification", OnSendNotificationEvent);
        }

        static AdapterBase()
        {
            ExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            BaseAdapterDirectory = Path.Combine(ExecutingDirectory, "adapters");
        }

        private void OnSendEvent(IEventMessage<Envelope> eventMessage)
        {
            Send(eventMessage);
        }

        private void OnSendNotificationEvent(IEventMessage<Notification> eventMessage) 
        {
            SendNotification(eventMessage);
        }

        public string Name { get; private set; }

        public static string ExecutingDirectory { get; private set; }

        public static string BaseAdapterDirectory { get; private set; }

        public virtual void Start()
        {
        }

        public abstract void Send(IEventMessage<Envelope> eventMessage);

        public virtual bool SendNotification(IEventMessage<Notification> eventMessage)
        {
            return false;
        }

        public virtual string MakeConfigFileName()
        {
            var adapterName = Name;
            var file = string.Format("{0}.config", adapterName);

            return Path.Combine(BaseAdapterDirectory, file);
        }
    }
}
