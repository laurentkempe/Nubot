namespace Nubot.Nancy
{
    using System.Linq;
    using global::Nancy;
    using global::Nancy.Conventions;
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

        //todo how do we do when we reload the plugins?
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            foreach (var staticPath in _robot.RobotPlugins.OfType<HttpPluginBase>().SelectMany(httpPlugin => httpPlugin.StaticPaths))
            {
                nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(staticPath.Item1, staticPath.Item2));
            }
        }
    }
}