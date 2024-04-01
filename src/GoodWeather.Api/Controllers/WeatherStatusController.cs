using Asp.Versioning;
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
    public async Task<IActionResult> GetWeatherStatus(string cityName, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var request = new GetWeatherScore(cityName, startDate, endDate);
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}
