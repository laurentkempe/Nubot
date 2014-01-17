namespace Nubot.Settings
{
    using System.Configuration;
    using Interfaces;

    public class AppSettings : ISettings
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}