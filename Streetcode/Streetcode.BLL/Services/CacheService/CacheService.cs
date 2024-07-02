using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Streetcode.BLL.Services.Tokens;

namespace Streetcode.BLL.Services.CacheService
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cache;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<CacheService> _logger;
        private readonly TokensConfiguration _authSettings;

        public CacheService(IConnectionMultiplexer redis, ILogger<CacheService> logger, TokensConfiguration authSettings)
        {
            _redis = redis;
            _cache = _redis.GetDatabase();
            _logger = logger;
            _authSettings = authSettings;
        }

        public async Task InvalidateCacheAsync(string pattern)
        {
            try
            {
                var endpoints = _redis.GetEndPoints();
                foreach (var endpoint in endpoints)
                {
                    var server = _redis.GetServer(endpoint);
                    var keys = server.Keys(pattern: "*" + pattern + "*").ToArray();

                    if (keys.Any())
                    {
                        foreach (var key in keys)
                        {
                            await _cache.KeyDeleteAsync(key);
                            _logger.LogInformation($"Cache key deleted: {key}");
                        }

                        _logger.LogInformation($"{keys.Length} cache keys invalidated for pattern: {pattern}");
                    }
                    else
                    {
                        _logger.LogInformation($"No cache keys found for pattern: {pattern}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while invalidating cache for pattern: {pattern}");
            }
        }

        public async Task SetCacheAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                var wasSetSuccessfully = await _cache.StringSetAsync(key, value, expiry);
                _logger.LogInformation($"Cache '{wasSetSuccessfully}' set for key: '{key}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting cache for key: {key}");
            }
        }

        public async Task<string?> GetCacheAsync(string key)
        {
            try
            {
                var value = await _cache.StringGetAsync(key);
                _logger.LogInformation($"Cache retrieved for key: {key}");
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving cache for key: {key}");
                return null;
            }
        }

        public async Task<bool> CacheKeyIsExist(string key)
        {
            if (await _cache.KeyExistsAsync(key))
            {
                _logger.LogInformation($"Cache key exists: {key}");
                return true;
            }

            return false;
        }
        
        public Task<bool> CacheKeyPatternExist(string pattern)
        {
            var endpoints = _redis.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                var keys = server.Keys(pattern: "*" + pattern + "*").ToArray();
                if (keys.Any())
                {
                    _logger.LogInformation($"Cache key pattern exists: {pattern}");
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
        
        public async Task DeleteCacheAsync(string key)
        {
            try
            {
                await _cache.KeyDeleteAsync(key);
                _logger.LogInformation($"Cache key deleted: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting cache for key: {key}");
            }
        }

        public async Task<bool> SetBlacklistedTokenAsync(string accessToken, string userId)
        {
            var key = $"blacklisted:{accessToken}";
            var isSuccess = await _cache.StringSetAsync(key, userId, TimeSpan.FromMinutes(_authSettings.AccessTokenExpirationMinutes));
            return isSuccess;
        }

        public async Task<bool> IsBlacklistedTokenAsync(string accessToken)
        {
            var key = $"blacklisted:{accessToken}";
            var isBlacklisted = await _cache.KeyExistsAsync(key);
            return isBlacklisted;
        }
    }
}
