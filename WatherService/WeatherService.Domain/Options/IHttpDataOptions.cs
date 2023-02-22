namespace WeatherService.Domain.Options;

public interface IHttpDataOptions<TIn> : IDataOptions<TIn>
{
    string BaseUrl { get; }
    string AuthenticationEndpoint { get; }
    string Username { get; }
    string Password { get; }
}