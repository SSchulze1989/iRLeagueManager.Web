using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Results;

namespace iRLeagueApiCore.Server.Validation.Results;

public sealed class PostResultConfigRequestValidator : AbstractValidator<PostResultConfigToChampSeasonRequest>
{
    private readonly LeagueDbContext dbContext;

    public PostResultConfigRequestValidator(LeagueDbContext dbContext, PostResultConfigModelValidator modelValidator)
    {
        this.dbContext = dbContext;
        RuleFor(x => x.Model)
            .SetValidator(modelValidator);
        RuleForEach(x => x.Model.Scorings)
            .Must(ScoringIdZero);
    }

    private bool ScoringIdZero(ScoringModel scoringModel)
    {
        return scoringModel.Id == 0;
    }
}
