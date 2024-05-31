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
    private readonly IDictionary<string, string> _cities;

    public GetCitiesHandler(ICacheService cacheService, IGeocodingClient geoCodingClient, IWeatherClient weatherClient)
    {
        _cacheService = cacheService;
        _geoCodingClient = geoCodingClient;
        _weatherClient = weatherClient;
        _cities = new Dictionary<string, string>();
        _cities.Add("Barcelona", "https://www.svgrepo.com/show/338974/barcelona.svg");
        _cities.Add("Madrid", "https://www.svgrepo.com/show/339334/madrid-statue.svg");
        _cities.Add("Maspalomas", "https://www.svgrepo.com/show/490550/beach-umbrella.svg");
        _cities.Add("Melbourne", "https://www.svgrepo.com/show/429083/animal-australia-kangaroo.svg");
        _cities.Add("Helsinki", "https://www.svgrepo.com/show/308251/finland.svg");
        _cities.Add("Nairobi", "https://www.svgrepo.com/show/481472/tiger-illustration-2.svg");

        // _cities.Add("Barcelona", "");
        // _cities.Add("Madrid", "");
        // _cities.Add("Maspalomas", "");
        // _cities.Add("Melbourne", "");
        // _cities.Add("Helsinki", "");
        // _cities.Add("Nairobi", "");
    }

    public async Task<IEnumerable<CityWeather>> Handle(GetCities request, CancellationToken cancellationToken)
    {
        var cities = new List<CityWeather>();
        foreach (var cityParam in _cities)
        {
            var cityParamFromApi = await GetCityParam(cityParam.Key, cancellationToken);
            if (cityParamFromApi is null)
                throw new Exception($"City with name {cityParam} not found");

            var temperature = await GetTemperature(cityParamFromApi, cancellationToken);
            cities.Add(new CityWeather(cityParam.Key, temperature, cityParam.Value));
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