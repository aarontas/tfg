using GoodWeather.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoodWeather.Common.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCommon(this IServiceCollection services) =>
        services.AddScoped<IScoreService, ScoreService>();
}