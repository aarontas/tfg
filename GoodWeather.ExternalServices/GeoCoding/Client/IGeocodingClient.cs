using GoodWeather.ExternalServices.GeoCoding.RequestModels;

namespace GoodWeather.ExternalServices.GeoCoding.Client;

public interface IGeocodingClient
{
    Task<CityWeatherParamFromApi> GetByCity(GeoCity geoCity, CancellationToken cancel);
}