namespace Nubot.Core
{
    using Abstractions;
    using global::Nancy;
    using global::Nancy.Owin;
    using log4net;
    using Nancy;
    using Owin;
    using Properties;

    [UsedImplicitly]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var robot = Helper.GetConfiguredContainer().Resolve<IRobot>();

            var logger = LogManager.GetLogger("Robot");

            app
                .UseSimpleLogger(logger)
                .UseNancy(new NancyOptions
            {
                Bootstrapper = new Bootstrapper(robot)
            });

            StaticConfiguration.DisableErrorTraces = false;
        }
    }
}