namespace Nubot.Plugins.Nubot {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using Interfaces;
    using ViewModel;

    [Export(typeof(IRobotPlugin))]
    public class Admin : HttpPluginBase
    {
        [ImportingConstructor]
        public Admin(IRobot robot)
            : base("Nubot Admin", "/nubot", robot)
        {
            HelpMessages = new List<string>
            {
                "admin plugins list - List plugin(s) currently loaded",
                "admin plugins reload - Reload plugin(s)"
            };

            Get["version"] = x => Robot.Version;
            Get["ping"] = x => "PONG";
            Get["time"] = x => string.Format("Server time is: {0}", DateTime.Now);
            Get["ip"] = x => new WebClient().DownloadString("http://ifconfig.me/ip");
            Get["plugins"] = x => ShowPlugins();
            Get["info"] = x =>
            {
                var currentProcess = Process.GetCurrentProcess();
                return string.Format("[pid:{0}] [Start Time:{1}]", currentProcess.Id, currentProcess.StartTime);
            };

            Get["admin"] = x => View["index.cshtml", new IndexViewModel { RobotPlugins = Robot.RobotPlugins, RobotVersion = Robot.Version }];
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

        public override void Respond(string message)
        {
            Robot.Respond(@"(admin) (plugins reload)", message, match => ReloadPlugins());
            Robot.Respond(@"(admin) (plugins list)", message, match => ShowPluginsAsMessage());
        }

        private void ReloadPlugins()
        {
            Robot.ReloadPlugins();
        }

        private void ShowPluginsAsMessage()
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