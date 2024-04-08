using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Championships;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Championships;
public sealed class PutChampSeasonHandlerTests : ChampionshipHandlersTestsBase<PutChampSeasonHandler, PutChampSeasonRequest, ChampSeasonModel>
{
    protected override PutChampSeasonHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PutChampSeasonRequest> validator)
    {
        return new(logger, dbContext, new[] { validator });
    }

    protected override PutChampSeasonRequest DefaultRequest()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ShouldSetDefaultResultConfig()
    {
        var champSeason = await dbContext.ChampSeasons
            .Include(x => x.DefaultResultConfig)
            .Include(x => x.ResultConfigurations)
            .FirstAsync();
        var defaultResultConfig = champSeason.ResultConfigurations.Last();
        champSeason.DefaultResultConfig?.ResultConfigId.Should().NotBe(defaultResultConfig.ResultConfigId);
        var request = new PutChampSeasonRequest(champSeason.ChampSeasonId, DefaultUser(), new()
        {
            DefaultResultConfig = new() { LeagueId = champSeason.LeagueId, ResultConfigId = defaultResultConfig.ResultConfigId },
            ResultConfigs = champSeason.ResultConfigurations.Select(x => new ResultConfigInfoModel() { ResultConfigId = x.ResultConfigId, Name = x.Name }).ToArray(),
        });
        var sut = CreateSut();

        var test = await sut.Handle(request, default);

        test.DefaultResultConfig.Should().NotBeNull();
        test.DefaultResultConfig!.ResultConfigId.Should().Be(defaultResultConfig.ResultConfigId);
    }

    private PutChampSeasonHandler CreateSut()
    {
        return CreateTestHandler(dbContext, MockHelpers.TestValidator<PutChampSeasonRequest>());
    }
}
