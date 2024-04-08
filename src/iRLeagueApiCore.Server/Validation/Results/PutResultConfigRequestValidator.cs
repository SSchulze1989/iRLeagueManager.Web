using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Results;

namespace iRLeagueApiCore.Server.Validation.Results;

public sealed class PutResultConfigRequestValidator : AbstractValidator<PutResultConfigRequest>
{
    private readonly LeagueDbContext dbContext;

    public PutResultConfigRequestValidator(LeagueDbContext dbContext, PutResultConfigModelValidator modelValidator)
    {
        this.dbContext = dbContext;
        RuleFor(x => x.Model)
            .SetValidator(modelValidator);
        RuleForEach(x => x.Model.Scorings)
            .MustAsync(ScoringIdValid);
    }

    private async Task<bool> ScoringIdValid(PutResultConfigRequest request, ScoringModel scoringModel, CancellationToken cancellationToken)
    {
        if (scoringModel.Id == 0)
        {
            return true;
        }
        var exists = await dbContext.Scorings
            .Where(x => x.ScoringId == scoringModel.Id)
            .Select(x => new { x.ResultConfigId })
            .FirstOrDefaultAsync(cancellationToken);
        if (exists != null && exists.ResultConfigId == request.ResultConfigId)
        {
            return true;
        }
        return false;
    }
}
