using iRLeagueApiCore.Server.Handlers.Events;

namespace iRLeagueApiCore.Server.Validation.Events;

public sealed class PostEventToScheduleRequestValidator : AbstractValidator<PostEventToScheduleRequest>
{
    private readonly LeagueDbContext dbContext;

    public PostEventToScheduleRequestValidator(LeagueDbContext dbContext, PostEventModelValidator eventValidator)
    {
        this.dbContext = dbContext;

        RuleFor(x => x.ScheduleId)
            .MustAsync(EventExists)
            .WithMessage("No entry for schedule found with combination of LeagueId and ScheduleId");
        RuleFor(x => x.Event)
            .SetValidator(eventValidator);
    }

    public async Task<bool> EventExists(PostEventToScheduleRequest request, long scheduleId, CancellationToken cancellationToken)
    {
        return await dbContext.Schedules
            .Where(x => x.ScheduleId == scheduleId)
            .AnyAsync(cancellationToken);
    }
}
