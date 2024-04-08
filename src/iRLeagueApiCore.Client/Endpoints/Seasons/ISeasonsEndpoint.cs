using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Seasons;

public interface ISeasonsEndpoint : IPostEndpoint<SeasonModel, PostSeasonModel>, IGetAllEndpoint<SeasonModel>, IWithIdEndpoint<ISeasonByIdEndpoint>
{
    IGetEndpoint<SeasonModel> Current();
}
