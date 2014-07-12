namespace Nubot.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public string Name { get; private set; }

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
    }
}