using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Schedules;

public record PutScheduleRequest(LeagueUser User, long ScheduleId, PutScheduleModel Model) : IRequest<ScheduleModel>;

public sealed class PutScheduleHandler : ScheduleHandlerBase<PutScheduleHandler, PutScheduleRequest>,
    IRequestHandler<PutScheduleRequest, ScheduleModel>
{
    public PutScheduleHandler(ILogger<PutScheduleHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutScheduleRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<ScheduleModel> Handle(PutScheduleRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putSchedule = await GetScheduleEntityAsync(request.ScheduleId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        putSchedule = MapToScheduleEntity(request.User, request.Model, putSchedule);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getSchedule = await MapToGetScheduleModelAsync(request.ScheduleId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getSchedule;
    }
}
