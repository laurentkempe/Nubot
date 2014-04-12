namespace Nubot
{
    using Annotations;
    using Owin;

    [UsedImplicitly]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }
    }
}