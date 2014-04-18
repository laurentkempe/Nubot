namespace Nubot.Router
{
    using Annotations;
    using Interfaces;
    using Nancy.Owin;
    using Owin;

    [UsedImplicitly]
    public class Startup
    {
        [UsedImplicitly]
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(new NancyOptions
            {
                Bootstrapper = new Bootstrapper(Helper.GetConfiguredContainer().Resolve<IRobot>())
            });
        }
    }
}