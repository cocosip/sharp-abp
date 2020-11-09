using FreeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.FreeRedis;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Caching.FreeRedis
{
    //[DisableConventionalRegistration]
    public class AbpFreeRedisCache : IDistributedCache
    {
        protected FreeRedisCacheOptions Options { get; }
        protected RedisClient RedisClient { get; }
        public AbpFreeRedisCache(IOptions<FreeRedisCacheOptions> options, IRedisClientFactory redisClientFactory)
        {
            Options = options.Value;
            RedisClient = redisClientFactory.Get(Options.Name);
        }

        private const string SetScript = (@"
                redis.call('HMSET', KEYS[1], 'absexp', ARGV[1], 'sldexp', ARGV[2], 'data', ARGV[4])
                if ARGV[3] ~= '-1' then
                  redis.call('EXPIRE', KEYS[1], ARGV[3])
                end
                return 1");
        private const string AbsoluteExpirationKey = "absexp";
        private const string SlidingExpirationKey = "sldexp";
        private const string DataKey = "data";
        private const long NotPresent = -1;
 
        public byte[] Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return GetAndRefresh(key, getData: true);
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            token.ThrowIfCancellationRequested();

            return await GetAndRefreshAsync(key, getData: true, token: token);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var creationTime = DateTimeOffset.UtcNow;

            var absoluteExpiration = GetAbsoluteExpiration(creationTime, options);
            _ = RedisClient.Eval(SetScript, new string[] { key },
                new object[]
                {
                        absoluteExpiration?.Ticks ?? NotPresent,
                        options.SlidingExpiration?.Ticks ?? NotPresent,
                        GetExpirationInSeconds(creationTime, absoluteExpiration, options) ?? NotPresent,
                        value
                });
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            token.ThrowIfCancellationRequested();

            var creationTime = DateTimeOffset.UtcNow;

            var absoluteExpiration = GetAbsoluteExpiration(creationTime, options);
            await RedisClient.EvalAsync(SetScript, new string[] { key },
                new object[]
                {
                        absoluteExpiration?.Ticks ?? NotPresent,
                        options.SlidingExpiration?.Ticks ?? NotPresent,
                        GetExpirationInSeconds(creationTime, absoluteExpiration, options) ?? NotPresent,
                        value
                });
        }

        public void Refresh(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            GetAndRefresh(key, getData: false);
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            token.ThrowIfCancellationRequested();

            await GetAndRefreshAsync(key, getData: false, token: token);
        }

        private byte[] GetAndRefresh(string key, bool getData)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // This also resets the LRU status as desired.
            // TODO: Can this be done in one operation on the server side? Probably, the trick would just be the DateTimeOffset math.
            object[] results;
            byte[] value = null;
            if (getData)
            {
                var ret = RedisClient.HMGet<byte[]>(key, AbsoluteExpirationKey, SlidingExpirationKey, DataKey);
                results = new object[] { ret[0] == null ? null : Encoding.UTF8.GetString(ret[0]), ret[1] == null ? null : Encoding.UTF8.GetString(ret[1]), value = ret[2] };
            }
            else
            {
                results = RedisClient.HMGet(key, AbsoluteExpirationKey, SlidingExpirationKey);
            }

            // TODO: Error handling
            if (results.Length >= 2)
            {
                MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr);
                Refresh(key, absExpr, sldExpr);
            }

            if (results.Length >= 3)
            {
                return value;
            }

            return null;
        }

        private async Task<byte[]> GetAndRefreshAsync(string key, bool getData, CancellationToken token = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            token.ThrowIfCancellationRequested();

            // This also resets the LRU status as desired.
            // TODO: Can this be done in one operation on the server side? Probably, the trick would just be the DateTimeOffset math.
            object[] results;
            byte[] value = null;
            if (getData)
            {
                var ret = await RedisClient.HMGetAsync<byte[]>(key, AbsoluteExpirationKey, SlidingExpirationKey, DataKey);
                results = new object[] { ret[0] == null ? null : Encoding.UTF8.GetString(ret[0]), ret[1] == null ? null : Encoding.UTF8.GetString(ret[1]), value = ret[2] };
            }
            else
            {
                results = await RedisClient.HMGetAsync(key, AbsoluteExpirationKey, SlidingExpirationKey);
            }

            // TODO: Error handling
            if (results.Length >= 2)
            {
                MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr);
                await RefreshAsync(key, absExpr, sldExpr, token);
            }

            if (results.Length >= 3)
            {
                return value;
            }

            return null;
        }

        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            RedisClient.Del(key.Split('|'));
            // TODO: Error handling
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await RedisClient.DelAsync(key.Split('|'));
            // TODO: Error handling
        }

        private void MapMetadata(object[] results, out DateTimeOffset? absoluteExpiration, out TimeSpan? slidingExpiration)
        {
            absoluteExpiration = null;
            slidingExpiration = null;
            if (long.TryParse(results[0]?.ToString(), out var absoluteExpirationTicks) && absoluteExpirationTicks != NotPresent)
            {
                absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks, TimeSpan.Zero);
            }
            if (long.TryParse(results[1]?.ToString(), out var slidingExpirationTicks) && slidingExpirationTicks != NotPresent)
            {
                slidingExpiration = new TimeSpan(slidingExpirationTicks);
            }
        }

        private void Refresh(string key, DateTimeOffset? absExpr, TimeSpan? sldExpr)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (sldExpr.HasValue)
            {

                // Note Refresh has no effect if there is just an absolute expiration (or neither).
                TimeSpan? expr;
                if (absExpr.HasValue)
                {
                    var relExpr = absExpr.Value - DateTimeOffset.Now;
                    expr = relExpr <= sldExpr.Value ? relExpr : sldExpr;
                }
                else
                {
                    expr = sldExpr;
                }
                RedisClient.Expire(key, expr?.Seconds ?? 0);
                // TODO: Error handling
            }
        }

        private async Task RefreshAsync(string key, DateTimeOffset? absExpr, TimeSpan? sldExpr, CancellationToken token = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            token.ThrowIfCancellationRequested();
            if (sldExpr.HasValue)
            {

                // Note Refresh has no effect if there is just an absolute expiration (or neither).
                TimeSpan? expr;
                if (absExpr.HasValue)
                {
                    var relExpr = absExpr.Value - DateTimeOffset.Now;
                    expr = relExpr <= sldExpr.Value ? relExpr : sldExpr;
                }
                else
                {
                    expr = sldExpr;
                }
                await RedisClient.ExpireAsync(key, expr?.Seconds ?? 0);
                // TODO: Error handling
            }
        }

        private static long? GetExpirationInSeconds(DateTimeOffset creationTime, DateTimeOffset? absoluteExpiration, DistributedCacheEntryOptions options)
        {
            if (absoluteExpiration.HasValue && options.SlidingExpiration.HasValue)
            {
                return (long)Math.Min(
                    (absoluteExpiration.Value - creationTime).TotalSeconds,
                    options.SlidingExpiration.Value.TotalSeconds);
            }
            else if (absoluteExpiration.HasValue)
            {
                return (long)(absoluteExpiration.Value - creationTime).TotalSeconds;
            }
            else if (options.SlidingExpiration.HasValue)
            {
                return (long)options.SlidingExpiration.Value.TotalSeconds;
            }
            return null;
        }

        private static DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset creationTime, DistributedCacheEntryOptions options)
        {
            if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(DistributedCacheEntryOptions.AbsoluteExpiration),
                    options.AbsoluteExpiration.Value,
                    "The absolute expiration value must be in the future.");
            }
            var absoluteExpiration = options.AbsoluteExpiration;
            if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                absoluteExpiration = creationTime + options.AbsoluteExpirationRelativeToNow;
            }

            return absoluteExpiration;
        }
    }
}
