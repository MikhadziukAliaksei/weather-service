using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WeatherService.Application.Interfaces;
using WeatherService.ConsoleUI;
using WeatherService.Domain.Interfaces;
using WeatherService.Domain.Weather;
using WeatherService.Infrastructure.DataProcessors;
using WeatherService.Infrastructure.DependencyInjection;

var validationResult = CommandLineValidator.Validate(args);
if (!validationResult.isValid)
{
    return;
}

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .Build();

var services = new ServiceCollection().AddSingleton<IConfiguration>(configuration);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.WithProperty("ApplicationContext", typeof(Program).Assembly.GetName().Name)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

services.AddHttpDataClient<WeatherParameter, Weather, WeatherOptions>(configuration);
services.AddScoped<IWeatherTracker, WeatherService.Application.Services.WeatherTracker>();
services.AddScoped<IDataProcessor, DataProcessor>();
services.AddScoped<IDataViewer, DataViewer>();

var provider = services.BuildServiceProvider();

var weatherService = provider.GetRequiredService<IWeatherTracker>();


await weatherService.StartWeatherTrackingAsync(validationResult.cities);