using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Schedules;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Schedules;

public sealed class PutScheduleDbTestFixture : HandlersTestsBase<PutScheduleHandler, PutScheduleRequest, ScheduleModel>
{
    private const string testScheduleName = "TestSchedule";

    public PutScheduleDbTestFixture() : base()
    {
    }

    protected override PutScheduleHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PutScheduleRequest> validator)
    {
        return new PutScheduleHandler(logger, dbContext, new IValidator<PutScheduleRequest>[] { validator });
    }

    protected override PutScheduleRequest DefaultRequest()
    {
        return DefaultRequest(TestScheduleId);
    }

    private PutScheduleRequest DefaultRequest(long scheduleId)
    {
        var model = new PutScheduleModel()
        {
            Name = testScheduleName
        };
        return new PutScheduleRequest(DefaultUser(), scheduleId, model);
    }

    protected override void DefaultAssertions(PutScheduleRequest request, ScheduleModel result, LeagueDbContext dbContext)
    {
        var expected = request.Model;
        result.ScheduleId.Should().Be(request.ScheduleId);
        result.Name.Should().Be(expected.Name);
        AssertChanged(request.User, DateTime.UtcNow, result);
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
