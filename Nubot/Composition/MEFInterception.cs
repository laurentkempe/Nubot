namespace Nubot.Core.Composition
{
    using System.Configuration;
    using Abstractions;
    using MefContrib.Hosting.Interception;
    using Settings;

    // see: http://pwlodek.blogspot.com/2010/11/introduction-to-interceptingcatalog.html
    public class CopyConfigInterceptor : IExportedValueInterceptor
    {
        protected ISettings Settings = new AppSettings();

        public object Intercept(object value)
        {
            var adapter = value as IAdapter;
            if (adapter != null)
            {
                var configFileName = adapter.MakeConfigFileName();
                ReadAndCopy(configFileName);
            }

            var plugin = value as IRobotPlugin;
            if (plugin != null)
            {
                var configFileName = plugin.MakeConfigFileName();
                ReadAndCopy(configFileName);
            }

            return value;
        }

        private void ReadAndCopy(string configFileName)
        {
            var mapping = new ExeConfigurationFileMap { ExeConfigFilename = configFileName };
            var config = ConfigurationManager.OpenMappedExeConfiguration(mapping, ConfigurationUserLevel.None);

            foreach (var key in config.AppSettings.Settings.AllKeys)
            {
                Settings.Create(key, config.AppSettings.Settings[key].Value);
            }
        }
    }
}
