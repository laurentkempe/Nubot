namespace Nubot.Plugins.AppHarbor
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Interfaces;
    using Models;
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.TinyIoc;

    [Export(typeof(IRobotPlugin))]
    public class AppharborListener : NancyModule, IRobotPlugin
    {
        private readonly IRobot _robot;

        public AppharborListener()
            : base("/hubot/appharbor")
        {
            Name = "Appharbor Listener";

            _robot = TinyIoCContainer.Current.Resolve<IRobot>();

            Post["/"] = x =>
            {
                var model = this.Bind<AppharborModel>();

                var message = string.Format("Your application {0} has been deployed with status: {1}", model.Application.Name, model.Build.Status);
                _robot.Message(message);

                return HttpStatusCode.OK;
            };
        }

        public string Name { get; private set; }

        public IEnumerable<string> HelpMessages { get; private set; }

        public void Respond(string message)
        {
        }
    }
}