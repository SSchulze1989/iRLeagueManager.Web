using iRLeagueApiCore.Common.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Members;

public class MembersHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public MembersHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    protected async Task<IEnumerable<MemberModel>> MapToMemberListAsync(IEnumerable<long> memberIds, CancellationToken cancellationToken)
    {
        return await dbContext.LeagueMembers
            .Where(x => memberIds.Contains(x.MemberId))
            .Select(MapToMemberModelExpression)
            .ToListAsync(cancellationToken);
    }

    protected Expression<Func<MemberEntity, MemberInfoModel>> MapToMemberInfoExpression => member => new()
    {
        MemberId = member.Id,
        FirstName = member.Firstname,
        LastName = member.Lastname,
    };

    protected Expression<Func<LeagueMemberEntity, MemberModel>> MapToMemberModelExpression => member => new()
    {
        MemberId = member.Member.Id,
        FirstName = member.Member.Firstname,
        LastName= member.Member.Lastname,
        IRacingId = member.Member.IRacingId,
        TeamName = member.Team == null ? string.Empty : member.Team.Name,
    };
}
