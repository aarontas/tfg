using GoodWeather.Common.Dto;
using GoodWeather.ExternalServices.Weather.RequestModels;

namespace GoodWeather.Common.Services;
public interface IScoreService
{
    public CityWeatherScore GetScore(WeatherFromApi cityWeather, string cityName);
}

public class ScoreService : IScoreService
{
    public CityWeatherScore GetScore(WeatherFromApi cityWeather, string cityName)
    {
        var temperatures = cityWeather.Hourly.Temperature_2m;
        if (temperatures.Count == 0)
            throw new Exception("No temperatures found");

        var average = temperatures.Count > 0 ? temperatures.Average() : 0.0;
        switch (average)
        {
            case > 18 and < 30:
                return new CityWeatherScore(10.00, cityName, average);
            case > 10 and < 18:
            case > 30 and < 40:
                return new CityWeatherScore(5.00, cityName, average);
            default:
                return new CityWeatherScore(0.00, cityName, average);
        }
    }
}

