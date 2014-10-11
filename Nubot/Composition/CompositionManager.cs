namespace Nubot.Composition
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Linq;
    using Interfaces;
    using MefContrib.Hosting.Interception.Configuration;
    using MefContrib.Hosting.Interception;
    using Settings;

    public class CompositionManager
    {
        private readonly Robot _robot;
        private static readonly string _executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string _pluginsDirectory = Path.Combine(_executingDirectory, RobotPluginBase.BasePluginsDirectory);
        private static readonly string _adaptersDirectory = Path.Combine(_executingDirectory, AdapterBase.BaseAdapterDirectory);

        private ApplicationCatalog _applicationCatalog;
        private DirectoryCatalog _adapterdirectoryCatalog;
        private DirectoryCatalog _pluginsdirectoryCatalog;

        public CompositionManager(Robot robot)
        {
            _robot = robot;
        }

        public void Compose()
        {
            if (!Directory.Exists(_pluginsDirectory))
            {
                Directory.CreateDirectory(_pluginsDirectory);
            }

            LoadAdapterAndPlugins();
        }

        private void LoadAdapterAndPlugins()
        {
            var interceptingCatalog = GetInterceptionCatalog();

            var container = new CompositionContainer(interceptingCatalog);
            container.ComposeExportedValue<IRobot>(_robot);
            container.ComposeParts(_robot);

            // log loadings
            ShowLoadedPlugins(_applicationCatalog, "Loaded the following Nubot plugins");
            ShowLoadedPlugins(_adapterdirectoryCatalog, "Loaded the following adapter");
            ShowLoadedPlugins(_pluginsdirectoryCatalog, "Loaded the following plugins");
        }

        private ComposablePartCatalog GetInterceptionCatalog()
        {
            _applicationCatalog = new ApplicationCatalog();
            _adapterdirectoryCatalog = new DirectoryCatalog(_adaptersDirectory);
            _pluginsdirectoryCatalog = new DirectoryCatalog(_pluginsDirectory);

            var catalog = new AggregateCatalog(_applicationCatalog, _adapterdirectoryCatalog, _pluginsdirectoryCatalog);

            var cfg = new InterceptionConfiguration().AddInterceptionCriteria(
                            new PredicateInterceptionCriteria(
                                new CopyConfigInterceptor(), 
                                def => def.ExportDefinitions.First().ContractName.Contains("IAdapter") ||
                                       def.ExportDefinitions.First().ContractName.Contains("IRobotPlugin"))); 

            // Create the InterceptingCatalog with above configuration
            var interceptingCatalog = new InterceptingCatalog(catalog, cfg);
            return interceptingCatalog;
        }

        private void ShowLoadedPlugins(ComposablePartCatalog catalog, string message)
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