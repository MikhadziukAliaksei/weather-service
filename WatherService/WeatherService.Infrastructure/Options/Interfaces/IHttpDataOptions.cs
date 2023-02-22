using WeatherService.Domain.Options;

namespace WeatherService.Infrastructure.Options.Interfaces;

public interface IHttpDataOptions<TIn> : IDataOptions<TIn>
{
    string BaseUrl { get; }
    string AuthenticationEndpoint { get; }
    string Username { get; }
    string Password { get; }
}