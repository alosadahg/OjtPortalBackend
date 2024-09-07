using Microsoft.Extensions.Caching.Memory;
using OjtPortal.Entities;
using System.Collections.Concurrent;

namespace OjtPortal.Services
{
    public interface ICacheService
    {
        void AddToCache(string key, string index, object record);
        void AddToPermanentCache(string key, object record);
        T? GetFromCache<T>(string key, string index) where T : class;
        T? GetFromPermanentCache<T>(string key) where T : class;
        void RemoveFromCache(string key, string index);
        void RemoveFromPermanentCache(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;
        private readonly ConcurrentDictionary<string, object> _permanentCache = new ConcurrentDictionary<string, object>();

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public T? GetFromCache<T>(string key, string index) where T : class
        {
            var find = !string.IsNullOrEmpty(index) ? $"{key}:{index}" : $"{key}";

            if (_cache.TryGetValue(find, out var existing))
            {
                _logger.LogInformation($"Cache hit for {find}");
                return existing as T;
            }

            _logger.LogInformation($"Cache miss for {find}. Fetching from database...");
            return default;
        }

        public void AddToCache(string key, string index, object record)
        {
            var newRecordKey = !string.IsNullOrEmpty(index) ? $"{key}:{index}" : $"{key}";
            var cacheOptions = GetCacheOptions();
            _cache.Set(newRecordKey, record, cacheOptions);
            _logger.LogInformation($"Cache set for {newRecordKey}.");
        }

        public void RemoveFromCache(string key, string index)
        {
            var newRecordKey = !string.IsNullOrEmpty(index) ? $"{key}:{index}" : $"{key}";
            var cacheOptions = GetCacheOptions();
            _cache.Remove(newRecordKey);
            _logger.LogInformation($"Cache removed for {newRecordKey}.");
        }

        public T? GetFromPermanentCache<T>(string key) where T : class
        {
            if (_permanentCache.TryGetValue(key, out var existing))
            {
                _logger.LogInformation($"Cache hit from concurrent map for {key}");
                return existing as T;
            }

            _logger.LogInformation($"Cache miss from concurrent map for {key}. Fetching from database...");
            return default;
        }

        public void AddToPermanentCache(string key, object record)
        {
            _permanentCache.AddOrUpdate(
                key,
                record,
                (existingKey, existingValue) => record
            );
            _logger.LogInformation($"Cache set to concurrent map for {key}.");
        }

        public void RemoveFromPermanentCache(string key)
        {
            _permanentCache.Remove(key, out var removed);
            _logger.LogInformation($"Cache removed from concurrent map for {key}.");
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions()
               .SetSlidingExpiration(TimeSpan.FromMinutes(5))
               .SetAbsoluteExpiration(TimeSpan.FromMinutes(20))
               .SetSize(1);
        }
    }
}
