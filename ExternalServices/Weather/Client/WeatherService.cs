using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ExternalServices.Weather.RequestModels;

namespace ExternalServices.Weather.Client;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherRequest> GetByCity(CityParameters parameters, CancellationToken cancel)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("archive/"),
            Content = CreateContent(parameters)
        };
        var response = await _httpClient.SendAsync(request, cancel);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WeatherRequest>(cancellationToken: cancel);
    }

    private static StringContent CreateContent(CityParameters parameters)
    {
        var content = JsonSerializer.Serialize(parameters);
        return new StringContent(content, Encoding.Unicode, "application/json");
    }
}