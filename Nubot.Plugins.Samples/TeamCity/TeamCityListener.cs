namespace Nubot.Plugins.Samples.TeamCity
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Text;
    using Abstractions;
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
                new PluginSetting(Robot, this, "TeamCityNotifyRoomName"),
                new PluginSetting(Robot, this, "TeamCityHipchatAuthToken")
            };

            Post["/"] = x =>
            {
                var model = this.Bind<TeamCityModel>(new BindingConfig {IgnoreErrors = true, BodyOnly = true});

                Robot.SendNotification(
                    Robot.Settings.Get("TeamCityNotifyRoomName").Trim(),
                    Robot.Settings.Get("TeamCityHipchatAuthToken").Trim(), 
                    BuildMessage(model),
                    true);

                return HttpStatusCode.OK;
            };
        }

        public override IEnumerable<IPluginSetting> Settings
        {
            get { return _settings; }
        }

        private string BuildMessage(TeamCityModel model)
        {
            var stringBuilder = new StringBuilder();

            var buildStatusHtml = model.build.buildStatusHtml.Replace(@"<span class=""tcWebHooksMessage"">", "").Replace("</span>", "");

            stringBuilder
                .Append(buildStatusHtml);

            return stringBuilder.ToString();
        }
    }
}