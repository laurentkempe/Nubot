namespace Nubot.Plugins{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using Interfaces;

    [Export(typeof(IRobotPlugin))]
    public class Httpd : RobotPluginBase
    {
        public Httpd()
            : base("Httpd")
        {
            Robot.Router.Get("nubot/version", x => Robot.Version);
            Robot.Router.Get("nubot/ping", x => "PONG");
            Robot.Router.Get("nubot/time", x => string.Format("Server time is: {0}", DateTime.Now));
            Robot.Router.Get("nubot/info", x =>
            {
                var currentProcess = Process.GetCurrentProcess();
                return string.Format("[pid:{0}] [Start Time:{1}]", currentProcess.Id, currentProcess.StartTime);
            });
            Robot.Router.Get("nubot/ip", x => new WebClient().DownloadString("http://ifconfig.me/ip"));
            Robot.Router.Get("nubot/plugins", x => ShowPlugins());
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