namespace Nubot.Abstractions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public abstract class RobotPluginBase : IRobotPlugin
    {
        protected readonly IRobot Robot;

        protected RobotPluginBase(string pluginName, IRobot robot)
        {
            Name = pluginName;

            Robot = robot;

            HelpMessages = new List<string>();
        }

        static RobotPluginBase()
        {
            ExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            BasePluginsDirectory = Path.Combine(ExecutingDirectory, "plugins");
        }

        public string Name { get; protected set; }

        public static string ExecutingDirectory { get; private set; }

        public static string BasePluginsDirectory { get; private set; }

        public IEnumerable<string> HelpMessages { get; protected set; }

        public virtual IEnumerable<IPluginSetting> Settings { get { return Enumerable.Empty<IPluginSetting>();} }

        public virtual string MakeConfigFileName()
        {
            var pluginName = this.Name.Replace(" ", string.Empty);
            var file = string.Format("{0}.config", pluginName);
            var configFileName = Path.Combine(BasePluginsDirectory, file);

            return configFileName;
        }
    }
}