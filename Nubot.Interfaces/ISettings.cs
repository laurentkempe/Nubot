namespace Nubot.Interfaces
{
    public interface ISettings
    {
        string Get(string key);
        void Set(string key, string value);
    }
}