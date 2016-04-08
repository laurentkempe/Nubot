namespace Nubot.Core.Settings
{
    using System;
    using System.Configuration;
    using System.Linq;
    using Abstractions;

    public class AppSettings : ISettings
    {
        public string Get(string key, string path = "")
        {
            var config = GetConfiguration(path);

            return config.AppSettings.Settings[key].Value.Trim();
        }

        public T Get<T>(string key, string path = "") where T : IConvertible
        {
            var configValue = Get(key, path);

            return (T) Convert.ChangeType(configValue, typeof (T));
        }

        public void Set(string key, string value, string path = "")
        {
            var config = GetConfiguration(path);

            config.AppSettings.Settings[key].Value = value;
            UpdateConfig(config);
        }

        // see: http://social.msdn.microsoft.com/Forums/vstudio/en-US/d68a872e-14bc-414a-82c4-d1035a11b4a8/how-do-i-updateinsertremove-the-config-file-during-runtime
        public void Create(string key, string value, string path = "")
        {
            var config = GetConfiguration(path);

            if (config.AppSettings.Settings.AllKeys.Contains(key)) return;

            config.AppSettings.Settings.Add(key, value);
            UpdateConfig(config);
        }

        private static Configuration GetConfiguration(string path)
        {
            Configuration config;
            if (string.IsNullOrEmpty(path))
            {
                config = ConfigurationManager.OpenExeConfiguration(string.Empty);
            }
            else
            {
                var mapping = new ExeConfigurationFileMap {ExeConfigFilename = path};
                config = ConfigurationManager.OpenMappedExeConfiguration(mapping, ConfigurationUserLevel.None);
            }
            return config;
        }

        private static void UpdateConfig(Configuration config)
        {
            // Save the configuration file.
            config.Save(ConfigurationSaveMode.Modified, true);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}