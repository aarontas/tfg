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

    public GetWeatherScoreHandler(
        IGeocodingClient geoCodingClient,
        IWeatherClient weatherClient,
        IScoreService scoreService)
    {
        _geoCodingClient = geoCodingClient;
        _weatherClient = weatherClient;
        _scoreService = scoreService;
    }

    public async Task<CityWeatherScore> Handle(GetWeatherScore request, CancellationToken cancellationToken)
    {
        var city = await GetCityParam(request, cancellationToken);
        if (city is null)
            throw new Exception($"City with name {request.CityName} not found");

        var weather = await GetWeather(request, cancellationToken, city);
        return _scoreService.GetScore(weather, request.CityName);
    }

    private async Task<WeatherFromApi> GetWeather(GetWeatherScore request, CancellationToken cancellationToken, CityParamFromApi city)
    {
        var parameters = new CityParameters(
            city.latitude,
            city.longitude,
            request.StartDate,
            request.EndDate,
            "temperature_2m");
        var weather = await _weatherClient.GetByParameters(parameters, cancellationToken);
        return weather;
    }

    private async Task<CityParamFromApi?> GetCityParam(GetWeatherScore request, CancellationToken cancellationToken)
    {
        var geoCity = new GeoCity(request.CityName, 1, "es", "json");
        var cityParam = await _geoCodingClient.GetByCity(geoCity, cancellationToken);
        return cityParam.results.FirstOrDefault();
    }
}