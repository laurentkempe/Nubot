namespace Nubot.Plugins
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Text;
    using Interfaces;

    [Export(typeof (IRobotPlugin))]
    public class Admin : RobotPluginBase
    {
        [ImportingConstructor]
        public Admin(IRobot robot)
            : base("Admin", robot)
        {
            HelpMessages = new List<string>
            {
                "admin plugins list - List plugin(s) currently loaded",
                "admin plugins reload - Reload plugin(s)"
            };
        }

        public override void Respond(string message)
        {
            Robot.Respond(@"(admin) (plugins reload)", message, match => ReloadPlugins());
            Robot.Respond(@"(admin) (plugins list)", message, match => ShowPlugins());
        }

        private void ReloadPlugins()
        {
            Robot.ReloadPlugins();
        }

        private void ShowPlugins()
        {
            var stringBuilder = new StringBuilder();

            foreach (var plugin in Robot.RobotPlugins)
            {
                stringBuilder.AppendFormat("{0}\n", plugin.Name);
            }

            Robot.Message(stringBuilder.ToString());
        }
    }
}