namespace Nubot.Interfaces
{
    using Nubot.Interfaces.Message;
    using System;
    using System.IO;
    using System.Reflection;

    public class AdapterBase : IAdapter
    {
        protected readonly IRobot Robot;

        public AdapterBase(string adapterName, IRobot robot)
        {
            Name = adapterName;

            Robot = robot;

            Robot.Messenger.On<string>("Message", message => { this.Message(message); });
            Robot.Messenger.On<Notification>("SendNotification", notify => { this.SendNotification(notify); });
        }

        static AdapterBase()
        {
            ExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            BaseAdapterDirectory = Path.Combine(ExecutingDirectory, "adapters");
        }

        public string Name { protected set; get; }

        public static string ExecutingDirectory { get; private set; }

        public static string BaseAdapterDirectory { get; private set; }

        public virtual void Start()
        {
        }

        public virtual void Message(IMessage<string> message)
        {
        }

        public virtual bool SendNotification(IMessage<Notification> notify)
        {
            return true;
        }

        public virtual string MakeConfigFileName()
        {
            var adapterName = this.Name;
            var file = string.Format("{0}.config", adapterName);
            var configFileName = Path.Combine(BaseAdapterDirectory, file);

            return configFileName;
        }
    }
}
