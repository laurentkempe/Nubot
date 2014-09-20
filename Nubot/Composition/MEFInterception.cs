namespace Nubot.Settings
{
    using MefContrib.Hosting.Interception;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Nubot.Interfaces;
    using System.Configuration;
    using System.Reflection;
    using System.IO;

    // see: http://pwlodek.blogspot.com/2010/11/introduction-to-interceptingcatalog.html
    public class CopyConfigInterceptor : IExportedValueInterceptor
    {
        protected ISettings Settings = new AppSettings();

        public object Intercept(object value)
        {
            var adapter = value as IAdapter;
            if (adapter != null)
            {
                var adapterName = adapter.GetType().Name;
                var configFileName = MakeAdapterConfigFileName(adapterName);

                ReadAndCopy(configFileName);
            }

            var plugin = value as IRobotPlugin;
            if (plugin != null)
            {
                // TODO: add plugin handle here
            }

            // we don`t care value in fact
            return value;
        }

        private void ReadAndCopy(string configFileName)
        {
            // currently we only scan for adapters: "..\Output\bin\Debug\adapters\"
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(path, "adapters", configFileName);

            var mapping = new ExeConfigurationFileMap { ExeConfigFilename = filename };
            var config = ConfigurationManager.OpenMappedExeConfiguration(mapping, ConfigurationUserLevel.None);

            // let`s do the copy
            foreach (var key in config.AppSettings.Settings.AllKeys)
            {
                Settings.Create(key, config.AppSettings.Settings[key].Value);
            }
        }

        private string MakeAdapterConfigFileName(string adapterName)
        {
            return string.Format("{0}.config", adapterName);
        }

        private string MakePluginConfigFileName(string pluginName)
        {
            return string.Format("{0}.config", pluginName);
        }
    }
}
