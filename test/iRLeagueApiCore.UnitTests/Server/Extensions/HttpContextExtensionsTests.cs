using FluentIdentityBuilder;
using iRLeagueApiCore.Common;
using iRLeagueApiCore.Server.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

namespace iRLeagueApiCore.UnitTests.Server.Extensions;
public sealed class HttpContextExtensionsTests
{
    private readonly ClaimsPrincipalBuilder principalBuilder = new();

    [Fact]
    public void GetLeagueUser_ShouldReturnLeagueUser_WhenUserAndLeaguenNmeIsSet()
    {
        var httpContext = TestHttpContext();
        httpContext.GetRouteData().Values.Add("leagueName", "Testleague");
        httpContext.User = principalBuilder.StartBuild()
            .WithIdentifier("TestId")
            .WithName("Testuser")
            .WithRole(LeagueRoles.GetLeagueRoleName("Testleague", LeagueRoles.Admin))
            .Create();

        var leagueUser = httpContext.GetLeagueUser();

        leagueUser.Should().NotBeNull();
        leagueUser!.Id.Should().Be("TestId");
        leagueUser.Name.Should().Be("Testuser");
        leagueUser.Roles.Should().Contain(x => x == LeagueRoles.Admin);
    }

    [Fact]
    public void GetLeagueUser_ShouldReturnUserWithoutRoles_WhenLeagueNameIsNotSet()
    {
        var httpContext = TestHttpContext();
        httpContext.User = principalBuilder.StartBuild()
            .WithRole(LeagueRoles.GetLeagueRoleName("Testleague", LeagueRoles.Organizer))
            .Create();

        var leagueUser = httpContext.GetLeagueUser();
        
        leagueUser.Should().NotBeNull();
        leagueUser.Roles.Should().BeEmpty();
    }

    [Fact]
    public void GetLeagueUser_ShouldReturnAdminUserAsAdmin_WhenLeagueNameIsNotSet()
    {
        var httpContext = TestHttpContext();
        httpContext.User = principalBuilder.StartBuild()
            .WithRole("Admin")
            .Create();

        var leagueUser = httpContext.GetLeagueUser();

        leagueUser.Should().NotBeNull();
        leagueUser.Roles.Should().Contain(x => x == "Admin");
    }

    private static HttpContext TestHttpContext(RouteData? routeData = default)
    {
        routeData ??= new RouteData();
        var context = new Mock<HttpContext>();
        var features = new Mock<IFeatureCollection>();
        var iroutingFeature = new Mock<IRoutingFeature>();
        iroutingFeature.SetupProperty(x => x.RouteData, routeData);
        var irouteValueFeature = new Mock<IRouteValuesFeature>();
        irouteValueFeature.SetupProperty(x => x.RouteValues, routeData.Values);
        features.Setup(x => x.Get<IRoutingFeature>()).Returns(iroutingFeature.Object);
        features.Setup(x => x.Get<IRouteValuesFeature>()).Returns(irouteValueFeature.Object);
        context.SetupProperty(x => x.User);
        context.SetupGet(x => x.Features).Returns(features.Object);
        return context.Object;
    }
}
