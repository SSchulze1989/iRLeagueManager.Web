using iRLeagueApiCore.Client.Endpoints.Leagues;
using iRLeagueApiCore.Client.QueryBuilder;

namespace iRLeagueApiCore.UnitTests.Client.Endpoints;

public sealed class LeaguesEndpointTests
{
    private const string testLeagueName = "testLeague";
    private const long testLeagueId = 1;

    [Fact]
    public async Task ShouldCallCorrectRequestUrlLeagues()
    {
        var shouldRequestUrl = EndpointsTests.BaseUrl + "Leagues";
        await EndpointsTests.TestRequestUrl<ILeaguesEndpoint>(shouldRequestUrl, x => new LeaguesEndpoint(x, new RouteBuilder()), x => x.Get());
    }

    [Fact]
    public async Task ShouldCallCorrectRequestUrlWithId()
    {
        var requestUrl = EndpointsTests.BaseUrl + $"Leagues/{testLeagueId}";
        await EndpointsTests.TestRequestUrl<ILeaguesEndpoint>(requestUrl,
            x => new LeaguesEndpoint(x, new RouteBuilder()),
            x => x.WithId(1).Get());
    }

    [Fact]
    public async Task ShouldCallCorrectRequestUrlWithName()
    {
        var requestUrl = EndpointsTests.BaseUrl + $"{testLeagueName}/Seasons";
        await EndpointsTests.TestRequestUrl<ILeaguesEndpoint>(requestUrl,
            x => new LeaguesEndpoint(x, new RouteBuilder()),
            x => x.WithName(testLeagueName).Seasons().Get());
    }
}
