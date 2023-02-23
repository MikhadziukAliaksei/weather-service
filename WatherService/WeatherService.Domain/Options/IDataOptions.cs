namespace WeatherService.Domain.Options;

public interface IDataOptions<TIn>
{
    static abstract string Name { get; }
    string BuildUrl(TIn @in);
}