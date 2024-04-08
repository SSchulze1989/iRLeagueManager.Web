using FluentValidation;
using iRLeagueApiCore.Server.Handlers.Championships;
using iRLeagueDatabaseCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Championships;
public sealed class DeleteChampSeasonHandlerTests :
    ChampionshipHandlersTestsBase<DeleteChampSeasonHandler, DeleteChampSeasonRequest, Unit>
{
    protected override DeleteChampSeasonHandler CreateTestHandler(LeagueDbContext dbContext, 
        IValidator<DeleteChampSeasonRequest> validator)
    {
        return new DeleteChampSeasonHandler(logger, dbContext, new[] { validator });
    }

    protected override DeleteChampSeasonRequest DefaultRequest()
    {
        return DefaultRequest(TestChampSeasonId);
    }

    private DeleteChampSeasonRequest DefaultRequest(long champSeasonId)
    {
        return new DeleteChampSeasonRequest(champSeasonId);
    }

    protected override void DefaultAssertions(DeleteChampSeasonRequest request, Unit result, LeagueDbContext dbContext)
    {
        var test = dbContext.ChampSeasons.FirstOrDefault(x => x.ChampSeasonId == request.ChampSeasonId);
        test.Should().NotBeNull();
        test!.IsActive.Should().BeFalse();
        base.DefaultAssertions(request, result, dbContext);
    }

    [Fact]
    public async Task HandleDefault()
    {
        await ShouldHandleDefault();
    }
}
