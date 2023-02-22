using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherService.Domain.Interfaces;
using WeatherService.Domain.Options;
using WeatherService.Infrastructure.DataClients;

namespace WeatherService.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpDataClient<TIn, TOut, TOption>(this IServiceCollection services,
        IConfiguration configuration)
        where TOption : class, IHttpDataOptions<TIn>
    {
        services.AddOptions<TOption>(TOption.Name)
            .BindConfiguration(TOption.Name);
        services.AddHttpClient<HttpDataClient<TIn, TOut, TOption>>();
        services.AddHttpClient<IDataClient<TIn, TOut>, HttpDataClient<TIn, TOut, TOption>>();

        return services;
    }
}