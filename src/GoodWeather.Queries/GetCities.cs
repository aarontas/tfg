using GoodWeather.Cache;
using GoodWeather.Common.Dto;
using GoodWeather.Common.Services;
using GoodWeather.ExternalServices.GeoCoding.Client;
using GoodWeather.ExternalServices.GeoCoding.RequestModels;
using GoodWeather.ExternalServices.Weather.Client;
using GoodWeather.ExternalServices.Weather.RequestModels;
using MediatR;

namespace GoodWeather.Queries;

public record GetCities() : IRequest<IEnumerable<CityWeather>>;

public class GetCitiesHandler : IRequestHandler<GetCities, IEnumerable<CityWeather>>
{
    private readonly ICacheService _cacheService;
    private readonly IGeocodingClient _geoCodingClient;
    private readonly IWeatherClient _weatherClient;
    private readonly IScoreService _scoreService;

    public GetCitiesHandler(ICacheService cacheService, IGeocodingClient geoCodingClient, IWeatherClient weatherClient, IScoreService scoreService)
    {
        _cacheService = cacheService;
        _geoCodingClient = geoCodingClient;
        _weatherClient = weatherClient;
        _scoreService = scoreService;
    }

    public async Task<IEnumerable<CityWeather>> Handle(GetCities request, CancellationToken cancellationToken)
    {
        var cities = _scoreService.Cities;
        var cityWeathers = new List<CityWeather>();
        foreach (var cityParam in cities)
        {
            var cityParamFromApi = await GetCityParam(cityParam.Key, cancellationToken);
            if (cityParamFromApi is null)
                throw new Exception($"City with name {cityParam} not found");

            var temperature = await GetTemperature(cityParamFromApi, cancellationToken);
            cityWeathers.Add(new CityWeather(cityParam.Key, temperature, cityParam.Value));
        }
        return cityWeathers;
    }

    private async Task<CityParamFromApi?> GetCityParam(string cityName, CancellationToken cancellationToken)
    {
        var geoCity = new GeoCity(cityName, 1, "es", "json");

        var cityParam = await _cacheService.GetAsync<CityWeatherParamFromApi>(cityName, cancellationToken);
        if(cityParam is not null)
            return cityParam.results.FirstOrDefault();

        cityParam = await _geoCodingClient.GetByCity(geoCity, cancellationToken);
        return cityParam.results.FirstOrDefault();
    }

    private async Task<double> GetTemperature(
        CityParamFromApi city,
        CancellationToken cancellationToken)
    {
        var parameters = new CityParameters(
            city.latitude,
            city.longitude,
            DateTime.Today.AddDays(-5),
            DateTime.Today.AddDays(-5),
            "temperature_2m");

        var weather = await _cacheService.GetAsync<WeatherFromApi>(parameters.ToString(), cancellationToken);
        if (weather is not null)
            return weather.Hourly.Temperature_2m.FirstOrDefault();

        weather = await _weatherClient.GetByParameters(parameters, cancellationToken);
        return weather.Hourly.Temperature_2m.FirstOrDefault();
    }
}