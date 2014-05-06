namespace Nubot.Plugins.Httpd {

    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using Interfaces;
    using Nancy;
    using Nancy.Responses.Negotiation;

    [Export(typeof(IRobotPlugin))]
    public class Httpd : HttpPluginBase
    {
        [ImportingConstructor]
        public Httpd(IRobot robot)
            : base("Httpd", "/nubot", robot)
        {
            Get["version"] = x => Robot.Version;
            Get["ping"] = x => "PONG";
            Get["time"] = x => string.Format("Server time is: {0}", DateTime.Now);
            Get["info"] = x =>
            {
                var currentProcess = Process.GetCurrentProcess();
                return string.Format("[pid:{0}] [Start Time:{1}]", currentProcess.Id, currentProcess.StartTime);
            };
            Get["ip"] = x => new WebClient().DownloadString("http://ifconfig.me/ip");
            Get["plugins"] = x => ShowPlugins();

            Get["test"] = x => PluginView["test.html"];
        }

        private string ShowPlugins()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<ul>");

            foreach (var plugin in Robot.RobotPlugins)
            {
                stringBuilder.AppendFormat("<li>{0}</li>", plugin.Name);
            }

            stringBuilder.Append("</ul>");

            return stringBuilder.ToString();
        }
    }
}