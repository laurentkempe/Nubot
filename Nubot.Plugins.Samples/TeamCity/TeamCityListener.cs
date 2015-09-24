namespace Nubot.Plugins.Samples.TeamCity
{
    using System.ComponentModel.Composition;
    using Abstractions;
    using Model;
    using Nancy;
    using Nancy.ModelBinding;

    [Export(typeof(IRobotPlugin))]
    public class TeamCityListener : HttpPluginBase
    {
        [ImportingConstructor]
        public TeamCityListener(IRobot robot)
            : base("TeamCity Listener", "/teamcity", robot)
        {
            Post["/"] = x =>
            {
                var model = this.Bind<TeamCityModel>(new BindingConfig {IgnoreErrors = true, BodyOnly = true});

                Robot.Messenger.Publish("TeamCity.BuildStatus", model);

                return HttpStatusCode.OK;
            };
        }
    }
}
