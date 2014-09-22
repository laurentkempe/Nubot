namespace Nubot.Plugins.Samples.TeamCity
{
    using System.ComponentModel.Composition;
    using Interfaces;
    using Nancy;
    using Nancy.ModelBinding;
    using Model;

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

                Robot.Messenger.Emit("TeamCityBuild", model);

                return HttpStatusCode.OK;
            };
        }
    }
}