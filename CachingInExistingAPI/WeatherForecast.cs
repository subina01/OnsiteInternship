namespace CachingInExistingAPI
{
    public class WeatherForecast
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsDayTime { get; set; }
        public int Temperature { get; set; }
        public string? TemperatureUnit { get; set; }
        public string? TemperatureTrend { get; set; }
        public string? WindSpeed { get; set; }
        public string? WindDirection { get; set; }
        public string? ShortForecast { get; set; }
        public string? DetailedForecast { get; set; }
    }
}
