namespace Nubot.Plugins.Samples.AppHarbor
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net;
    using Abstractions;
    using Models;
    using Nancy.ModelBinding;

    [Export(typeof(IRobotPlugin))]
    public class AppharborListener : HttpPluginBase
    {
        private readonly IEnumerable<IPluginSetting> _settings;

        [ImportingConstructor]
        public AppharborListener(IRobot robot)
            : base("Appharbor Listener", "/appharbor", robot)
        {
            _settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, this, "AppharborListenerRoomName"),
            };

            Post["/"] = x =>
            {
                var model = this.Bind<AppharborModel>();

                var message = string.Format("Your application {0} has been deployed with status: {1}", model.Application.Name, model.Build.Status);

                var room = Robot.Settings.Get("AppharborListenerRoomName");

                Robot.MessageRoom(room, message);

                return HttpStatusCode.OK;
            };

            Get["index"] = x => View[string.Format("plugins{0}/views/", ModulePath) + "index.html"];
        }
        public override IEnumerable<IPluginSetting> Settings
        {
            get { return _settings; }
        }
    }
}