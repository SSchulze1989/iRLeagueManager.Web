using iRLeagueApiCore.Server.Handlers.Reviews;

namespace iRLeagueApiCore.Server.Validation.Reviews;

public class PostProtestRequestValidator : AbstractValidator<PostProtestToSessionRequest>
{
    public PostProtestRequestValidator(PostProtestModelValidator modelValidator)
    {
        RuleFor(x => x.Model)
            .SetValidator(modelValidator);
    }
}
