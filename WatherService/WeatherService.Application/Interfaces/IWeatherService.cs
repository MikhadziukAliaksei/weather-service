using WeatherService.Data.Weather;

namespace WeatherService.Application.Interfaces;

public interface IWeatherService
{
    IAsyncEnumerable<Weather> GetWeatherAsync(IEnumerable<string> cities, CancellationToken cancellationToken = default);
}