using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GoodWeather.CQRS.Queries.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        return services;
    }
}