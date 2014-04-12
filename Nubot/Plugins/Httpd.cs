namespace Nubot.Plugins{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using Interfaces;
    using Nancy;
    using Nancy.TinyIoc;

    [Export(typeof(IRobotPlugin))]
    public class Httpd : NancyModule, IRobotPlugin
    {
        private readonly IRobot _robot;

        public Httpd()
            : base("/")
        {
            Name = "Httpd";

            _robot = TinyIoCContainer.Current.Resolve<IRobot>();

            Get["nubot/version"] = x => _robot.Version;
            Get["nubot/ping"] = x => "PONG";
            Get["nubot/time"] = x => string.Format("Server time is: {0}", DateTime.Now);
            Get["nubot/info"] = x =>
            {
                var currentProcess = Process.GetCurrentProcess();
                return string.Format("[pid:{0}] [Start Time:{1}]", currentProcess.Id, currentProcess.StartTime);
            };
            Get["nubot/ip"] = x => new WebClient().DownloadString("http://ifconfig.me/ip");
            Get["nubot/plugins"] = x => ShowPlugins();
        }

        private string ShowPlugins()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<ul>");

            foreach (var plugin in _robot.RobotPlugins)
            {
                stringBuilder.AppendFormat("<li>{0}</li>", plugin.Name);
            }

            stringBuilder.Append("</ul>");

            return stringBuilder.ToString();
        }

        public string Name { get; private set; }
        public IEnumerable<string> HelpMessages { get; }

        public void Respond(string message)
        {
        }
    }
}