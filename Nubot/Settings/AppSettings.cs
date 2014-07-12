namespace Nubot.Settings
{
    using System.Configuration;
    using Interfaces;

    public class AppSettings : ISettings
    {
        public string Get(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(string.Empty);
            return config.AppSettings.Settings[key].Value;
        }

        public void Set(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(string.Empty);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Full);
        }
    }
}