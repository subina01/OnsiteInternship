using Grpc.Core;
using WeatherGrpc;

namespace GrpcWeatherServer
{
    public class WeatherService : Weather.WeatherBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        public override Task<WeatherReply> GetWeather(WeatherRequest request, ServerCallContext context)
        {
            var rng = new Random();
            return Task.FromResult(new WeatherReply
            {
                City = request.City,
                Summary = Summaries[rng.Next(Summaries.Length)],
                TemperatureC = rng.Next(-20, 55)
            });
        }

        public override async Task GetWeatherStream(WeatherRequest request, IServerStreamWriter<WeatherReply> responseStream, ServerCallContext context)
        {
            var rng = new Random();
            for (int i = 0; i < 5; i++)
            {
                await responseStream.WriteAsync(new WeatherReply
                {
                    City = request.City,
                    Summary = Summaries[rng.Next(Summaries.Length)],
                    TemperatureC = rng.Next(-20, 55)
                });
                await Task.Delay(1000);
            }
        }
    }
}
