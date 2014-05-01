namespace Nubot
{
    using Annotations;
    using global::Nancy.Owin;
    using Interfaces;
    using Nancy;
    using Owin;

    [UsedImplicitly]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(new NancyOptions
            {
                Bootstrapper = new Bootstrapper(Helper.GetConfiguredContainer().Resolve<IRobot>())
            });
        }
    }
}