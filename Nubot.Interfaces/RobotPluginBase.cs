namespace Nubot.Interfaces
{
    using System.Collections.Generic;

    public abstract class RobotPluginBase : IRobotPlugin
    {
        protected readonly IRobot Robot;

        protected RobotPluginBase(string pluginName, IRobot robot)
        {
            Name = pluginName;

            Robot = robot;

            HelpMessages = new List<string>();
        }

        public IEnumerable<string> HelpMessages { get; protected set; }

        public virtual void Respond(string message)
        {
        }

        public string Name { get; private set; }
    }
}