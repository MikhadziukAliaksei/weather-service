using WeatherService.Domain.Interfaces;
using WeatherService.Domain.Weather;

namespace WeatherService.Infrastructure.DataProcessors;

public class DataViewer : IDataViewer
{
    public Task DisplayDataAsync(IEnumerable<Weather> weathers)
    {
        foreach (var weather in weathers)
        {
            Console.WriteLine(weather);
        }

        return Task.CompletedTask;
    }
}