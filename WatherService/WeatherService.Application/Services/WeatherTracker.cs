using Serilog;
using WeatherService.Application.Interfaces;
using WeatherService.Domain.Interfaces;
using WeatherService.Domain.Weather;

namespace WeatherService.Application.Services;

public class WeatherTracker : IWeatherTracker
{
    private readonly IDataClient<WeatherParameter, Weather> _dataClient;
    private readonly IDataViewer _dataViewer;
    private readonly IDataProcessor _dataProcessor;

    public WeatherTracker(IDataClient<WeatherParameter, Weather> dataClient, IDataViewer dataViewer,
        IDataProcessor dataProcessor)
    {
        _dataClient = dataClient;
        _dataViewer = dataViewer;
        _dataProcessor = dataProcessor;
    }

    public async Task StartWeatherTrackingAsync(IEnumerable<string> cities,
        CancellationToken cancellationToken = default)
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(15));
        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            var weathers = new List<Weather>();

            foreach (var city in cities)
            {
                var weather = await _dataClient.GetDataAsync(new WeatherParameter
                {
                    CityName = city
                }, cancellationToken);

                weathers.Add(weather);
            }

            await _dataProcessor.StoreDataAsync(weathers);
            await _dataViewer.DisplayDataAsync(weathers);

            Log.Information("Finished fetching weather data");
        }
    }
}