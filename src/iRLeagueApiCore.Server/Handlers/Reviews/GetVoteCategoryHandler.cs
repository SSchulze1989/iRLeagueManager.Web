using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record GetVoteCategoryRequest(long CatId) : IRequest<VoteCategoryModel>;

public class GetVoteCategoryHandler : VoteCategoriesHandlerBase<GetVoteCategoryHandler, GetVoteCategoryRequest>, 
    IRequestHandler<GetVoteCategoryRequest, VoteCategoryModel>
{
    public GetVoteCategoryHandler(ILogger<GetVoteCategoryHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetVoteCategoryRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<VoteCategoryModel> Handle(GetVoteCategoryRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getVoteCategory = await MapToVoteCategoryModel(request.CatId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getVoteCategory;
    }
}
