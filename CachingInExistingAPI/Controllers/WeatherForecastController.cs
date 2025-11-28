using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CachingInExistingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly IDatabase _redis;

        public WeatherForecastController(HttpClient client, IConnectionMultiplexer muxer)
        {
            _client = client;
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weatherCachingApp", "1.0"));
            _redis = muxer.GetDatabase();
        }

        private async Task<string> GetForecast(double latitude, double longitude)
        {
            try
            {
                var pointsRequestQuery = $"https://api.weather.gov/points/{latitude},{longitude}";
                var result = await _client.GetFromJsonAsync<JsonObject>(pointsRequestQuery);

                if (result == null || !result.ContainsKey("properties"))
                    throw new Exception("Weather.gov points API returned invalid data.");

                var gridX = result["properties"]!["gridX"]!.ToString();
                var gridY = result["properties"]!["gridY"]!.ToString();
                var gridId = result["properties"]!["gridId"]!.ToString();

                var forecastRequestQuery = $"https://api.weather.gov/gridpoints/{gridId}/{gridX},{gridY}/forecast";
                var forecastResult = await _client.GetFromJsonAsync<JsonObject>(forecastRequestQuery);

                if (forecastResult == null || !forecastResult.ContainsKey("properties"))
                    throw new Exception("Weather.gov forecast API returned invalid data.");

                var periodsJson = forecastResult["properties"]!["periods"]!.ToJsonString();
                return periodsJson;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching forecast: {ex.Message}", ex);
            }
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get([FromQuery] double latitude, [FromQuery] double longitude)
        {
            string json;
            var watch = Stopwatch.StartNew();
            var keyName = $"forecast:{latitude},{longitude}";

            try
            {
                // Try to get cached value
                json = await _redis.StringGetAsync(keyName);

                if (string.IsNullOrEmpty(json))
                {
                    // Fetch from API if not cached
                    json = await GetForecast(latitude, longitude);

                    // Store in Redis
                    var setTask = _redis.StringSetAsync(keyName, json);
                    var expireTask = _redis.KeyExpireAsync(keyName, TimeSpan.FromSeconds(3600));
                    await Task.WhenAll(setTask, expireTask);
                }

                var forecast = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(json);
                watch.Stop();

                return Ok(new ForecastResult(forecast!, watch.ElapsedMilliseconds));
            }
            catch (RedisConnectionException redisEx)
            {
                return StatusCode(500, $"Redis connection error: {redisEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Optional Redis health check
        [HttpGet("redis-test")]
        public IActionResult RedisTest()
        {
            try
            {
                var pong = _redis.Ping();
                return Ok($"Redis is working! Ping: {pong.TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Redis error: {ex.Message}");
            }
        }
    }
}