namespace Nubot.Plugins.Nubot {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Linq;
    using Interfaces;
    using Nancy;
    using Nancy.ModelBinding;
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

            Get["plugins"] = x => View["plugins.cshtml", new IndexViewModel { RobotPlugins = Robot.RobotPlugins, RobotVersion = Robot.Version }];
            Post["plugins/update"] = parameters =>
            {
                var pluginName = this.Context.Request.Form.PluginName;
                var plugin = robot.RobotPlugins.FirstOrDefault(p => p.Name == pluginName);
                var configFileName = string.Empty;
                if (plugin != null)
                {
                    configFileName = plugin.MakeConfigFileName();
                }

                var settings = this.Bind<List<SettingsModel>>();
                foreach (var setting in settings)
                {
                    Robot.Settings.Set(setting.Key, setting.Value);

                    // not only Update App.config but also the corresponding config file
                    Robot.Settings.Set(setting.Key, setting.Value, configFileName);
                }

                return Response.AsRedirect("/nubot/plugins");
            };
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

        private class SettingsModel
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}