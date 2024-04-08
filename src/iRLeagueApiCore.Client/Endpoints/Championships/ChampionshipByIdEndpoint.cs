using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Championships;
internal sealed class ChampionshipByIdEndpoint : UpdateEndpoint<ChampionshipModel, PutChampionshipModel>, IChampionshipByIdEndpoint, ISeasonChampionshipByIdEndpoint
{
    public ChampionshipByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long id) : 
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddParameter(id);
    }

    IChampionshipChampSeasonsEndpoint IChampionshipByIdEndpoint.ChampSeasons()
    {
        return new ChampSeasonsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    async Task<ClientActionResult<ChampSeasonModel>> IGetEndpoint<ChampSeasonModel>.Get(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.GetAsClientActionResult<ChampSeasonModel>(QueryUrl, cancellationToken);
    }

    async Task<ClientActionResult<ChampSeasonModel>> IPostEndpoint<ChampSeasonModel, PostChampSeasonModel>.Post(PostChampSeasonModel model, CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.PostAsClientActionResult<ChampSeasonModel>(QueryUrl, model, cancellationToken);
    }
}
