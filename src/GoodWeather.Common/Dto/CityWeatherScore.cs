namespace GoodWeather.Common.Dto;

public record CityWeatherScore(string CityName, double Score, string AverageTemperature, string ImageUrl);
public record CityWeatherScoreDto(string CityName, double Score, double AverageTemperature, string ImageUrl);