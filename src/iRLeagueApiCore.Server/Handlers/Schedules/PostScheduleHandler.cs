using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Schedules;

public record PostScheduleRequest(long seasonId, LeagueUser User, PostScheduleModel Model) : IRequest<ScheduleModel>;

public sealed class PostScheduleHandler : ScheduleHandlerBase<PostScheduleHandler, PostScheduleRequest>,
    IRequestHandler<PostScheduleRequest, ScheduleModel>
{
    public PostScheduleHandler(ILogger<PostScheduleHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PostScheduleRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<ScheduleModel> Handle(PostScheduleRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var postSchedule = await CreateScheduleEntityAsync(request.seasonId, request.User, cancellationToken);
        postSchedule = MapToScheduleEntity(request.User, request.Model, postSchedule);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getSchedule = await MapToGetScheduleModelAsync(postSchedule.ScheduleId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getSchedule;
    }

    private async Task<ScheduleEntity> CreateScheduleEntityAsync(long seasonId, LeagueUser user, CancellationToken cancellationToken)
    {
        var scheduleEntity = new ScheduleEntity()
        {
            CreatedByUserId = user.Id,
            CreatedByUserName = user.Name,
            CreatedOn = DateTime.UtcNow
        };
        var season = await dbContext.Seasons
            .SingleOrDefaultAsync(x => x.SeasonId == seasonId)
            ?? throw new ResourceNotFoundException();
        season.Schedules.Add(scheduleEntity);
        return scheduleEntity;
    }
}
