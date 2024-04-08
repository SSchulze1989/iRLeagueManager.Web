using iRLeagueApiCore.Common.Models.Reviews;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public class VoteCategoriesHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public VoteCategoriesHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    protected virtual async Task<VoteCategoryEntity?> GetVoteCategoryEntityAsync(long catId, CancellationToken cancellationToken)
    {
        return await dbContext.VoteCategories
            .Where(x => x.CatId == catId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual async Task<IEnumerable<VoteCategoryEntity>> GetCategoryEntitiesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.VoteCategories
            .ToListAsync(cancellationToken);
    }

    protected virtual async Task<VoteCategoryEntity> MapToVoteCategoryEntityAsync(PostVoteCategoryModel model, VoteCategoryEntity target, CancellationToken cancellationToken)
    {
        target.DefaultPenalty = model.DefaultPenalty;
        target.Index = model.Index;
        target.Text = model.Text;
        return await Task.FromResult(target);
    }

    protected virtual async Task<VoteCategoryModel?> MapToVoteCategoryModel(long catId, CancellationToken cancellationToken)
    {
        return await dbContext.VoteCategories
            .Where(x => x.CatId == catId)
            .Select(MapToVoteCategoryModelExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual Expression<Func<VoteCategoryEntity, VoteCategoryModel>> MapToVoteCategoryModelExpression => cat => new VoteCategoryModel()
    {
        Id = cat.CatId,
        DefaultPenalty = cat.DefaultPenalty,
        Index = cat.Index,
        Text = cat.Text,
    };
}
