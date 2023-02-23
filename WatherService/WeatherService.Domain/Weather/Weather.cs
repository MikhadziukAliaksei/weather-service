using Newtonsoft.Json;

namespace WeatherService.Domain.Weather;

public class Weather
{
    public required string City { get; init; }
    public required double Temperature { get; init; }
    public required double Precipitation { get; init; }
    public required double WindSpeed { get; init; }
    public required string Summary { get; init; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}