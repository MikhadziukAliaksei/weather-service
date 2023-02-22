using WeatherService.Domain.Options;

namespace WeatherService.Domain.Weather;

public class WeatherOptions : IHttpDataOptions<WeatherParameter>
{
    public static string Name { get; } = nameof(WeatherOptions);

    public required string BaseUrl { get; init; }
    public required string GetWeatherByCityNameEndpoint { get; init; }
    public required string AuthenticationEndpoint { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }

    public string BuildUrl(WeatherParameter @in)
    {
        return $"{BaseUrl}/{GetWeatherByCityNameEndpoint}/{@in.CityName}";
    }
}