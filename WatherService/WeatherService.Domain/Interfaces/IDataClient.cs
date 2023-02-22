namespace WeatherService.Domain.Interfaces;

public interface IDataClient<in TIn, TOut>
{
    Task<TOut?> GetDataAsync(TIn @in, CancellationToken cancellationToken = default);
    
}