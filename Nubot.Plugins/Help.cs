namespace Nubot.Plugins
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Abstractions;

    [Export(typeof(IRobotPlugin))]
    public class Default : RobotPluginBase
    {
        [ImportingConstructor]
        public Default(IRobot robot)
            : base("Default", robot)
        {
            HelpMessages = new List<string> {
                "help - Show help of all currently loaded plugin(s)"
            };

            Robot.Respond(@"(help)", msg => Robot.ShowHelp(msg));
        }
    }
}