using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Validation.Seasons;

public sealed class PutSeasonModelValidator : AbstractValidator<PutSeasonModel>
{
    public PutSeasonModelValidator(PostSeasonModelValidator putSeasonValidator)
    {
        Include(putSeasonValidator);
    }
}
