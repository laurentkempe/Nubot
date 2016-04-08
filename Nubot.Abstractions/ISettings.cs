namespace Nubot.Abstractions
{
    using System;

    public interface ISettings
    {
        string Get(string key, string path = "");
        T Get<T>(string key, string path = "") where T : IConvertible;
        void Set(string key, string value, string path = "");
        void Create(string key, string value, string path = "");
    }
}