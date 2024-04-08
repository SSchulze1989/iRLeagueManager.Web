using iRLeagueApiCore.Client.Endpoints;
using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.VoteCategories;

public interface IVoteCategoryByIdEndpoint : IUpdateEndpoint<VoteCategoryModel, PutVoteCategoryModel>
{
}