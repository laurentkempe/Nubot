namespace Nubot.Plugins
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Text;
    using Interfaces;
    using Nancy.TinyIoc;

    [Export(typeof (IRobotPlugin))]
    public class Admin : IRobotPlugin
    {
        private readonly IRobot _robot;

        public Admin()
        {
            Name = "Admin";

            _robot = TinyIoCContainer.Current.Resolve<IRobot>();

            HelpMessages = new List<string>
            {
                "admin plugins list - List plugin(s) currently loaded",
                "admin plugins reload - Reload plugin(s)"
            };
        }

        public IEnumerable<string> HelpMessages { get; private set; }

        public string Name { get; private set; }

        public void Respond(string message)
        {
            _robot.Respond(@"(admin) (plugins reload)", message, match => ReloadPlugins());
            _robot.Respond(@"(admin) (plugins list)", message, match => ShowPlugins());
        }

        private void ReloadPlugins()
        {
            _robot.ReloadPlugins();
        }

        private void ShowPlugins()
        {
            var stringBuilder = new StringBuilder();

            foreach (var plugin in _robot.RobotPlugins)
            {
                stringBuilder.AppendFormat("{0}\n", plugin.Name);
            }

            _robot.Message(stringBuilder.ToString());
        }
    }
}