namespace Nubot.Interfaces
{
    using System.Collections.Generic;
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

        public IEnumerable<string> HelpMessages { get; private set; }

        public virtual void Respond(string message)
        {
        }
    }
}