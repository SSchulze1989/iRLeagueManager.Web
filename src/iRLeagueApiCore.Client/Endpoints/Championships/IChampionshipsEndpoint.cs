using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Championships;
public interface IChampionshipsEndpoint : IPostGetAllEndpoint<ChampionshipModel, PostChampionshipModel>, IWithIdEndpoint<IChampionshipByIdEndpoint>
{
}
