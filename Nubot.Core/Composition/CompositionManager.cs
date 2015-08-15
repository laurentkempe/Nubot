namespace Nubot.Core.Composition
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Abstractions;
    using log4net;
    using MefContrib.Hosting.Interception;
    using MefContrib.Hosting.Interception.Configuration;

    public class CompositionManager
    {
        private readonly IRobot _robot;

        //LAURENT: TODO remove the static
        private static readonly string ExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string PluginsDirectory = Path.Combine(ExecutingDirectory, RobotPluginBase.BasePluginsDirectory);
        private static readonly string AdaptersDirectory = Path.Combine(ExecutingDirectory, AdapterBase.BaseAdapterDirectory);

        private ApplicationCatalog _applicationCatalog;
        private DirectoryCatalog _adapterdirectoryCatalog;
        private DirectoryCatalog _pluginsdirectoryCatalog;

        public CompositionManager(IRobot robot)
        {
            _robot = robot;
        }

        public void Compose()
        {
            if (!Directory.Exists(PluginsDirectory))
            {
                Directory.CreateDirectory(PluginsDirectory);
            }

            LoadAdapterAndPlugins();
        }

        private void LoadAdapterAndPlugins()
        {
            var interceptingCatalog = GetInterceptionCatalog();

            try
            {
                var container = new CompositionContainer(interceptingCatalog);
                container.ComposeExportedValue(_robot);
                container.ComposeParts(_robot);

                ShowLoaded(_applicationCatalog, "Loaded the following Nubot plugins");
                ShowLoaded(_adapterdirectoryCatalog, "Loaded the following adapter");
                ShowLoaded(_pluginsdirectoryCatalog, "Loaded the following plugins");
            }
            catch (ReflectionTypeLoadException e)
            {
                var logger = LogManager.GetLogger("Robot");

                if (logger.IsDebugEnabled) logger.Debug(e.LoaderExceptions);
            }
        }

        private ComposablePartCatalog GetInterceptionCatalog()
        {
            _applicationCatalog = new ApplicationCatalog();
            _adapterdirectoryCatalog = new DirectoryCatalog(AdaptersDirectory);
            _pluginsdirectoryCatalog = new DirectoryCatalog(PluginsDirectory);

            var catalog = new AggregateCatalog(_applicationCatalog, _adapterdirectoryCatalog, _pluginsdirectoryCatalog);

            var cfg = new InterceptionConfiguration().AddInterceptionCriteria(
                            new PredicateInterceptionCriteria(
                                new CopyConfigInterceptor(_robot.Settings),
                                def => def.ExportDefinitions.First().ContractName.Contains("IAdapter") ||
                                       def.ExportDefinitions.First().ContractName.Contains("IRobotPlugin")));

            return new InterceptingCatalog(catalog, cfg);
        }

        private void ShowLoaded(ComposablePartCatalog catalog, string message)
        {
            var builder = new StringBuilder();
            builder.AppendLine(message);
            foreach (var part in catalog.Parts)
            {
                builder.AppendFormat("\t{0}\n", part);
            }

            _robot.Logger.WriteLine(builder.ToString());
        }

        public void Refresh()
        {
            _pluginsdirectoryCatalog.Refresh();
        }
    }
}