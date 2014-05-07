namespace Nubot.Plugins.AppHarbor
{
    using System.ComponentModel.Composition;
    using Interfaces;
    using Models;
    using Nancy;
    using Nancy.ModelBinding;

    [Export(typeof(IRobotPlugin))]
    public class AppharborListener : HttpPluginBase
    {
        [ImportingConstructor]
        public AppharborListener(IRobot robot)
            : base("Appharbor Listener", "/appharbor", robot)
        {
            Post["/"] = x =>
            {
                var model = this.Bind<AppharborModel>();

                var message = string.Format("Your application {0} has been deployed with status: {1}", model.Application.Name, model.Build.Status);
                Robot.Message(message);

                return HttpStatusCode.OK;
            };

            Get["index"] = x => View[string.Format("plugins{0}/views/", ModulePath) + "index.html"];
        }
    }
}