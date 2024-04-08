using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Schedules;

public record GetSchedulesFromSeasonRequest(long SeasonId) : IRequest<IEnumerable<ScheduleModel>>;

public sealed class GetSchedulesFromSeasonHandler : ScheduleHandlerBase<GetSchedulesFromSeasonRequest, GetSchedulesFromSeasonRequest>,
    IRequestHandler<GetSchedulesFromSeasonRequest, IEnumerable<ScheduleModel>>
{
    public GetSchedulesFromSeasonHandler(ILogger<GetSchedulesFromSeasonRequest> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetSchedulesFromSeasonRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<ScheduleModel>> Handle(GetSchedulesFromSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getSchedules = await MapToGetScheduleModelsAsync(request.SeasonId, cancellationToken);
        return getSchedules;
    }

    private async Task<IEnumerable<ScheduleModel>> MapToGetScheduleModelsAsync(long seasonId, CancellationToken cancellationToken)
    {
        return await dbContext.Schedules
            .Where(x => x.SeasonId == seasonId)
            .Select(MapToGetScheduleModelExpression)
            .ToListAsync();
    }
}
