using GoodWeather.ExternalServices.Weather.RequestModels;

namespace GoodWeather.ExternalServices.Weather.Client;

public interface IWeatherService
{
    Task<WeatherRequest> GetByCity(CityParameters parameters, CancellationToken cancel);
}