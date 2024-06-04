using GoodWeather.Common.Dto;
using GoodWeather.ExternalServices.Weather.RequestModels;

namespace GoodWeather.Common.Services;
public interface IScoreService
{
    public CityWeatherScoreDto GetScore(WeatherFromApi cityWeather, string cityName);
    public IDictionary<string, string> Cities { get; }
}

public class ScoreService : IScoreService
{
    public IDictionary<string, string> Cities { get; }
    public ScoreService()
    {
        Cities = new Dictionary<string, string>();
        // Cities.Add("Barcelona", "https://www.svgrepo.com/show/338974/barcelona.svg");
        // Cities.Add("Madrid", "https://www.svgrepo.com/show/339334/madrid-statue.svg");
        // Cities.Add("Maspalomas", "https://www.svgrepo.com/show/490550/beach-umbrella.svg");
        // Cities.Add("Melbourne", "https://www.svgrepo.com/show/429083/animal-australia-kangaroo.svg");
        // Cities.Add("Helsinki", "https://www.svgrepo.com/show/308251/finland.svg");
        // Cities.Add("Nairobi", "https://www.svgrepo.com/show/481472/tiger-illustration-2.svg");

        Cities.Add("Barcelona", "");
        Cities.Add("Madrid", "");
        Cities.Add("Maspalomas", "");
        Cities.Add("Melbourne", "");
        Cities.Add("Helsinki", "");
        Cities.Add("Nairobi", "");
    }

    public CityWeatherScoreDto GetScore(WeatherFromApi cityWeather, string cityName)
    {
        var temperatures = cityWeather.Hourly.Temperature_2m;
        if (temperatures.Count == 0)
            throw new Exception("No temperatures found");

        var average = temperatures.Count > 0 ? temperatures.Average() : 0.0;
        switch (average)
        {
            case > 18 and < 30:
                return new CityWeatherScoreDto( cityName, 10.00,average, "");
            case > 10 and < 18:
            case > 30 and < 40:
                return new CityWeatherScoreDto( cityName, 5.00, average, "");
            default:
                return new CityWeatherScoreDto( cityName, 0.00, average, "");
        }
    }

}

