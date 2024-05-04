using System.Text.Json;
using GoodWeather.Cache;
using GoodWeather.Common.DependencyInjection;
using GoodWeather.CQRS.Queries.DependencyInjection;
using GoodWeather.ExternalServices.GeoCoding.DependencyInjection;
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
        services.AddHttpClient();
        services.AddSwaggerGen();
        services.AddApiVersioning();
        services.AddQueries();
        services.AddApplicationCommon();
        services.AddWeatherExternalServices(configuration);
        services.AddGeoCodingExternalServices(configuration);
        services.AddScoped<ICacheService, CacheService>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("redis");
        });
        return builder;
    }
    public static WebApplication UseAppMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        return app;
    }
}