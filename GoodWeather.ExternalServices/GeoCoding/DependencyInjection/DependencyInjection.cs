using GoodWeather.ExternalServices.GeoCoding.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoodWeather.ExternalServices.GeoCoding.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddGeoCodingExternalServices(this IServiceCollection services, IConfiguration config)
    {
        var geocodingConfig = config
            .GetSection(GeoCodingOptions.SECTION_NAME)
            .Get<GeoCodingOptions>();

        if (geocodingConfig is null || string.IsNullOrEmpty(geocodingConfig.BaseAddress))
            throw new InvalidOperationException("BaseAddress is not set for GeoCoding options");

        services.AddHttpClient<IGeocodingClient, GeocodingClient>(c =>
        {
            c.BaseAddress = new Uri(geocodingConfig.BaseAddress);
        });
        return services;
    }
}