using FluentValidation;
using iRLeagueApiCore.Server.Handlers.Schedules;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using MediatR;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Schedules;

public sealed class DeleteScheduleDbTestFixture : HandlersTestsBase<DeleteScheduleHandler, DeleteScheduleRequest, Unit>
{
    public DeleteScheduleDbTestFixture() : base()
    {
    }

    protected override DeleteScheduleHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<DeleteScheduleRequest> validator)
    {
        return new DeleteScheduleHandler(logger, dbContext, new IValidator<DeleteScheduleRequest>[] { validator });
    }

    protected override DeleteScheduleRequest DefaultRequest()
    {
        return DefaultRequest(TestScheduleId);
    }

    private DeleteScheduleRequest DefaultRequest(long scheduleId)
    {
        return new DeleteScheduleRequest(scheduleId);
    }

    protected override void DefaultPreTestAssertions(DeleteScheduleRequest request, LeagueDbContext dbContext)
    {
        // assert schedule exists in dbContext
        Assert.Contains(dbContext.Schedules, x => x.ScheduleId == request.ScheduleId);
    }

    protected override void DefaultAssertions(DeleteScheduleRequest request, Unit result, LeagueDbContext dbContext)
    {
        // assert schedule was deleted
        Assert.DoesNotContain(dbContext.Schedules, x => x.ScheduleId == request.ScheduleId);
    }

    [Fact]
    public override async Task<Unit> ShouldHandleDefault()
    {
        return await base.ShouldHandleDefault();
    }

    [Fact]
    public override async Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }

    [Theory]
    [InlineData(0, defaultId)]
    [InlineData(defaultId, 0)]
    [InlineData(-42, defaultId)]
    [InlineData(defaultId, -42)]
    public async Task HandleNotFoundAsync(long? leagueId, long? scheduleId)
    {
        leagueId ??= TestLeagueId;
        scheduleId ??= TestScheduleId;
        accessMockHelper.SetCurrentLeague(leagueId.Value);
        var request = DefaultRequest(scheduleId.Value);
        await HandleNotFoundRequestAsync(request);
    }
}
