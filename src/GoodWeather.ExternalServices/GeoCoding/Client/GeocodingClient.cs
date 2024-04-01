using System.Net.Http.Json;
using GoodWeather.ExternalServices.GeoCoding.RequestModels;

namespace GoodWeather.ExternalServices.GeoCoding.Client;

public class GeocodingClient : IGeocodingClient
{
    private readonly HttpClient _httpClient;

    public GeocodingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CityWeatherParamFromApi> GetByCity(GeoCity geoCity, CancellationToken cancel)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"search?" +
            $"name={geoCity.CityName}&" +
            $"count={geoCity.Count}&" +
            $"language={geoCity.Language}&" +
            $"format={geoCity.Format}");

        var response = await _httpClient.SendAsync(request, cancel);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancel);
        if(string.IsNullOrEmpty(responseContent))
            throw new Exception("No content in response");

        return await response.Content.ReadFromJsonAsync<CityWeatherParamFromApi>(cancellationToken: cancel);
    }
}