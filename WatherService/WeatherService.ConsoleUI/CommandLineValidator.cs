using NDesk.Options;

namespace WeatherService.ConsoleUI;

public static class CommandLineValidator
{
    public static (bool isValid, IEnumerable<string> cities) Validate(string[] args)
    {
        var isValid = true;
        string cities = null;

        var validOptions = new OptionSet
        {
            {"cities=", "comma-separated list of cities to fetch weather data", c => cities = c}
        };

        try
        {
            validOptions.Parse(args);
        }
        catch (OptionException ex)
        {
            Console.Error.WriteLine($"WeatherService.ConsoleUI.exe: {ex.Message}");
            isValid = false;
        }

        if (string.IsNullOrEmpty(cities))
        {
            Console.Error.WriteLine("WeatherService.ConsoleUI.exe: --cities option is required");
            isValid = false;
        }

        var citiesList = cities?.Split(',');
        
        if (citiesList?.Length == 0)
        {
            Console.Error.WriteLine("WeatherService.ConsoleUI.exe: --cities option must contain at least one city");
            isValid = false;
        }

        return ( isValid: isValid, cities: citiesList);
    }
}