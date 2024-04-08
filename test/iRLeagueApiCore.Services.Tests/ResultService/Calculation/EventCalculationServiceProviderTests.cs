using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;

public sealed class EventCalculationServiceProviderTests
{
    private readonly Fixture fixture;
    private readonly Mock<ICalculationServiceProvider<SessionCalculationConfiguration, SessionCalculationData, SessionCalculationResult>>
        mockServiceProvider;

    public EventCalculationServiceProviderTests()
    {
        fixture = new();
        mockServiceProvider = new Mock<ICalculationServiceProvider<SessionCalculationConfiguration, SessionCalculationData, SessionCalculationResult>>();
        mockServiceProvider.Setup(x => x.GetCalculationService(It.IsAny<SessionCalculationConfiguration>()))
            .Returns(() => Mock.Of<ICalculationService<SessionCalculationData, SessionCalculationResult>>());
        fixture.Register(() => mockServiceProvider.Object);
    }

    [Fact]
    public void GetCalculationService_ShouldProvideEventCalculationService()
    {
        var config = GetCalculationConfiguration();
        var sut = CreateSut();

        var test = sut.GetCalculationService(config);

        test.Should().BeOfType<EventCalculationService>();
    }

    private EventCalculationServiceProvider CreateSut()
    {
        return fixture.Create<EventCalculationServiceProvider>();
    }

    private EventCalculationConfiguration GetCalculationConfiguration()
    {
        return fixture.Create<EventCalculationConfiguration>();
    }
}
