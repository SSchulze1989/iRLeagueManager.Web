using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Models.Payments;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Validation.AdminPanel.Payments;

public class PostPaymentModelValidator : AbstractValidator<PostPaymentModel>
{
    private readonly LeagueDbContext dbContext;
    private readonly UserManager<ApplicationUser> userManager;

    public PostPaymentModelValidator(LeagueDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        RuleFor(x => x.PlanId)
            .MustAsync(SubscriptionPlanExists)
            .WithMessage("PlanId was not a valid subscription plan");
        RuleFor(x => x.UserId)
            .MustAsync(UserExists)
            .WithMessage("No user with UserId was found");
    }

    private async Task<bool> SubscriptionPlanExists(string planId, CancellationToken cancellationToken)
    {
        return await dbContext.Subscriptions.AnyAsync(x => x.PlanId == planId, cancellationToken);
    }

    private async Task<bool> UserExists(string userId, CancellationToken cancellationToken)
    {
        return await userManager.FindByIdAsync(userId) is not null;
    }
}
