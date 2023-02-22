namespace WeatherService.Domain.Interfaces;

public interface IDataProcessor
{
    Task StoreDataAsync(IEnumerable<Weather.Weather> weathers);
}