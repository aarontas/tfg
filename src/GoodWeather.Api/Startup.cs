using System.Text.Json;
using GoodWeather.ExternalServices.Weather.DependencyInjection;

namespace GoodWeather.Api;

public static class Startup
{
    public static WebApplicationBuilder AddAppServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddControllers()
            .AddJsonOptions(configure =>
            {
                var options = configure.JsonSerializerOptions;
                options.PropertyNameCaseInsensitive = true;
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        services.AddSwaggerGen();
        services.AddWeatherExternalServices(configuration);
        return builder;
    }
    public static WebApplication UseAppMiddlewares(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        return app;
    }
}