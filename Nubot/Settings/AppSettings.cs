namespace Nubot.Settings
{
    using System.Configuration;
    using Interfaces;

    public class AppSettings : ISettings
    {
        public string Get(string key, string path = "")
        {
            Configuration config = null;
            if (!string.IsNullOrEmpty(path))
            {
                var mapping = new ExeConfigurationFileMap { ExeConfigFilename = path };
                config = ConfigurationManager.OpenMappedExeConfiguration(mapping, ConfigurationUserLevel.None);
            }
            else
            {
                config = ConfigurationManager.OpenExeConfiguration(string.Empty);
            }

            return config.AppSettings.Settings[key].Value;
        }

        public void Set(string key, string value, string path = "")
        {
            Configuration config = null;
            if (!string.IsNullOrEmpty(path))
            {
                var mapping = new ExeConfigurationFileMap { ExeConfigFilename = path };
                config = ConfigurationManager.OpenMappedExeConfiguration(mapping, ConfigurationUserLevel.None);
            }
            else
            {
                config = ConfigurationManager.OpenExeConfiguration(string.Empty);
            }

            config.AppSettings.Settings[key].Value = value;
            UpdateConfig(config);
        }

        // see: http://social.msdn.microsoft.com/Forums/vstudio/en-US/d68a872e-14bc-414a-82c4-d1035a11b4a8/how-do-i-updateinsertremove-the-config-file-during-runtime
        public void Create(string key, string value, string path = "")
        {
            Configuration config = null;
            if (!string.IsNullOrEmpty(path))
            {
                var mapping = new ExeConfigurationFileMap { ExeConfigFilename = path };
                config = ConfigurationManager.OpenMappedExeConfiguration(mapping, ConfigurationUserLevel.None);
            }
            else
            {
                config = ConfigurationManager.OpenExeConfiguration(string.Empty);
            }

            config.AppSettings.Settings.Add(key, value);
            UpdateConfig(config);
        }

        private void UpdateConfig(Configuration config)
        {
            // Save the configuration file.
            config.Save(ConfigurationSaveMode.Modified, true);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}