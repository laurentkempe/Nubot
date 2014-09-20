namespace Nubot.Interfaces
{
    public interface ISettings
    {
        string Get(string key);
        void Set(string key, string value);
        void Create(string key, string value);
    }
}