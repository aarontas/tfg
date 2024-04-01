using Asp.Versioning;
using GoodWeather.CQRS.Queries;
using GoodWeather.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GoodWeather.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class WeatherStatusController : ControllerBase
{

    private readonly IMediator _mediator;
    public WeatherStatusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetWeatherStatus(string cityName, DateTime startDate, DateTime endDate)
    {
        // var parameters = new CityParameters(
        //     52.52,
        //     13.41,
        //     new DateTime(2024,03,15),
        //     new DateTime(2024,03,29),
        //     "temperature_2m");
        // var weatherStatus = await _weatherClient.GetByParameters(parameters, CancellationToken.None);
        //
        // var geoCity = new GeoCity("Barcelona", 1, "es", "json");
        // var geocoding = await _geocodingClient.GetByCity(geoCity, CancellationToken.None);

        var result = await _mediator.Send(new GetWeatherScore(cityName, startDate, endDate));
        return Ok(result);
    }
}
