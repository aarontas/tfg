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
    private readonly IDictionary<string, string> _cities;

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

        _cities = new Dictionary<string, string>();
        _cities.Add("Barcelona", "https://www.svgrepo.com/show/338974/barcelona.svg");
        _cities.Add("Madrid", "https://www.svgrepo.com/show/339334/madrid-statue.svg");
        _cities.Add("Maspalomas", "https://www.svgrepo.com/show/490550/beach-umbrella.svg");
        _cities.Add("Melbourne", "https://www.svgrepo.com/show/429083/animal-australia-kangaroo.svg");
        _cities.Add("Helsinki", "https://www.svgrepo.com/show/308251/finland.svg");
        _cities.Add("Nairobi", "https://www.svgrepo.com/show/481472/tiger-illustration-2.svg");
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
            averageScore.AverageTemperature / YearsToForecast,
            _cities[request.CityName]);
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
        var averageScore = new CityWeatherScore(request.CityName, default, default, "");
        var startDate = new DateTime(2023, request.StartDate.Month, request.StartDate.Day);
        var endDate = new DateTime(2023, request.EndDate.Month, request.EndDate.Day);

        for (int yearsBefore = 1; yearsBefore <= YearsToForecast; yearsBefore++)
        {
            var weather = await GetWeather(startDate, endDate, city, yearsBefore, cancellationToken);
            var currentScore = _scoreService.GetScore(weather, request.CityName);

            averageScore = new CityWeatherScore(
                request.CityName,
                averageScore.Score + currentScore.Score,
                averageScore.AverageTemperature + currentScore.AverageTemperature,
                "");
        }

        return averageScore;
    }

    private async Task<WeatherFromApi> GetWeather(
        DateTime startDateRequest,
        DateTime endDateRequest,
        CityParamFromApi city,
        int yearsBefore,
        CancellationToken cancellationToken)
    {
        var startDate = startDateRequest.AddYears(-yearsBefore);
        var endDate = endDateRequest.AddYears(-yearsBefore);
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