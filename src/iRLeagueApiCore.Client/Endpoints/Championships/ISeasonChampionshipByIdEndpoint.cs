using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Championships;

public interface ISeasonChampionshipByIdEndpoint : IPostEndpoint<ChampSeasonModel, PostChampSeasonModel>, IGetEndpoint<ChampSeasonModel>
{
}