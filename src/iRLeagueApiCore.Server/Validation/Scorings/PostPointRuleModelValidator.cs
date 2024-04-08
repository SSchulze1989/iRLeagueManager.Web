using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Validation.Scorings;

public sealed class PostPointRuleModelValidator : AbstractValidator<PostPointRuleModel>
{
    public PostPointRuleModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
