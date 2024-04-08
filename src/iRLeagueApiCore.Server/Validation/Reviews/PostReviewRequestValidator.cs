using iRLeagueApiCore.Server.Handlers.Reviews;

namespace iRLeagueApiCore.Server.Validation.Reviews;

public sealed class PostReviewRequestValidator : AbstractValidator<PostReviewToSessionRequest>
{
    private readonly LeagueDbContext dbContext;

    public PostReviewRequestValidator(LeagueDbContext dbContext)
    {
        this.dbContext = dbContext;

        RuleFor(x => x.SessionId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("No session selected")
            .MustAsync((request, sessionId, cancellationToken) => SessionisValid(sessionId, cancellationToken))
            .WithMessage("Selected session does not exist");
        RuleFor(x => x.Model)
            .SetValidator(new PostReviewModelValidator(dbContext));
    }

    private async Task<bool> SessionisValid(long sessionId, CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .Where(x => x.SessionId == sessionId)
            .AnyAsync(cancellationToken);
    }
}
