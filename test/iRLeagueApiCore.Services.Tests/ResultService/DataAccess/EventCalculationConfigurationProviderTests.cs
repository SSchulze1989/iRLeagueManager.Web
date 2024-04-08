using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Services.ResultService.DataAccess;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.Tests.ResultService.DataAccess;

public sealed class EventCalculationConfigurationProviderTests : DataAccessTestsBase
{
    public EventCalculationConfigurationProviderTests()
    {
        var sessionConfigurationProvider = fixture.Create<SessionCalculationConfigurationProvider>();
        fixture.Register<ISessionCalculationConfigurationProvider>(() => sessionConfigurationProvider);
    }

    [Fact]
    public async Task GetResultConfigIds_ShouldReturnEmpty_WhenNoResulConfigConfigured()
    {
        var @event = await dbContext.Events
            .FirstAsync();
        var sut = CreateSut();

        var test = await sut.GetResultConfigIds(@event.EventId);

        test.Should().BeEmpty();
    }

    [Fact]
    public async Task GetResultConfigIds_ShouldReturnCollection_WhenResultConfigsConfigured()
    {
        var @event = await dbContext.Events
            .FirstAsync();
        var configs = accessMockHelper.ConfigurationBuilder(@event).CreateMany();
        dbContext.ResultConfigurations.AddRange(configs);
        await dbContext.SaveChangesAsync();
        var sut = CreateSut();

        var test = await sut.GetResultConfigIds(@event.EventId);

        test.Should().HaveSameCount(configs);
        test.Should().BeEquivalentTo(configs.Select(x => x.ResultConfigId));
    }

    [Fact]
    public async Task GetResultConfigIds_ShouldReturnInOrderOfDependency_WhenSourceResultConfigIsConfigured()
    {
        var @event = await dbContext.Events.FirstAsync();
        int resultConfigCount = 5;
        var resultConfigs = accessMockHelper.ConfigurationBuilder(@event).CreateMany(resultConfigCount).ToList();
        // add dependencies
        resultConfigs[0].SourceResultConfigId = resultConfigs[2].ResultConfigId;
        resultConfigs[1].SourceResultConfigId = resultConfigs[0].ResultConfigId;
        resultConfigs[2].SourceResultConfigId = resultConfigs[4].ResultConfigId;
        resultConfigs[3].SourceResultConfigId = null;
        resultConfigs[4].SourceResultConfigId = resultConfigs[3].ResultConfigId;
        dbContext.ResultConfigurations.AddRange(resultConfigs);
        await dbContext.SaveChangesAsync();
        var expectedOrder = new[] { 3, 4, 2, 0, 1 };
        var sut = CreateSut();

        var test = await sut.GetResultConfigIds(@event.EventId);

        foreach ((var configId, var expIndex) in test.Zip(expectedOrder))
        {
            configId.Should().Be(resultConfigs[expIndex].ResultConfigId);
        }
    }

    [Fact]
    public async Task GetResultConfigIds_ShouldThrow_WhenSourceResultConfigContainsCyclicDependency()
    {
        var @event = await dbContext.Events.FirstAsync();
        int resultConfigCount = 5;
        var resultConfigs = accessMockHelper.ConfigurationBuilder(@event).CreateMany(resultConfigCount).ToList();
        // add dependencies
        resultConfigs[0].SourceResultConfigId = resultConfigs[1].ResultConfigId;
        resultConfigs[1].SourceResultConfigId = resultConfigs[0].ResultConfigId;
        resultConfigs[2].SourceResultConfigId = resultConfigs[4].ResultConfigId;
        resultConfigs[3].SourceResultConfigId = null;
        resultConfigs[4].SourceResultConfigId = resultConfigs[3].ResultConfigId;
        dbContext.ResultConfigurations.AddRange(resultConfigs);
        await dbContext.SaveChangesAsync();
        var expectedOrder = new[] { 3, 4, 2, 0, 1 };
        var sut = CreateSut();

        var test = async () => await sut.GetResultConfigIds(@event.EventId);

        await test.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetConfiguration_ShouldReturnDefaultConfiguration_WhenResultConfigIsNull()
    {
        var @event = await dbContext.Events
            .Include(x => x.Sessions)
            .FirstAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(@event.EventId, null);

        test.LeagueId.Should().Be(@event.LeagueId);
        test.EventId.Should().Be(@event.EventId);
        test.DisplayName.Should().Be("Default");
        test.ResultConfigId.Should().BeNull();
        test.SessionResultConfigurations.Should().HaveSameCount(@event.Sessions);
    }

    [Fact]
    public async Task GetConfiguration_ShouldReturnConfiguration_WithChampSeasonId()
    {
        var @event = await dbContext.Events
            .Include(x => x.Schedule)
                .ThenInclude(x => x.Season)
            .Include(x => x.Sessions)
            .FirstAsync();
        var championship = await dbContext.Championships.FirstAsync();
        var champSeason = accessMockHelper.CreateChampSeason(championship, @event.Schedule.Season);
        var config = accessMockHelper.CreateConfiguration(@event);
        champSeason.ResultConfigurations = new[] { config };
        dbContext.ChampSeasons.Add(champSeason);
        dbContext.ResultConfigurations.Add(config);
        await dbContext.SaveChangesAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(@event.EventId, config.ResultConfigId);

        test.ChampSeasonId.Should().Be(champSeason.ChampSeasonId);
    }

    [Fact]
    public async Task GetConfiguration_ShouldReturnConfiguration_WhenResultConfigIsNotNull()
    {
        var @event = await dbContext.Events
            .Include(x => x.Sessions)
            .FirstAsync();
        var config = accessMockHelper.CreateConfiguration(@event);
        dbContext.ResultConfigurations.Add(config);
        await dbContext.SaveChangesAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(@event.EventId, config.ResultConfigId);

        test.ResultConfigId.Should().Be(config.ResultConfigId);
    }

    [Fact]
    public async Task GetConfiguration_ShouldProvideDefaultResultId_WhenResultExistsAndConfigIsNull()
    {
        var @event = await dbContext.Events
            .Include(x => x.Sessions)
            .FirstAsync();
        var config = (ResultConfigurationEntity?)null;
        var eventResult = accessMockHelper.CreateScoredResult(@event, config);
        dbContext.ScoredEventResults.Add(eventResult);
        await dbContext.SaveChangesAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(@event.EventId, config?.ResultConfigId);

        test.ResultId.Should().Be(eventResult.ResultId);
    }

    [Fact]
    public async Task GetConfiguration_ShouldProvideConfigurationResultId_WhenResultExistsAndConfigIsNotNull()
    {
        var @event = await dbContext.Events
            .Include(x => x.Sessions)
            .FirstAsync();
        var config = accessMockHelper.CreateConfiguration(@event);
        var eventResult = accessMockHelper.CreateScoredResult(@event, config);
        dbContext.ScoredEventResults.Add(eventResult);
        dbContext.ResultConfigurations.Add(config);
        await dbContext.SaveChangesAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(@event.EventId, config.ResultConfigId);

        test.ResultConfigId.Should().Be(config.ResultConfigId);
        test.ResultId.Should().Be(eventResult.ResultId);
    }

    [Fact]
    public async Task GetConfiguration_ShouldProvideSourceConfigId_WhenSourceConfigIsConfigured()
    {
        var @event = await dbContext.Events
            .Include(x => x.Sessions)
            .FirstAsync();
        var sourceConfig = accessMockHelper.CreateConfiguration(@event);
        var config = accessMockHelper.CreateConfiguration(@event);
        config.SourceResultConfig = sourceConfig;
        dbContext.ResultConfigurations.Add(sourceConfig);
        dbContext.ResultConfigurations.Add(config);
        await dbContext.SaveChangesAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(@event.EventId, config.ResultConfigId);

        test.ResultConfigId.Should().Be(config.ResultConfigId);
        test.SourceResultConfigId.Should().Be(sourceConfig.ResultConfigId);
    }

    private EventCalculationConfigurationProvider CreateSut()
    {
        return fixture.Create<EventCalculationConfigurationProvider>();
    }
}
