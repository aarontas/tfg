using GoodWeather.Cache;
using GoodWeather.Common.Dto;
using GoodWeather.Common.Services;
using GoodWeather.ExternalServices.GeoCoding.Client;
using GoodWeather.ExternalServices.GeoCoding.RequestModels;
using GoodWeather.ExternalServices.Weather.Client;
using GoodWeather.ExternalServices.Weather.RequestModels;
using MediatR;

namespace GoodWeather.Queries;

public record GetWeatherScore(string CityName, DateTime StartDate, DateTime EndDate) : IRequest<CityWeatherScore>;

public class GetWeatherScoreHandler : IRequestHandler<GetWeatherScore, CityWeatherScore>
{
    private readonly IGeocodingClient _geoCodingClient;
    private readonly IWeatherClient _weatherClient;
    private readonly IScoreService _scoreService;
    private readonly ICacheService _cacheService;
    private const int YearsToForecast = 5;

    public GetWeatherScoreHandler(
        IGeocodingClient geoCodingClient,
        IWeatherClient weatherClient,
        IScoreService scoreService,
        ICacheService cacheService)
    {
        _geoCodingClient = geoCodingClient;
        _weatherClient = weatherClient;
        _scoreService = scoreService;
        _cacheService = cacheService;
    }

    public async Task<CityWeatherScore> Handle(GetWeatherScore request, CancellationToken cancellationToken)
    {
        var city = await GetCityParam(request, cancellationToken);
        if (city is null)
            throw new Exception($"City with name {request.CityName} not found");

        var averageScore = await GetAverageScore(request, city, cancellationToken);

        return new CityWeatherScore(
            request.CityName,
            averageScore.Score / YearsToForecast,
            averageScore.AverageTemperature / YearsToForecast);
    }
    private async Task<CityParamFromApi?> GetCityParam(GetWeatherScore request, CancellationToken cancellationToken)
    {
        var geoCity = new GeoCity(request.CityName, 1, "es", "json");

        var cityParam = await _cacheService.GetAsync<CityWeatherParamFromApi>(request.CityName, cancellationToken);
        if(cityParam is not null)
            return cityParam.results.FirstOrDefault();

        cityParam = await _geoCodingClient.GetByCity(geoCity, cancellationToken);
        return cityParam.results.FirstOrDefault();
    }

    private async Task<CityWeatherScore> GetAverageScore(GetWeatherScore request, CityParamFromApi city, CancellationToken cancellationToken)
    {
        var averageScore = new CityWeatherScore(request.CityName, default, default);

        for (int yearsBefore = 1; yearsBefore <= YearsToForecast; yearsBefore++)
        {
            var weather = await GetWeather(request, city, yearsBefore, cancellationToken);
            var currentScore = _scoreService.GetScore(weather, request.CityName);

            averageScore = new CityWeatherScore(
                request.CityName,
                averageScore.Score + currentScore.Score,
                averageScore.AverageTemperature + currentScore.AverageTemperature);
        }

        return averageScore;
    }

    private async Task<WeatherFromApi> GetWeather(
        GetWeatherScore request,
        CityParamFromApi city,
        int yearsBefore,
        CancellationToken cancellationToken)
    {
        var startDate = request.StartDate.AddYears(-yearsBefore);
        var endDate = request.EndDate.AddYears(-yearsBefore);
        var parameters = new CityParameters(
            city.latitude,
            city.longitude,
            startDate,
            endDate,
            "temperature_2m");

        var weather = await _cacheService.GetAsync<WeatherFromApi>(parameters.ToString(), cancellationToken);
        if (weather is not null)
            return weather;
        
        return await _weatherClient.GetByParameters(parameters, cancellationToken);
    }
}