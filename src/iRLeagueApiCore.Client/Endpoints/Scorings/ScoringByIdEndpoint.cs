using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Scorings;

internal class ScoringByIdEndpoint : UpdateEndpoint<ScoringModel, PutScoringModel>, IScoringByIdEndpoint
{
    public ScoringByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long scoringId) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddParameter(scoringId);
    }

    IPostEndpoint<NoContent, NoContent> IScoringByIdEndpoint.AddSession(long sessionId)
    {
        var addSessionBuilder = RouteBuilder.Copy();
        addSessionBuilder.AddEndpoint("AddSession");
        addSessionBuilder.AddParameter(sessionId);
        return new PostEndpoint<NoContent, NoContent>(HttpClientWrapper, addSessionBuilder);
    }

    IPostEndpoint<NoContent, NoContent> IScoringByIdEndpoint.RemoveSession(long sessionId)
    {
        var removeSessionBuilder = RouteBuilder.Copy();
        removeSessionBuilder.AddEndpoint("RemoveSession");
        removeSessionBuilder.AddParameter(sessionId);
        return new PostEndpoint<NoContent, NoContent>(HttpClientWrapper, removeSessionBuilder);
    }
}
