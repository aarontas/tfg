using ExternalServices.Weather.RequestModels;

namespace ExternalServices.Weather.Client;

public interface IWeatherService
{
    Task<WeatherRequest> GetByCity(CityParameters parameters, CancellationToken cancel);
}