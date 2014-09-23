namespace Nubot.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy;
    using System.IO;
    using System.Reflection;

    public abstract class HttpPluginBase : NancyModule, IRobotPlugin
    {
        protected readonly IRobot Robot;

        protected HttpPluginBase(string pluginName, string modulePath, IRobot robot)
            : base(modulePath)
        {
            Name = pluginName;

            Robot = robot;

            HelpMessages = new List<string>();

            ExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            BasePluginsDirectory = Path.Combine(ExecutingDirectory, "plugins");
        }

        public string Name { get; protected set; }

        public IEnumerable<string> HelpMessages { get; protected set; }

        public virtual IEnumerable<IPluginSetting> Settings { get {return Enumerable.Empty<IPluginSetting>();} }

        public virtual void Respond(string message)
        {
        }

        public virtual IEnumerable<Tuple<string, string>> StaticPaths
        {
            get
            {
                yield return new Tuple<string, string>(ModulePath + "/css", string.Format("plugins{0}/views/css", ModulePath));
                yield return new Tuple<string, string>(ModulePath + "/scripts", string.Format("plugins{0}/views/scripts", ModulePath));
            }
        }


        public string ExecutingDirectory { protected set; get; }

        public string BasePluginsDirectory { protected set; get; }

        public virtual string MakeConfigFileName()
        {
            var subPath = this.ModulePath.StartsWith("/") ? this.ModulePath.Substring(1) : this.ModulePath;

            var pluginName = this.Name.Replace(" ", string.Empty);
            var file = string.Format("{0}.config", pluginName);
            var configFileName = Path.Combine(this.BasePluginsDirectory, subPath, file);

            return configFileName;
        }
    }
}