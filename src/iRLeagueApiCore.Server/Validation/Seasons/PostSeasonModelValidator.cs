using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Validation.Seasons;

public sealed class PostSeasonModelValidator : AbstractValidator<PostSeasonModel>
{
    public PostSeasonModelValidator()
    {
        RuleFor(x => x.SeasonName)
            .NotEmpty()
            .WithMessage("Season name required");
    }
}
