namespace Nubot.Plugins
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Interfaces;
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
        }

        public override void Respond(string message)
        {
            Robot.Respond(@"(help)", message, match => ShowHelp());
            Robot.Respond(@"(list)", message, match => ListAdapters());
            Robot.Respond(@"(chain) (.*)", message, match => ChainAdapter(match.Groups[2].Value));
            Robot.Respond(@"(drop) (.*)", message, match => DropAdapter(match.Groups[2].Value));
        }

        private void ListAdapters()
        {
            var stringBuilder = new StringBuilder();

            foreach (var adapter in Robot.RobotAdapters)
            {
                stringBuilder.AppendFormat(" {0} \n", adapter.Name);
            }

            Robot.Message(stringBuilder.ToString());
        }

        private void DropAdapter(string adapterName)
        {
            Robot.DropAdapter(adapterName);
        }

        private void ChainAdapter(string adapterName)
        {
            Robot.ChainAdapter(adapterName);
        }

        private void ShowHelp()
        {
            Robot.ShowHelp();
        }
    }
}