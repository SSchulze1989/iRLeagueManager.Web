using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Validation.Scorings;

public sealed class PutPointRuleModelValidator : AbstractValidator<PutPointRuleModel>
{
    public PutPointRuleModelValidator(PostPointRuleModelValidator parentValidator)
    {
        Include(parentValidator);
    }
}
