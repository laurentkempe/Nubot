namespace Nubot.Core.Brains
{
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using Akavache;

    public class AkavacheBrain : IBrain
    {
        private readonly IBlobCache _blobCache;

        public AkavacheBrain()
        {
            BlobCache.ApplicationName = "Nubot";

            _blobCache = BlobCache.LocalMachine;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await _blobCache.GetObject<T>(key);
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue)
        {
            T cached;
            try
            {
                cached = await _blobCache.GetObject<T>(key); ;
            }
            catch (KeyNotFoundException)
            {
                cached = defaultValue;
                await SetAsync(key, cached);
            }

            return cached;
        }

        public async Task SetAsync<T>(string key, T value)
        {
            await _blobCache.InsertObject(key, value);
        }

        public async Task Remove<T>(string key)
        {
            await _blobCache.Invalidate(key);
        }
    }
}