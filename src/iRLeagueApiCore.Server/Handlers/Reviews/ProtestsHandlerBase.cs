using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public class ProtestsHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public ProtestsHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    protected virtual async Task<ProtestEntity?> GetProtestEntityAsync(long protestId, CancellationToken cancellationToken)
    {
        return await dbContext.Protests
            .Where(x => x.ProtestId == protestId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual async Task<ProtestEntity> MapToProtestEntity(LeagueUser user, PostProtestModel model, ProtestEntity entity, CancellationToken cancellationToken)
    {
        entity.Author = await dbContext.LeagueMembers
            .Where(x => x.MemberId == model.AuthorMemberId)
            .FirstAsync(cancellationToken);
        entity.Corner = model.Corner;
        entity.FullDescription = model.FullDescription;
        entity.OnLap = model.OnLap;
        entity.InvolvedMembers = await MapToLeagueMemberList(model.InvolvedMembers.Select(x => x.MemberId), entity.InvolvedMembers, cancellationToken);
        return entity;
    }

    protected virtual async Task<ICollection<LeagueMemberEntity>> MapToLeagueMemberList(IEnumerable<long> involvedMemberIds, 
        ICollection<LeagueMemberEntity> involvedMembers, CancellationToken cancellationToken)
    {
        var members = await dbContext.LeagueMembers
            .Where(x => involvedMemberIds.Contains(x.MemberId))
            .ToListAsync(cancellationToken);
        return members;
    }

    protected virtual async Task<ProtestModel?> MapToProtestModel(long protestId, bool includeAuthor, CancellationToken cancellationToken)
    {
        return await dbContext.Protests
            .Where(x => x.ProtestId == protestId)
            .Select(MapToProtestModelExpression(includeAuthor))
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual Expression<Func<ProtestEntity, ProtestModel>> MapToProtestModelExpression(bool includeAutho = false) => protest => new()
    {
        Author = includeAutho == false ? new() : new()
        {
            FirstName = protest.Author.Member.Firstname,
            LastName = protest.Author.Member.Lastname,
            MemberId = protest.Author.MemberId,
        },
        Corner = protest.Corner,
        EventId = protest.Session.EventId,
        FullDescription = protest.FullDescription,
        InvolvedMembers = protest.InvolvedMembers.Select(leagueMember => new MemberInfoModel()
        {
            FirstName = leagueMember.Member.Firstname,
            LastName = leagueMember.Member.Lastname,
            MemberId = leagueMember.MemberId,
        }).ToList(),
        OnLap = protest.OnLap,
        ProtestId = protest.ProtestId,
        SessionId = protest.SessionId,
        SessionNr = protest.Session.SessionNr,
        SessionName = protest.Session.Name,
    };
}
