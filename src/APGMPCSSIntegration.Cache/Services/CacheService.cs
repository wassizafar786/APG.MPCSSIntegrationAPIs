using APGDigitalIntegration.Cache.Interfaces;
using APGMPCSSIntegration.Cache.Options.v1;
using Microsoft.Extensions.Options;
using NetDevPack.Messaging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace APGDigitalIntegration.Cache.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly string _hostName;
        private readonly string _port;
        private readonly string _password;
        private readonly bool _enabled;
        private readonly string _username;
        private bool _isRedisDown;
        private readonly int _redisDb;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly ConnectionMultiplexer _redis;

        public delegate void SubscriberDelegate(string message);
        public RedisCacheService(IOptions<RedisConfiguration> redisMqOptions)
        {
            _hostName = redisMqOptions.Value.Hostname;
            _port = redisMqOptions.Value.Port;
            _password = redisMqOptions.Value.Password;
            _enabled = redisMqOptions.Value.Enabled;
            _isRedisDown = false;
            _redisDb=redisMqOptions.Value.RedisDb;

            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            var connectionSettings = new ConfigurationOptions()
            {
                AbortOnConnectFail = false,
                EndPoints = { $"{_hostName}:{_port}" },
                Password = _password,
                SyncTimeout = 5000,
                AsyncTimeout = 5000,
                ConnectTimeout = 1000,
                ConnectRetry = 5,
                ReconnectRetryPolicy = new LinearRetry(1000),
                BacklogPolicy = BacklogPolicy.FailFast,
                DefaultDatabase=_redisDb

            };

            _redis = ConnectionMultiplexer.Connect(connectionSettings);
            _isRedisDown = _redis.IsConnected == false;


            _redis.ConnectionRestored += (sender, args) =>
            {
                _isRedisDown = false;
            };

            _redis.ConnectionFailed += (sender, args) =>
            {
                _isRedisDown = true;
            };
        }

        /// <summary>
        /// Remove Any Keys that start with: keyPattern
        /// <br/>
        /// </summary>
        /// <param name="keyPattern"></param>
        public void RemoveAllKeysThatStartsWith(string keyPattern)
        {
            var redisUrl = $"{_hostName}:{_port}";
            if (_enabled == false || string.IsNullOrWhiteSpace(redisUrl) || string.IsNullOrWhiteSpace(keyPattern) || _isRedisDown)
                return;

            try
            {
                var server = _redis.GetServer(redisUrl);
                var db = _redis.GetDatabase();

                var pattern = $"{keyPattern}*";

                foreach (var key in server.Keys(pattern: pattern, pageSize: 1000))
                    db.KeyDelete(key);

                db.KeyDelete(server.Keys(pattern: pattern).ToArray());
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<TData> GetOrAddAsync<TData, TFactoryParam>(string key, Func<TFactoryParam, Task<TData>> factory, TFactoryParam factoryParameter)
        {
            var redisUrl = $"{_hostName}:{_port}";
            if (_enabled == false || string.IsNullOrWhiteSpace(redisUrl) || _isRedisDown)
                return default;

            try
            {
                var db = _redis.GetDatabase();
                var cachedData = await db.StringGetAsync(key);

                if (cachedData.HasValue)
                    return JsonConvert.DeserializeObject<TData>(cachedData.ToString(), _jsonSerializerSettings);

                var data = await factory(factoryParameter);
                if (data == null || data.Equals(default))
                    return default;

                var serializedData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);

                await db.StringSetAsync(key, serializedData);

                return data;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// AddKeyWithValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        public void AddKeyWithValue<TValue>(string key, TValue value)
        {
            if (_enabled == false || _isRedisDown)
                return;

            try
            {
                var serializedData = JsonConvert.SerializeObject(value, _jsonSerializerSettings);

                var db = _redis.GetDatabase();
                db.StringSet(key, serializedData);
            }
            catch (Exception ex)
            {
                // Should Log Exception.
            }
        }

        public async Task AddKeyWithValueAsync<TValue>(string key, TValue value)
        {
            if (_enabled == false || _isRedisDown)
                return;

            try
            {
                var serializedData = JsonConvert.SerializeObject(value, _jsonSerializerSettings);

                var db = _redis.GetDatabase();
                await db.StringSetAsync(key, serializedData);
            }
            catch (Exception ex)
            {
                // Should Log Exception.
            }
        }

        /// <summary>
        /// AddListAsBulk
        /// </summary>
        /// <param name="cachedItems"></param>
        /// <param name="keySelector"></param>
        /// <typeparam name="TItem"></typeparam>
        public void AddListAsBulk<TItem>(List<TItem> cachedItems, Func<TItem, string> keySelector)
        {
            if (_enabled == false || cachedItems is null || cachedItems.Any() == false || _isRedisDown)
                return;

            try
            {
                var db = _redis.GetDatabase();

                const int batchSize = 100;
                var batch = new List<KeyValuePair<RedisKey, RedisValue>>(batchSize);
                foreach (var item in cachedItems)
                {
                    batch.Add(new KeyValuePair<RedisKey, RedisValue>(keySelector.Invoke(item), JsonConvert.SerializeObject(item, _jsonSerializerSettings)));
                    if (batch.Count != batchSize)
                        continue;

                    db.StringSet(batch.ToArray());
                    batch.Clear();
                }

                if (batch.Count != 0) // final batch
                    db.StringSet(batch.ToArray());
            }
            catch (Exception ex)
            {
            }
        }

        public async Task AddListAsBulkAsync<TItem>(List<TItem> cachedItems, Func<TItem, string> keySelector)
        {
            if (_enabled == false || cachedItems is null || cachedItems.Any() == false || _isRedisDown)
                return;

            try
            {
                var db = _redis.GetDatabase();

                const int batchSize = 2;
                var batch = new List<KeyValuePair<RedisKey, RedisValue>>(batchSize);
                foreach (var item in cachedItems)
                {
                    batch.Add(new KeyValuePair<RedisKey, RedisValue>(keySelector.Invoke(item), JsonConvert.SerializeObject(item, _jsonSerializerSettings)));
                    if (batch.Count != batchSize)
                        continue;

                    await db.StringSetAsync(batch.ToArray());
                    batch.Clear();
                }

                if (batch.Count != 0) // final batch
                    await db.StringSetAsync(batch.ToArray());
            }
            catch (Exception ex)
            {
                // Exception Should be logged here.
            }
        }


        public TData GetValueByKey<TData>(string key)
        {
            if (_enabled == false || _isRedisDown)
                return default;

            try
            {
                var db = _redis.GetDatabase();
                var cachedData = db.StringGet(key);

                return cachedData.HasValue
                    ? JsonConvert.DeserializeObject<TData>(cachedData, _jsonSerializerSettings)
                    : default;
            }

            catch (Exception ex)
            {
                return default;
            }
        }

        public async Task<TData> GetValueByKeyAsync<TData>(string key)
        {
            if (_enabled == false || _isRedisDown)
                return default;


            try
            {
                var db = _redis.GetDatabase();
                var cachedData = await db.StringGetAsync(key);

                return cachedData.HasValue
                    ? JsonConvert.DeserializeObject<TData>(cachedData.ToString(), _jsonSerializerSettings)
                    : default;
            }

            catch (Exception ex)
            {
                return default;
            }
        }

        public void RemoveSingleKeyFromCache(string key)
        {
            if (_enabled == false || _isRedisDown)
                return;

            try
            {
                var db = _redis.GetDatabase();
                db.KeyDelete(key);
            }
            catch (Exception ex)
            {
                // Should Log Exception
            }
        }

        public async Task RemoveSingleKeyFromCacheAsync(string key)
        {
            if (_enabled == false || _isRedisDown)
                return;

            try
            {
                await _redis.GetDatabase().KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                // Should Log Exception
            }
        }

        public async Task<bool> KeyExistAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync(key);
        }

        public async Task GetSubScribe(string messageId, SubscriberDelegate keySelector)
        {
            await _redis.GetSubscriber().SubscribeAsync(messageId, (channel, messgae) =>
              {
                  keySelector.Invoke(messgae);
              });

        }

        public async Task Publish(string messageId, string publishMessage)
        {
            await _redis.GetSubscriber().PublishAsync(messageId, publishMessage);
        }

        [Obsolete("Implementation Not Completed or tested")]
        public List<TData> GetMultiValuesByKeys<TData>(List<string> keys)
        {
            throw new NotImplementedException();

            var redisUrl = $"{_hostName}:{_port}";
            if (_enabled == false || string.IsNullOrWhiteSpace(redisUrl) || _isRedisDown)
                return default;

            try
            {
                var db = _redis.GetDatabase();

                var redisKeys = keys.Select(key => new RedisKey(key)).ToArray();
                var values = db.StringGet(redisKeys);
                return values.Select(value => JsonConvert.DeserializeObject<TData>(value.ToString(), _jsonSerializerSettings)).ToList();
            }

            catch (Exception)
            {
                // should Log here.
                return default;
            }
        }

        public bool IsConnected()
        {
            return !_isRedisDown;
        }
    }
}