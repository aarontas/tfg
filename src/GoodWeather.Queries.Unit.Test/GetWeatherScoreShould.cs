using GoodWeather.Common.Dto;
using GoodWeather.Common.Services;
using GoodWeather.ExternalServices.GeoCoding.Client;
using GoodWeather.ExternalServices.GeoCoding.RequestModels;
using GoodWeather.ExternalServices.Weather.Client;
using Moq;
using Xunit;

namespace GoodWeather.Queries.Unit.Test;

public class GetWeatherScoreShould
{

    private GetWeatherScoreHandler _sut;
    private Mock<IWeatherClient> _weatherScoreService;
    private Mock<IGeocodingClient> _geoCodingClient;
    private Mock<IScoreService> _scoreService;

    public GetWeatherScoreShould()
    {
        _weatherScoreService = new Mock<IWeatherClient>(MockBehavior.Strict);
        _geoCodingClient = new Mock<IGeocodingClient>(MockBehavior.Strict);
        _scoreService = new Mock<IScoreService>(MockBehavior.Strict);
        _sut = new GetWeatherScoreHandler(_geoCodingClient.Object, _weatherScoreService.Object, _scoreService.Object);
    }

    [Fact]
    public async Task When_CityNotFound_ShouldThrownException()
    {
        var request = new GetWeatherScore("Barcelona", DateTime.Now, DateTime.Now.AddMonths(1));
        _geoCodingClient.Setup(x => x.GetByCity(It.IsAny<GeoCity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CityWeatherParamFromApi)null!);

        await Assert.ThrowsAsync<Exception>(() => _sut.Handle(request, CancellationToken.None));
    }
}