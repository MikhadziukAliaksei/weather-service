using System.Runtime.CompilerServices;
using Serilog;
using WeatherService.Application.Interfaces;
using WeatherService.Data.Weather;
using WeatherService.Domain.Interfaces;

namespace WeatherService.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly IDataClient<WeatherParameter, Weather> _dataClient;

    public WeatherService(IDataClient<WeatherParameter, Weather> dataClient)
    {
        _dataClient = dataClient;
    }

    public async IAsyncEnumerable<Weather> GetWeatherAsync(IEnumerable<string> cities,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var city in cities)
        {
            
            yield return await _dataClient.GetDataAsync(new WeatherParameter
            {
                CityName = city
            }, cancellationToken);
        }
        
        Log.Information("Finished fetching weather data");
    }
}