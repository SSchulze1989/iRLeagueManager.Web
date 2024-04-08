using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Championships;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Championships;
public sealed class PostChampSeasonHandlerTests : ChampionshipHandlersTestsBase<PostChampSeasonHandler, PostChampSeasonRequest, ChampSeasonModel>
{
    protected override PostChampSeasonHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PostChampSeasonRequest> validator)
    {
        return new PostChampSeasonHandler(logger, dbContext, new[] { validator });
    }

    protected override PostChampSeasonRequest DefaultRequest()
    {
        return DefaultRequest(TestChampionshipId, TestSeasonId);
    }

    private PostChampSeasonRequest DefaultRequest(long championshipId, long seasonId)
    {
        return new(championshipId, seasonId, DefaultUser(), new());
    }

    [Fact]
    public async Task HandleDefault()
    {
        await ShouldHandleDefault();
    }

    [Fact]
    public async Task Handle_ShouldCopySettings_WhenPreviousSeasonExists()
    {
        var request = DefaultRequest(TestChampionshipId, (await dbContext.Seasons.Skip(1).FirstAsync()).SeasonId);
        var champSeasonQuery = dbContext.ChampSeasons
            .Include(x => x.Championship)
            .Include(x => x.ResultConfigurations)
                .ThenInclude(x => x.Scorings)
                    .ThenInclude(x => x.PointsRule)
            .Include(x => x.ResultConfigurations)
                .ThenInclude(x => x.PointFilters)
            .Include(x => x.ResultConfigurations)
                .ThenInclude(x => x.ResultFilters)
            .Include(x => x.StandingConfiguration);
        var prevChampSeason = await champSeasonQuery
            .Where(x => x.ChampionshipId == TestChampionshipId)
            .FirstAsync();
        await HandleSpecialAsync(request, async (request, model, context) =>
        {
            var postChampSeason = await champSeasonQuery
                .Where(x => x.ChampSeasonId == model.ChampSeasonId)
                .FirstAsync();
            postChampSeason.ChampionshipId.Should().Be(prevChampSeason.ChampionshipId);
            CompareStandingConfigurationEntity(postChampSeason.StandingConfiguration, prevChampSeason.StandingConfiguration);
            postChampSeason.ResultConfigurations.Should().HaveSameCount(prevChampSeason.ResultConfigurations);
            postChampSeason.ResultConfigurations.Zip(prevChampSeason.ResultConfigurations)
                .ToList()
                .ForEach(x => CompareResultConfigurationEntity(x.First, x.Second));
        },
        async (request, context) =>
        {
            var existingChampSeason = await champSeasonQuery
                .Where(x => x.ChampionshipId == request.ChampionshipId)
                .Where(x => x.SeasonId == request.SeasonId)
                .FirstOrDefaultAsync();
            existingChampSeason.Should().BeNull();
        });
    }

    [Fact]
    public async Task Handle_ShouldActivateExistingSeason_WhenInactive()
    {
        var champSeason = await dbContext.ChampSeasons.FirstAsync();
        champSeason.IsActive = false;
        await dbContext.SaveChangesAsync();

        var request = DefaultRequest(champSeason.ChampionshipId, champSeason.SeasonId);
        await HandleSpecialAsync(request, async (request, model, context) =>
        {
            var test = await context.ChampSeasons.FirstAsync();
            test.IsActive.Should().BeTrue();
        });
    }
}
