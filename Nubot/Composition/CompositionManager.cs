namespace Nubot.Composition
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.IO;
    using System.Text;

    public class CompositionManager
    {
        private readonly Robot _robot;
        private readonly string _pluginsDirectory = string.Format("{0}\\plugins\\", Environment.CurrentDirectory);
        private readonly string _adaptersDirectory = string.Format("{0}\\adapters\\", Environment.CurrentDirectory);

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

            var pluginsdirectoryCatalog = new DirectoryCatalog(_pluginsDirectory);
            var adapterdirectoryCatalog = new DirectoryCatalog(_adaptersDirectory);
            var applicationCatalog = new ApplicationCatalog();
            var catalog = new AggregateCatalog(applicationCatalog, pluginsdirectoryCatalog, adapterdirectoryCatalog);

            var container = new CompositionContainer(catalog);
            container.ComposeParts(_robot);

            ShowLoadedPlugins(pluginsdirectoryCatalog, "Loaded the following plugins");
            ShowLoadedPlugins(adapterdirectoryCatalog, "Loaded the following adapter");
        }

        private static void ShowLoadedPlugins(ComposablePartCatalog catalog, string message)
        {
            var builder = new StringBuilder();
            builder.AppendLine(message);
            foreach (var part in catalog.Parts)
            {
                builder.AppendFormat("\t{0}\n", part);
            }

            Console.WriteLine(builder.ToString());
        }
    }
}