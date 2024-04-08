using iRLeagueApiCore.Server.Handlers.AdminPanel;

namespace iRLeagueApiCore.Server.Validation.AdminPanel.Payments;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    private readonly LeagueDbContext dbContext;

    public PostPaymentRequestValidator(PostPaymentModelValidator modelValidator, LeagueDbContext dbContext)
    {
        this.dbContext = dbContext;
        RuleFor(x => x.Model)
            .SetValidator(modelValidator);
        RuleFor(x => x.LeagueId)
            .MustAsync(LeagueExists)
            .WithMessage("No league with given leagueId exists");
    }

    private async Task<bool> LeagueExists(long leagueId, CancellationToken cancellationToken)
    {
        return await dbContext.Leagues.AnyAsync(x => x.Id == leagueId, cancellationToken);
    }
}
