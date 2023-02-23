using CommandLine;

namespace WeatherService.ConsoleUI;

public static class CommandLineValidator
{
    public static (bool isValid, IEnumerable<string> cities) Validate(string[] args)
    {
        var isValid = true;
        IEnumerable<string> cities = null;

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(opts =>
            {
                if (opts.Cities == null || !opts.Cities.Any())
                {
                    Console.Error.WriteLine("Cities cannot be empty.");
                    isValid = false;
                }
                else
                {
                    cities = opts.Cities.Select(_ => _.Trim(','));
                    isValid = true;
                }
            })
            .WithNotParsed(_ =>
            {
                Console.WriteLine("Invalid arguments.");
                isValid = false;
            });


        return (isValid, cities);
    }
}

public class Options
{
    [Option('c', "cities", Required = true, HelpText = "List of cities separated by commas.")]
    public IEnumerable<string> Cities { get; set; }
}