using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Members;

public record GetMembersFromEventRequest(long EventId) : IRequest<IEnumerable<MemberModel>>;

public sealed class GetMembersFromEventHandler : MembersHandlerBase<GetMembersFromEventHandler, GetMembersFromEventRequest>,
    IRequestHandler<GetMembersFromEventRequest, IEnumerable<MemberModel>>
{
    public GetMembersFromEventHandler(ILogger<GetMembersFromEventHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetMembersFromEventRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<MemberModel>> Handle(GetMembersFromEventRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getMembers = await GetMembersFromEvent(request.EventId, cancellationToken);
        return getMembers;
    }

    private async Task<IEnumerable<MemberModel>> GetMembersFromEvent(long eventId, CancellationToken cancellationToken)
    {
        var resultMembers = await dbContext.EventResults
            .Where(x => x.EventId == eventId)
            .Select(x => x.SessionResults
                .SelectMany(y => y.ResultRows)
                .Select(y => y.MemberId))
            .SelectMany(x => x!)
            .ToListAsync(cancellationToken);
        var scoredResultMembers = await dbContext.ScoredEventResults
            .Where(x => x.EventId == eventId)
            .Select(x => x.ScoredSessionResults
                .SelectMany(y => y.ScoredResultRows)
                .Where(y => y.MemberId != null)
                .Select(y => y.MemberId.GetValueOrDefault()))
            .SelectMany(x => x)
            .ToListAsync(cancellationToken);
        var memberIds = resultMembers
            .Concat(scoredResultMembers)
            .Distinct();

        return await MapToMemberListAsync(memberIds, cancellationToken);
    }
}
