namespace ExternalServices.Weather.DependencyInjection;

public class WeatherOptions
{
    public const string SECTION_NAME = "WeatherApi";
    public string BaseAddress { get; set; }
}