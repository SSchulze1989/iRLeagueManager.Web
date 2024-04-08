using iRLeagueApiCore.Client.Endpoints.Leagues;

namespace iRLeagueApiCore.UnitTests.Client.Endpoints;
public sealed class ResultsEndpointsTests
{
    public ResultsEndpointsTests()
    {
    }

    [Fact]
    public async Task FetchEndpoint_ShouldFetchFromCorrectRoute()
    {
        string leagueName = "TestLeague";
        long eventId = 123;
        int subSessionId = 12345;
        var shouldRequestUrl = EndpointsTests.BaseUrl + leagueName + "/Events/" + eventId + "/Results/Fetch/" + subSessionId;
        await EndpointsTests.TestRequest<ILeaguesEndpoint>(shouldRequestUrl, 
            x => new LeaguesEndpoint(x, new()),
            x => x.WithName(leagueName)
                .Events()
                .WithId(eventId)
                .Results()
                .Fetch()
                .FromIracingSubSession(subSessionId)
                .Post(),
            HttpMethod.Post);
    }
}
