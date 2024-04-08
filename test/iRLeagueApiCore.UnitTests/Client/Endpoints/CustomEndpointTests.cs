using iRLeagueApiCore.Client.Endpoints;

namespace iRLeagueApiCore.UnitTests.Client.Endpoints;

public sealed class CustomEndpointTests
{
    [Fact]
    public async Task ShouldCallCorrectRoute()
    {
        var route = "CustomEndpoint/parameter?queryParam=1";
        var expected = $"{EndpointsTests.BaseUrl}{route}";

        await EndpointsTests.TestRequestUrl<ICustomEndpoint<object>>(expected, x => new CustomEndpoint<object>(x, new(), route), x => x.Get());
    }
}
