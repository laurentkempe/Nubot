namespace Nubot.Abstractions
{
    using System.Threading.Tasks;

    public interface IBrain
    {
        Task<T> GetAsync<T>(string key);

        Task<T> GetAsync<T>(string key, T defaultValue);

        Task SetAsync<T>(string key, T value);

        Task Remove<T>(string key);
    }
}