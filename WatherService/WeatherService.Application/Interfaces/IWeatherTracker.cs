namespace WeatherService.Application.Interfaces;

public interface IWeatherTracker
{
    Task StartWeatherTrackingAsync(IEnumerable<string> cities, CancellationToken cancellationToken = default);
}