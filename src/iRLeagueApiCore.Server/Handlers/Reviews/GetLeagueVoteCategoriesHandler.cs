using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record GetLeagueVoteCategoriesRequest() : IRequest<IEnumerable<VoteCategoryModel>>;

public sealed class GetLeagueVoteCategoriesHandler : VoteCategoriesHandlerBase<GetLeagueVoteCategoriesHandler, GetLeagueVoteCategoriesRequest>,
    IRequestHandler<GetLeagueVoteCategoriesRequest, IEnumerable<VoteCategoryModel>>
{
    public GetLeagueVoteCategoriesHandler(ILogger<GetLeagueVoteCategoriesHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<GetLeagueVoteCategoriesRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<VoteCategoryModel>> Handle(GetLeagueVoteCategoriesRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);

        var getVoteCategories = await MapToLeagueVoteCategoryModels(cancellationToken);
        return getVoteCategories;
    }

    private async Task<IEnumerable<VoteCategoryModel>> MapToLeagueVoteCategoryModels(CancellationToken cancellationToken)
    {
        return await dbContext.VoteCategories
            .OrderBy(x => x.Index)
            .Select(MapToVoteCategoryModelExpression)
            .ToListAsync(cancellationToken);
    }
}
