﻿using NDesk.Options;

namespace WeatherService.ConsoleUI;

public static class CommandLineValidator
{
    public static bool Validate(string[] args)
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
            Console.Error.WriteLine($"isun.exe: {ex.Message}");
            isValid = false;
        }

        if (string.IsNullOrEmpty(cities))
        {
            Console.Error.WriteLine("isun.exe: --cities option is required");
            isValid = false;
        }

        var citiesList = cities.Split(',');
        
        if (citiesList.Length == 0)
        {
            Console.Error.WriteLine("isun.exe: --cities option must contain at least one city");
            isValid = false;
        }

        return isValid;
    }
}