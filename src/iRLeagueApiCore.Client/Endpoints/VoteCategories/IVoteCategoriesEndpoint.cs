using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.VoteCategories;

public interface IVoteCategoriesEndpoint : IPostGetAllEndpoint<VoteCategoryModel, PostVoteCategoryModel>, IWithIdEndpoint<IVoteCategoryByIdEndpoint>
{
}
