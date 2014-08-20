namespace Nubot.Plugins.Samples.TeamCity
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Interfaces;
    using Nancy;
    using Nancy.ModelBinding;
    using Model;

    [Export(typeof(IRobotPlugin))]
    public class TeamCityListener : HttpPluginBase
    {
        private readonly IEnumerable<IPluginSetting> _settings;

        [ImportingConstructor]
        public TeamCityListener(IRobot robot)
            : base("TeamCity Listener", "/teamcity", robot)
        {
            _settings = new List<IPluginSetting>
            {
                new PluginSetting(Robot, "TeamCityNotifyRoomName"),
                new PluginSetting(Robot, "TeamCityHipchatAuthToken")
            };

            Post["/"] = x =>
            {
                var model = this.Bind<TeamCityModel>(new BindingConfig {IgnoreErrors = true, BodyOnly = true});

                Robot.Emit("TeamCityBuild", model);

                return HttpStatusCode.OK;
            };
        }

        public override IEnumerable<IPluginSetting> Settings
        {
            get { return _settings; }
        }
    }
}