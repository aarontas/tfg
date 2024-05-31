using GoodWeather.Common.Dto;
using GoodWeather.ExternalServices.Weather.RequestModels;

namespace GoodWeather.Common.Services;
public interface IScoreService
{
    public CityWeatherScoreDto GetScore(WeatherFromApi cityWeather, string cityName);
}

public class ScoreService : IScoreService
{
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

