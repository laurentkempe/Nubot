namespace Nubot.Interfaces
{
    using System.Collections.Generic;
    using Nancy.TinyIoc;

    public abstract class RobotPluginBase : IRobotPlugin
    {
        protected readonly IRobot Robot;

        protected RobotPluginBase(string pluginName)
        {
            Name = pluginName;

            Robot = TinyIoCContainer.Current.Resolve<IRobot>();

            HelpMessages = new List<string>();
        }

        public IEnumerable<string> HelpMessages { get; protected set; }

        public virtual void Respond(string message)
        {
        }

        public string Name { get; protected set; }
    }
}