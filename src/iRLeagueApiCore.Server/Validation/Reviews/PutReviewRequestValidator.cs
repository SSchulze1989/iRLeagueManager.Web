using iRLeagueApiCore.Server.Handlers.Reviews;

namespace iRLeagueApiCore.Server.Validation.Reviews;

public sealed class PutReviewRequestValidator : AbstractValidator<PutReviewRequest>
{
    public PutReviewRequestValidator(LeagueDbContext dbContext)
    {
        RuleFor(x => x.Model)
            .SetValidator(new PostReviewModelValidator(dbContext));
    }
}
