using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GoodWeather.ExternalServices.Weather.RequestModels;

namespace GoodWeather.ExternalServices.Weather.Client;

public class WeatherClient : IWeatherClient
{
    private readonly HttpClient _httpClient;

    public WeatherClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherFromApi> GetByParameters(CityParameters parameters, CancellationToken cancel)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"archive?" +
            $"latitude={parameters.Latitude}&" +
            $"longitude={parameters.Longitude}&" +
            $"start_date={parameters.StartDate}&" +
            $"end_date={parameters.EndDate}&" +
            $"hourly={parameters.Hourly}");

        var response = await _httpClient.SendAsync(request, cancel);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancel);
        if(string.IsNullOrEmpty(responseContent))
            throw new Exception("No content in response");

        return await response.Content.ReadFromJsonAsync<WeatherFromApi>(cancellationToken: cancel);
    }
}