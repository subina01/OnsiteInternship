using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace sample_redistestapp.RedisHelper
{
    public class RedisHelper
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisHelper(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase();
        }

        public void SetValue(string key, string value)
        {
            _db.StringSet(key, value);
        }

        public string? GetValue(string key)
        {
            return _db.StringGet(key);
        }
    }
}
