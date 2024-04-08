using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Schedules;

public record GetSchedulesRequest() : IRequest<IEnumerable<ScheduleModel>>;

public sealed class GetSchedulesHandler : ScheduleHandlerBase<GetSchedulesHandler, GetSchedulesRequest>, IRequestHandler<GetSchedulesRequest, IEnumerable<ScheduleModel>>
{
    public GetSchedulesHandler(ILogger<GetSchedulesHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetSchedulesRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<ScheduleModel>> Handle(GetSchedulesRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getSchedules = await MapToGetScheduleModelsAsync(cancellationToken);
        if (getSchedules.Count() == 0)
        {
            throw new ResourceNotFoundException();
        }
        return getSchedules;
    }

    private async Task<IEnumerable<ScheduleModel>> MapToGetScheduleModelsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Schedules
            .Select(MapToGetScheduleModelExpression)
            .ToListAsync(cancellationToken);
    }
}
