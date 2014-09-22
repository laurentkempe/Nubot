namespace Nubot.Interfaces
{
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

            ExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            BaseAdapterDirectory = Path.Combine(ExecutingDirectory, "adapters");
        }

        public string Name { protected set; get; }

        public virtual void Start()
        {
        }

        public virtual void Message(string message)
        {
        }

        public virtual bool SendNotification(string roomName, string authToken, string htmlMessage, bool notify = false)
        {
            return false;
        }

        public string ExecutingDirectory { protected set; get; }

        public string BaseAdapterDirectory { protected set; get; }

        public virtual string MakeConfigFileName()
        {
            var adapterName = this.Name;
            var file = string.Format("{0}.config", adapterName);
            var configFileName = Path.Combine(this.BaseAdapterDirectory, file);

            return configFileName;
        }
    }
}
