namespace Nubot.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Nancy;

    public abstract class HttpPluginBase : NancyModule, IRobotPlugin
    {
        protected readonly IRobot Robot;

        protected HttpPluginBase(string pluginName, string modulePath, IRobot robot)
            : base(modulePath)
        {
            Name = pluginName;

            Robot = robot;

            HelpMessages = new List<string>();
        }

        static HttpPluginBase()
        {
            ExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            BasePluginsDirectory = Path.Combine(ExecutingDirectory, "plugins");
        }

        public string Name { get; protected set; }

        public static string ExecutingDirectory { get; private set; }

        public static string BasePluginsDirectory { get; private set; }

        public IEnumerable<string> HelpMessages { get; protected set; }

        public virtual IEnumerable<IPluginSetting> Settings { get {return Enumerable.Empty<IPluginSetting>();} }

        public virtual IEnumerable<Tuple<string, string>> StaticPaths
        {
            get
            {
                yield return new Tuple<string, string>(ModulePath + "/css", string.Format("plugins{0}/views/css", ModulePath));
                yield return new Tuple<string, string>(ModulePath + "/scripts", string.Format("plugins{0}/views/scripts", ModulePath));
                yield return new Tuple<string, string>(ModulePath + "/images", string.Format("plugins{0}/views/images", ModulePath));
            }
        }

        public virtual string MakeConfigFileName()
        {
            var subPath = ModulePath.StartsWith("/") ? ModulePath.Substring(1) : ModulePath;

            var pluginName = Name.Replace(" ", string.Empty);
            var file = string.Format("{0}.config", pluginName);
            var configFileName = Path.Combine(BasePluginsDirectory, subPath, file);

            return configFileName;
        }
    }
}