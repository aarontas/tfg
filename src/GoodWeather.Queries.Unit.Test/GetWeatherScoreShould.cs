using GoodWeather.Cache;
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
    private Mock<ICacheService> _cacheService;

    public GetWeatherScoreShould()
    {
        _weatherScoreService = new Mock<IWeatherClient>(MockBehavior.Strict);
        _geoCodingClient = new Mock<IGeocodingClient>(MockBehavior.Strict);
        _scoreService = new Mock<IScoreService>(MockBehavior.Strict);
        _cacheService = new Mock<ICacheService>(MockBehavior.Strict);
        _cacheService.Setup(x => x.GetAsync<CityWeatherParamFromApi>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CityWeatherParamFromApi?)null);
        _sut = new GetWeatherScoreHandler(_geoCodingClient.Object, _weatherScoreService.Object, _scoreService.Object, _cacheService.Object);
    }

    [Fact]
    public async Task When_CityNotFound_ShouldThrownException()
    {
        var request = new GetWeatherScore("Barcelona", DateTime.Now, DateTime.Now.AddMonths(1));
        _geoCodingClient.Setup(x => x.GetByCity(It.IsAny<GeoCity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync( new CityWeatherParamFromApi(){results = new List<CityParamFromApi>()});

        await Assert.ThrowsAsync<Exception>(() => _sut.Handle(request, CancellationToken.None));
    }
}