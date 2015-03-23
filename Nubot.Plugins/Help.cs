namespace Nubot.Plugins
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Abstractions;
    using System.Text;

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
            Robot.Respond(@"(list)", msg => ListAdapters(msg));
            Robot.Respond(@"(chain) (.*)", msg => Robot.ChainAdapter(msg.Match[2]));
            Robot.Respond(@"(drop) (.*)", msg => Robot.DropAdapter(msg.Match[2]));
        }

        private void ListAdapters(Response message)
        {
            var stringBuilder = new StringBuilder();

            foreach (var adapter in Robot.RobotAdapters)
            {
                stringBuilder.AppendFormat(" {0} \n", adapter.Name);
            }

            message.Send(stringBuilder.ToString());
        }
    }
}