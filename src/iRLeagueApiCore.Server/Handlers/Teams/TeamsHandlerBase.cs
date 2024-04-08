using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Teams;

public abstract class TeamsHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    protected TeamsHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    protected virtual async Task<TeamEntity?> GetTeamEntity(long teamId, CancellationToken cancellationToken)
    {
        return await dbContext.Teams
            .Include(x => x.Members)
            .Where(x => x.TeamId == teamId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual async Task<TeamEntity> CreateTeamEntity(LeagueUser user, CancellationToken cancellationToken)
    {
        var leagueId = dbContext.LeagueProvider.LeagueId;
        var league = await dbContext.Leagues
            .FirstOrDefaultAsync(x => x.Id == leagueId, cancellationToken);
        if (league is null)
        {
            throw new InvalidOperationException($"League with id: {leagueId} does not exist");
        }

        var team = CreateVersionEntity(user, new TeamEntity());
        team.League = league;
        team.LeagueId = leagueId;
        league.Teams.Add(team);
        return team;
    }

    protected virtual async Task<TeamEntity> MapToTeamEntityAsync(LeagueUser user, PostTeamModel model, TeamEntity entity, CancellationToken cancellationToken)
    {
        entity.Name = model.Name;
        entity.Profile = model.Profile;
        entity.TeamColor = model.TeamColor;
        entity.TeamHomepage = model.TeamHomepage;
        entity.Members = await MapToTeamMemberListAsync(model.Members, entity.Members, cancellationToken);
        entity.IRacingTeamId = model.IRacingTeamId;
        return UpdateVersionEntity(user, entity);
    }

    private async Task<ICollection<LeagueMemberEntity>> MapToTeamMemberListAsync(ICollection<MemberInfoModel> memberModels, 
        ICollection<LeagueMemberEntity> memberEntities, CancellationToken cancellationToken)
    {
        var memberIds = memberModels.Select(x => x.MemberId);
        var leagueMembers = await dbContext.LeagueMembers
            .Where(x => memberIds.Contains(x.MemberId))
            .ToListAsync(cancellationToken);

        return leagueMembers.ToList();
    }

    protected virtual async Task<TeamModel?> MapToTeamModel(long teamId, CancellationToken cancellationToken)
    {
        return await dbContext.Teams
            .Where(x => teamId == x.TeamId)
            .Select(MapToTeamModelExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual Expression<Func<TeamEntity, TeamModel>> MapToTeamModelExpression => team => new()
    {
        Members = team.Members.Select(leagueMember => new MemberInfoModel()
        {
            FirstName = leagueMember.Member.Firstname,
            LastName = leagueMember.Member.Lastname,
            MemberId = leagueMember.Member.Id,
        }).ToList(),
        TeamColor = team.TeamColor,
        TeamHomepage = team.TeamHomepage,
        TeamId = team.TeamId,
        IRacingTeamId = team.IRacingTeamId,
        Name = team.Name,
        Profile = team.Profile,
    };
}
