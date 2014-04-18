namespace Nubot.Plugins
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Interfaces;

    [Export(typeof (IRobotPlugin))]
    public class Default : RobotPluginBase
    {
        [ImportingConstructor]
        public Default(IRobot robot)
            : base("Default", robot)
        {
            HelpMessages = new List<string> {
                "help - Show help of all currently loaded plugin(s)"
            };
        }

        public override void Respond(string message)
        {
            Robot.Respond(@"(help)", message, match => ShowHelp());
        }

        private void ShowHelp()
        {
            Robot.ShowHelp();
        }
    }
}