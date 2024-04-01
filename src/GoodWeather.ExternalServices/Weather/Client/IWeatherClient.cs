using GoodWeather.ExternalServices.Weather.RequestModels;

namespace GoodWeather.ExternalServices.Weather.Client;

public interface IWeatherClient
{
    Task<WeatherFromApi> GetByParameters(CityParameters parameters, CancellationToken cancel);
}