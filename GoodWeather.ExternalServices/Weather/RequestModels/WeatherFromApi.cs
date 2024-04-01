namespace GoodWeather.ExternalServices.Weather.RequestModels;

public class WeatherFromApi
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double GenerationTimeMs { get; set; }
    public string Timezone { get; set; }
    public string TimezoneAbbreviation { get; set; }
    public Hourly Hourly { get; set; }
    public HourlyUnits HourlyUnits { get; set; }
}

public class Hourly
{
    public List<string> Time { get; set; }
    public List<double> Temperature_2m { get; set; }
}

public class HourlyUnits
{
    public string Temperature_2m { get; set; }
}
