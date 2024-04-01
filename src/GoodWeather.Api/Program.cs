using GoodWeather.Api;

var builder = WebApplication.CreateBuilder(args) ?? throw new ArgumentNullException("WebApplication.CreateBuilder(args)");
var app = builder.AddAppServices().Build();

await app.UseAppMiddlewares()
    .RunAsync();