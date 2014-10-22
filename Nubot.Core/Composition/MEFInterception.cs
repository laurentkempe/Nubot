namespace Nubot.Core.Composition
{
    using System.Configuration;
    using Abstractions;
    using MefContrib.Hosting.Interception;

    // see: http://pwlodek.blogspot.com/2010/11/introduction-to-interceptingcatalog.html
    public class CopyConfigInterceptor : IExportedValueInterceptor
    {
        private readonly ISettings _settings;

        public CopyConfigInterceptor(ISettings settings)
        {
            _settings = settings;
        }

        public object Intercept(object value)
        {
            var adapter = value as IAdapter;
            if (adapter != null)
            {
                var configFileName = adapter.MakeConfigFileName();
                if (configFileName != null) ReadAndCopy(configFileName);
            }

            var plugin = value as IRobotPlugin;
            if (plugin != null)
            {
                var configFileName = plugin.MakeConfigFileName();
                if (configFileName != null) ReadAndCopy(configFileName);
            }

            return value;
        }

        private void ReadAndCopy(string configFileName)
        {
            var mapping = new ExeConfigurationFileMap { ExeConfigFilename = configFileName };
            var config = ConfigurationManager.OpenMappedExeConfiguration(mapping, ConfigurationUserLevel.None);

            foreach (var key in config.AppSettings.Settings.AllKeys)
            {
                _settings.Create(key, config.AppSettings.Settings[key].Value);
            }
        }
    }
}
