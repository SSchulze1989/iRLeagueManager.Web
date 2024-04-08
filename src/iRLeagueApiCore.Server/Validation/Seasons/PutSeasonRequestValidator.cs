using iRLeagueApiCore.Server.Handlers.Seasons;

namespace iRLeagueApiCore.Server.Validation.Seasons;

public sealed class PutSeasonRequestValidator : AbstractValidator<PutSeasonRequest>
{
    private readonly LeagueDbContext dbContext;

    public PutSeasonRequestValidator(LeagueDbContext dbContext, PutSeasonModelValidator modelValidator)
    {
        this.dbContext = dbContext;

        RuleFor(x => x.SeasonId)
            .NotEmpty()
            .WithMessage("Season id required");
        RuleFor(x => x.Model)
            .SetValidator(modelValidator);
        RuleFor(x => x.Model.MainScoringId)
            .MustAsync(ScoringExists)
            .When(x => x.Model.MainScoringId != null)
            .WithMessage("Main scoring not found");
    }

    private async Task<bool> ScoringExists(PutSeasonRequest request, long? scoringId, CancellationToken cancellationToken)
    {
        return await dbContext.Scorings
            .AnyAsync(x => x.ScoringId == scoringId);
    }
}
