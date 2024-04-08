using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Championships;

public interface IChampionshipByIdEndpoint : IUpdateEndpoint<ChampionshipModel, PutChampionshipModel>
{
    public IChampionshipChampSeasonsEndpoint ChampSeasons();
}