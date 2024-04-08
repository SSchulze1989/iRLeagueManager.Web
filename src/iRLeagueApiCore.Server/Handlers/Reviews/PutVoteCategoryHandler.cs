using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record PutVoteCategoryRequest(long CatId, PutVoteCategoryModel Model) : IRequest<VoteCategoryModel>;

public class PutVoteCategoryHandler : VoteCategoriesHandlerBase<PutVoteCategoryHandler, PutVoteCategoryRequest>, 
    IRequestHandler<PutVoteCategoryRequest, VoteCategoryModel>
{
    public PutVoteCategoryHandler(ILogger<PutVoteCategoryHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutVoteCategoryRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<VoteCategoryModel> Handle(PutVoteCategoryRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putVoteCategory = await GetVoteCategoryEntityAsync(request.CatId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        putVoteCategory = await MapToVoteCategoryEntityAsync(request.Model, putVoteCategory, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getVoteCategory = await MapToVoteCategoryModel(putVoteCategory.CatId, cancellationToken)
            ?? throw new InvalidOperationException("Updated resource not found");
        return getVoteCategory;
    }
}
