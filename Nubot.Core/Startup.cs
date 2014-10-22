namespace Nubot.Core
{
    using Abstractions;
    using global::Nancy;
    using global::Nancy.Owin;
    using Nancy;
    using Owin;
    using Properties;

    [UsedImplicitly]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(new NancyOptions
            {
                Bootstrapper = new Bootstrapper(Helper.GetConfiguredContainer().Resolve<IRobot>())
            });

            StaticConfiguration.DisableErrorTraces = false;
        }
    }
}