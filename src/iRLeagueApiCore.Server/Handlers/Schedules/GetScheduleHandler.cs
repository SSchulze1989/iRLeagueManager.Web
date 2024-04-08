using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Schedules;

public record GetScheduleRequest(long ScheduleId) : IRequest<ScheduleModel>;

public sealed class GetScheduleHandler : ScheduleHandlerBase<GetScheduleHandler, GetScheduleRequest>,
    IRequestHandler<GetScheduleRequest, ScheduleModel>
{
    public GetScheduleHandler(ILogger<GetScheduleHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetScheduleRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<ScheduleModel> Handle(GetScheduleRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getSchedule = await MapToGetScheduleModelAsync(request.ScheduleId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getSchedule;
    }
}
