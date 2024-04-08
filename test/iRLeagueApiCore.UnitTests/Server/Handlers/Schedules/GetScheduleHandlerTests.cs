using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Schedules;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Schedules;

public sealed class GetScheduleDbTestFixture : HandlersTestsBase<GetScheduleHandler, GetScheduleRequest, ScheduleModel>
{
    public GetScheduleDbTestFixture() : base()
    {
    }

    protected override GetScheduleHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetScheduleRequest> validator)
    {
        return new GetScheduleHandler(logger, dbContext, new IValidator<GetScheduleRequest>[] { validator });
    }

    protected override GetScheduleRequest DefaultRequest()
    {
        return DefaultRequest(TestScheduleId);
    }

    private GetScheduleRequest DefaultRequest(long scheduleId)
    {
        return new GetScheduleRequest(scheduleId);
    }

    protected override void DefaultAssertions(GetScheduleRequest request, ScheduleModel result, LeagueDbContext dbContext)
    {
        result.ScheduleId.Should().Be(request.ScheduleId);
        var scheduleEntity = dbContext.Schedules
            .Include(x => x.Events)
            .SingleOrDefault(x => x.ScheduleId == result.ScheduleId);
        scheduleEntity.Should().NotBeNull();
        result.Name.Should().Be(scheduleEntity!.Name);
        result.EventIds.Should().BeEquivalentTo(scheduleEntity.Events.Select(x => x.EventId));
        result.SeasonId.Should().Be(scheduleEntity.SeasonId);
        AssertVersion(scheduleEntity, result);
        base.DefaultAssertions(request, result, dbContext);
    }

    [Fact]
    public override async Task<ScheduleModel> ShouldHandleDefault()
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
    [InlineData(-43, defaultId)]
    [InlineData(defaultId, -43)]
    public async Task HandleNotFoundAsync(long? leagueId, long? scheduleId)
    {
        leagueId ??= TestLeagueId;
        scheduleId ??= TestScheduleId;
        accessMockHelper.SetCurrentLeague(leagueId.Value);
        var request = DefaultRequest(scheduleId.Value);
        await base.HandleNotFoundRequestAsync(request);
    }
}
