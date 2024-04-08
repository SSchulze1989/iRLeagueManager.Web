using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;

public sealed class EventCalculationServiceTests
{
    private readonly Fixture fixture;
    private readonly Mock<ICalculationService<SessionCalculationData, SessionCalculationResult>> mockService;
    private readonly Mock<ICalculationServiceProvider<SessionCalculationConfiguration, SessionCalculationData, SessionCalculationResult>>
        mockServiceProvider;

    public EventCalculationServiceTests()
    {
        fixture = new();
        mockService = new Mock<ICalculationService<SessionCalculationData, SessionCalculationResult>>();
        mockService.Setup(x => x.Calculate(It.IsAny<SessionCalculationData>()))
            .ReturnsAsync(() => fixture.Create<SessionCalculationResult>())
            .Verifiable();
        mockServiceProvider = new Mock<ICalculationServiceProvider<SessionCalculationConfiguration, SessionCalculationData, SessionCalculationResult>>();
        mockServiceProvider
            .Setup(x => x.GetCalculationService(It.IsAny<SessionCalculationConfiguration>()))
            .Returns(() => mockService.Object)
            .Verifiable();
        fixture.Register(() => mockServiceProvider.Object);
    }

    [Fact]
    public async Task Calculate_ShouldThrow_WhenEventIdDoesNotMatch()
    {
        var data = GetCalculationData();
        var config = GetCalculationConfiguration(data.LeagueId, fixture.Create<long>());
        var sut = CreateSut(config);

        var test = async () => await sut.Calculate(data);

        await test.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Calculate_ShouldSetResultData()
    {
        var data = GetCalculationData();
        var config = GetCalculationConfiguration(data);
        var sut = CreateSut(config);

        var test = await sut.Calculate(data);

        test.LeagueId.Should().Be(config.LeagueId);
        test.EventId.Should().Be(config.EventId);
        test.ResultId.Should().Be(config.ResultId);
        test.ResultConfigId.Should().Be(config.ResultConfigId);
        test.Name.Should().Be(config.DisplayName);
        test.SessionResults.Should().HaveSameCount(config.SessionResultConfigurations);
    }

    [Fact]
    public async Task Calculate_ShouldCallCalculateForEachSession()
    {
        var data = GetCalculationData();
        var config = GetCalculationConfiguration(data);
        var sut = CreateSut(config);

        var test = await sut.Calculate(data);

        foreach (var session in data.SessionResults)
        {
            mockService.Verify(x => x.Calculate(session), Times.Once());
        }
    }

    [Fact]
    public async Task Calculate_ShouldNotCallCalculate_WhenNoMatchingConfigFound()
    {
        var data = GetCalculationData();
        var config = GetCalculationConfiguration(data);
        var addData = fixture.Create<SessionCalculationData>();
        data.SessionResults = data.SessionResults.Append(addData);
        var sut = CreateSut(config);

        var test = await sut.Calculate(data);

        test.SessionResults.Should().HaveCount(data.SessionResults.Count() - 1);
        mockService.Verify(x => x.Calculate(addData), Times.Never());
    }

    [Fact]
    public async Task Calculate_ShouldIngoreSessionConfig_WhenNoMatchingSessionFound()
    {
        var data = GetCalculationData();
        var config = GetCalculationConfiguration(data);
        var addConfig = fixture.Create<SessionCalculationConfiguration>();
        config.SessionResultConfigurations = config.SessionResultConfigurations.Append(addConfig);
        var sut = CreateSut(config);

        var test = await sut.Calculate(data);

        test.SessionResults.Should().HaveCount(config.SessionResultConfigurations.Count() - 1);
    }

    private EventCalculationService CreateSut(EventCalculationConfiguration config)
    {
        fixture.Register(() => config);
        return fixture.Create<EventCalculationService>();
    }

    private EventCalculationData GetCalculationData()
    {
        return fixture.Create<EventCalculationData>();
    }

    private EventCalculationConfiguration GetCalculationConfiguration(EventCalculationData data)
    {
        return GetCalculationConfiguration(data.LeagueId, data.EventId, data.SessionResults);
    }

    private EventCalculationConfiguration GetCalculationConfiguration(long leagueId, long eventId, IEnumerable<SessionCalculationData>? sessionData = default)
    {
        var sessionIds = sessionData?.Select(x => x.SessionId).NotNull() ?? Array.Empty<long>();
        var sessionNrs = sessionData?.Select(x => x.SessionNr).NotNull() ?? Array.Empty<int>();
        var config = fixture.Build<EventCalculationConfiguration>()
            .With(x => x.LeagueId, leagueId)
            .With(x => x.EventId, eventId)
            .Create();
        foreach ((var sessionConfig, var index) in config.SessionResultConfigurations.Select((x, i) => (x, i)))
        {
            sessionConfig.SessionId = sessionIds.ElementAtOrDefault(index);
            sessionConfig.SessionNr = sessionNrs.ElementAtOrDefault(index);
            sessionConfig.IsCombinedResult = false;
        }
        return config;
    }
}
