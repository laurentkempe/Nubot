namespace Nubot.Nancy
{
    using global::Nancy;
    using global::Nancy.TinyIoc;
    using Interfaces;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IRobot _robot;

        public Bootstrapper(IRobot robot)
        {
            _robot = robot;
        }

        /// <summary>
        /// Configures the container using AutoRegister followed by registration
        ///             of default INancyModuleCatalog and IRouteResolver.
        /// </summary>
        /// <param name="container">Container instance</param>
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register(_robot);
        }
    }
}