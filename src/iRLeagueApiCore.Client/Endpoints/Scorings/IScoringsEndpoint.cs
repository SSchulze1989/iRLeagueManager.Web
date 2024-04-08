using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Scorings;

public interface IScoringsEndpoint : IPostGetAllEndpoint<ScoringModel, PostScoringModel>, IWithIdEndpoint<IScoringByIdEndpoint>
{
}
