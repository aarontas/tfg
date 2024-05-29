using GoodWeather.Cache;
using GoodWeather.Common.Dto;
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
    private readonly IReadOnlyList<string> _cities;

    public GetCitiesHandler(ICacheService cacheService, IGeocodingClient geoCodingClient, IWeatherClient weatherClient)
    {
        _cacheService = cacheService;
        _geoCodingClient = geoCodingClient;
        _weatherClient = weatherClient;
        _cities = new List<string>
        {
            "Barcelona",
            "Madrid",
            "Maspalomas",
            "Melbourne",
            "Moscu"
        };
    }

    public async Task<IEnumerable<CityWeather>> Handle(GetCities request, CancellationToken cancellationToken)
    {
        var cities = new List<CityWeather>();
        foreach (var cityName in _cities)
        {
            var cityParamFromApi = await GetCityParam(cityName, cancellationToken);
            if (cityParamFromApi is null)
                throw new Exception($"City with name {cityName} not found");

            var temperature = await GetTemperature(cityParamFromApi, cancellationToken);
            cities.Add(new CityWeather(cityName, temperature));
        }
        return cities;
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
            DateTime.Now.AddHours(-1),
            DateTime.Now,
            "temperature_2m");

        var weather = await _cacheService.GetAsync<WeatherFromApi>(parameters.ToString(), cancellationToken);
        if (weather is not null)
            return weather.Hourly.Temperature_2m.FirstOrDefault();

        weather = await _weatherClient.GetByParameters(parameters, cancellationToken);
        return weather.Hourly.Temperature_2m.FirstOrDefault();
    }
}