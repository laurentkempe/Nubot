namespace Nubot.Interfaces
{
    using System.Collections.Generic;
    using Nancy;
    using Nancy.TinyIoc;

    public abstract class HttpPluginBase : NancyModule, IRobotPlugin
    {
        protected readonly IRobot Robot;

        protected HttpPluginBase(string pluginName, string modulePath)
            : base(modulePath)
        {
            Name = pluginName;

            Robot = TinyIoCContainer.Current.Resolve<IRobot>();

            HelpMessages = new List<string>();
        }

        public string Name { get; private set; }

        public IEnumerable<string> HelpMessages { get; private set; }

        public virtual void Respond(string message)
        {
        }
    }
}