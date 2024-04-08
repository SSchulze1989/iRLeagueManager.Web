using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Validation.Events;

public sealed class PostEventModelValidator : AbstractValidator<PostEventModel>
{
    private readonly LeagueDbContext dbContext;

    public PostEventModelValidator(LeagueDbContext dbContext)
    {
        this.dbContext = dbContext;

        RuleFor(x => x.Sessions)
            .NotNull()
            .WithMessage("Sessions required");
        RuleForEach(x => x.Sessions)
            .MustAsync(SessionIdValid)
            .WithMessage("Session id must be either 0 or target a valid SessionEntity.SessionId");
        RuleFor(x => x.ResultConfigs)
            .MustAsync(OnlyOneResultConfigPerChampSeason)
            .WithMessage("Only one result config per championship can be selected")
            .MustAsync(MustIncludeDependResultConfigs)
            .WithMessage("At least one selected result configuration has \"From source\" set to another result configuration. Please make sure to include all depended on result configurations to prevent errors during calculation");
    }

    public async Task<bool> SessionIdValid(SessionModel session, CancellationToken cancellationToken)
    {
        var sessionId = session.SessionId;
        var result = sessionId == 0 ||
            await dbContext.Sessions
            .Where(x => x.SessionId == session.SessionId)
            .AnyAsync();
        return result;
    }

    private async Task<bool> OnlyOneResultConfigPerChampSeason(IEnumerable<ResultConfigInfoModel> configs, CancellationToken cancellationToken)
    {
        var configIds = configs.Select(x => x.ResultConfigId).ToList();
        var configEntities = await dbContext.ResultConfigurations
            .Where(x => configIds.Contains(x.ResultConfigId))
            .ToListAsync(cancellationToken);
        // Return true if list has no duplicates
        return configEntities
            .Select(x => x.ChampSeasonId)
            .Distinct()
            .Count() == configs.Count();
    }

    private async Task<bool> MustIncludeDependResultConfigs(IEnumerable<ResultConfigInfoModel> configs, CancellationToken cancellationToken)
    {
        var configIds = configs.Select(x => x.ResultConfigId).ToList();
        var configEntities = await dbContext.ResultConfigurations
            .Where(x => configIds.Contains(x.ResultConfigId))
            .ToListAsync(cancellationToken);
        // Return true if all dependant result config were found in event configs list (or source is null)
        return configEntities
            .All(x => x.SourceResultConfigId == null || configIds.Contains(x.SourceResultConfigId.Value));
    }
}
