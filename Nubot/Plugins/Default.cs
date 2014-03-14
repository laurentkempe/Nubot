namespace Nubot.Plugins
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Interfaces;
    using Nancy.TinyIoc;

    [Export(typeof (IRobotPlugin))]
    public class Default : IRobotPlugin
    {
        private readonly IRobot _robot;

        public Default()
        {
            Name = "Default";

            _robot = TinyIoCContainer.Current.Resolve<IRobot>();

            HelpMessages = new List<string> {
                "help - Show help of all currently loaded plugin(s)"};
        }

        public string Name { get; private set; }

        public IEnumerable<string> HelpMessages { get; private set; }

        public void Respond(string message)
        {
            _robot.Respond(@"(help)", message, match => ShowHelp());
        }

        private void ShowHelp()
        {
            _robot.ShowHelp();
        }
    }
}