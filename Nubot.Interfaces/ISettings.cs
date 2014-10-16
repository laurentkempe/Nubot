namespace Nubot.Abstractions
{
    public interface ISettings
    {
        string Get(string key, string path = "");
        void Set(string key, string value, string path = "");
        void Create(string key, string value, string path = "");
    }
}