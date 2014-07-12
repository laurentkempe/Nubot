namespace Nubot.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;

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

        public virtual IEnumerable<IPluginSetting> Settings { get { return Enumerable.Empty<IPluginSetting>();} }

        public virtual void Respond(string message)
        {
        }

        public string Name { get; protected set; }
    }
}