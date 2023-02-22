using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WeatherService.Application.Interfaces;
using WeatherService.ConsoleUI;
using WeatherService.Data.Weather;
using WeatherService.Infrastructure.DependencyInjection;

var isValid = CommandLineValidator.Validate(args);
if (!isValid)
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
    .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

services.AddHttpDataClient<WeatherParameter, Weather, WeatherOptions>(configuration);
services.AddScoped<IWeatherService, WeatherService.Application.Services.WeatherService>();

var provider = services.BuildServiceProvider();

var weatherService = provider.GetRequiredService<IWeatherService>();


while (true)
{
    var result = weatherService.GetWeatherAsync(new[] {"Vilnius", "Kaunas", "Klaipėda"});

    await foreach (var weather in result)
    {
        Console.WriteLine(weather);
    }

    await Task.Delay(15000);
}