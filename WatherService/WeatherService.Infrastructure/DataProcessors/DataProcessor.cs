using Newtonsoft.Json;
using WeatherService.Domain.Interfaces;
using WeatherService.Domain.Weather;

namespace WeatherService.Infrastructure.DataProcessors;

public class DataProcessor : IDataProcessor
{
    public async Task StoreDataAsync(IEnumerable<Weather> weathers)
    {
        var jsonString = JsonConvert.SerializeObject(weathers);

        // Write the JSON string to a file
        await File.AppendAllTextAsync("weatherData.txt", jsonString);
    }
}