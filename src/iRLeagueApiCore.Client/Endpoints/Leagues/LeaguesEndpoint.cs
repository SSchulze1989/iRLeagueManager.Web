using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Leagues;

internal sealed class LeaguesEndpoint : EndpointBase, ILeaguesEndpoint
{
    public LeaguesEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) : base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Leagues");
    }

    async Task<ClientActionResult<IEnumerable<LeagueModel>>> IGetEndpoint<IEnumerable<LeagueModel>>.Get(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.GetAsClientActionResult<IEnumerable<LeagueModel>>(QueryUrl, cancellationToken);
    }

    async Task<ClientActionResult<LeagueModel>> ILeaguesEndpoint.Post(PostLeagueModel model, CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.PostAsClientActionResult<LeagueModel>(QueryUrl, model, cancellationToken);
    }

    ILeagueByIdEndpoint ILeaguesEndpoint.WithId(long leagueId)
    {
        return new LeagueByIdEndpoint(HttpClientWrapper, RouteBuilder, leagueId);
    }

    ILeagueByNameEndpoint ILeaguesEndpoint.WithName(string leagueName)
    {
        var withNameBuilder = RouteBuilder.Copy();
        withNameBuilder.RemoveLast();
        return new LeagueByNameEndpoint(HttpClientWrapper, withNameBuilder, leagueName);
    }
}
