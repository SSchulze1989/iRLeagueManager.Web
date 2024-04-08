using iRLeagueApiCore.Server.Handlers.Scorings;

namespace iRLeagueApiCore.Server.Validation.Scorings;

public sealed class PostPointRuleRequestValidator : AbstractValidator<PostPointRuleRequest>
{
    public PostPointRuleRequestValidator(PostPointRuleModelValidator modelValidator)
    {
        RuleFor(x => x.Model)
            .SetValidator(modelValidator);
    }
}
