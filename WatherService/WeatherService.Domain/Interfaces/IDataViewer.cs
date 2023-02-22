namespace WeatherService.Domain.Interfaces;

public interface IDataViewer
{
    Task DisplayDataAsync(IEnumerable<Weather.Weather> weathers);
}