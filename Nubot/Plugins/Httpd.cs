namespace Nubot.Plugins{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using Interfaces;

    [Export(typeof(IRobotPlugin))]
    public class Httpd : HttpPluginBase
    {
        [ImportingConstructor]
        public Httpd(IRobot robot)
            : base("Httpd", "/", robot)
        {
            Get["nubot/version"] = x => Robot.Version;
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

            foreach (var plugin in Robot.RobotPlugins)
            {
                stringBuilder.AppendFormat("<li>{0}</li>", plugin.Name);
            }

            stringBuilder.Append("</ul>");

            return stringBuilder.ToString();
        }
    }
}