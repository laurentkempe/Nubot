namespace Nubot.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Reflection;

    public abstract class RobotPluginBase : IRobotPlugin
    {
        protected readonly IRobot Robot;

        protected RobotPluginBase(string pluginName, IRobot robot)
        {
            Name = pluginName;

            Robot = robot;

            HelpMessages = new List<string>();

            ExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            BasePluginsDirectory = Path.Combine(ExecutingDirectory, "plugins");
        }

        public string ExecutingDirectory { protected set; get; }
        public string BasePluginsDirectory { protected set; get; }
        
        public IEnumerable<string> HelpMessages { get; protected set; }

        public virtual IEnumerable<IPluginSetting> Settings { get { return Enumerable.Empty<IPluginSetting>();} }

        public virtual void Respond(string message)
        {
        }

        public string Name { get; protected set; }

        public virtual string MakeConfigFileName()
        {
            var pluginName = this.Name.Replace(" ", string.Empty);
            var file = string.Format("{0}.config", pluginName);
            var configFileName = Path.Combine(this.BasePluginsDirectory, file);

            return configFileName;
        }
    }
}