using ExternalServices.Weather.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExternalServices.Weather.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWeatherExternalServices(this IServiceCollection services, IConfiguration config)
    {
        var weatherConfig = config
            .GetSection(WeatherOptions.SECTION_NAME)
            .Get<WeatherOptions>();

        if (weatherConfig is null || string.IsNullOrEmpty(weatherConfig.BaseAddress))
            throw new InvalidOperationException("BaseAddress is not set for Weather options");

        services.AddHttpClient<IWeatherService, WeatherService>(c =>
        {
            c.BaseAddress = new Uri(weatherConfig.BaseAddress);
        });
        return services;
    }
}