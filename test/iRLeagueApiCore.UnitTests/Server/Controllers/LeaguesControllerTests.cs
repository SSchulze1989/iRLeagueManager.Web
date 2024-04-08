using iRLeagueApiCore.Common;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Server.Controllers;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueApiCore.UnitTests.Fixtures;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace iRLeagueApiCore.UnitTests.Server.Controllers;

public sealed class LeaguesControllerTests : DataAccessTestsBase
{
    ILogger<LeaguesController> MockLogger { get; }

    private const string testFullName = "Full Name";
    private const long testLeagueId = 1;

    public LeaguesControllerTests()
    {
        MockLogger = new Mock<ILogger<LeaguesController>>().Object;
    }

    private LeaguesController CreateController(IMediator mediator)
    {
        return AddContexts.AddAdminControllerContext(new LeaguesController(MockLogger, mediator));
    }

    private LeagueModel DefaultGetModel()
    {
        return new LeagueModel()
        {
            Id = testLeagueId,
            Name = testLeagueName,
            NameFull = testFullName,
        };
    }

    private PostLeagueModel DefaultPostModel()
    {
        return new PostLeagueModel()
        {
            Name = testLeagueName,
            NameFull = testFullName
        };
    }

    private PutLeagueModel DefaultPutModel()
    {
        return new PutLeagueModel()
        {
            NameFull = testFullName,
        };
    }

    [Fact]
    public async Task GetAll()
    {
        var expectedResult = new LeagueModel[] { DefaultGetModel() };
        var mediator = MockHelpers.TestMediator<GetLeaguesRequest, IEnumerable<LeagueModel>>(result: expectedResult);
        var controller = CreateController(mediator);
        var result = await controller.GetAll();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResult, okResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetLeaguesRequest>(), default));
    }

    [Fact]
    public async Task Get()
    {
        var expectedResult = DefaultGetModel();
        var mediator = MockHelpers.TestMediator<GetLeagueRequest, LeagueModel>(
            x => x.LeagueId == testLeagueId, expectedResult);
        var controller = CreateController(mediator);
        var result = await controller.Get(testLeagueId);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResult, okResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetLeagueRequest>(), default));
    }

    [Fact]
    public async Task Post()
    {
        var expectedResult = DefaultGetModel();
        var mediator = MockHelpers.TestMediator<PostLeagueRequest, LeagueModel>(
            x => x.Model.Name == testLeagueName, expectedResult);
        var controller = CreateController(mediator);
        var result = await controller.Post(DefaultPostModel());
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(expectedResult, createdResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<PostLeagueRequest>(), default));
    }

    [Fact]
    public async Task Put()
    {
        var expectedResult = DefaultGetModel();
        var mediator = MockHelpers.TestMediator<PutLeagueRequest, LeagueModel>(
            x => x.LeagueId == testLeagueId, expectedResult);
        var controller = CreateController(mediator);
        var result = await controller.Put(testLeagueId, DefaultPutModel());
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResult, okResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<PutLeagueRequest>(), default));
    }

    [Fact]
    public async Task Delete()
    {
        var expectedResult = DefaultGetModel();
        var mediator = MockHelpers.TestMediator<DeleteLeagueRequest, Unit>(
            x => x.LeagueId == testLeagueId);
        var controller = CreateController(mediator);
        var result = await controller.Delete(testLeagueId);
        Assert.IsType<NoContentResult>(result);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<DeleteLeagueRequest>(), default));
    }

    [Theory]
    [InlineData(LeagueRoles.Owner, true)]
    [InlineData(LeagueRoles.Admin, true)]
    [InlineData(LeagueRoles.Organizer, true)]
    [InlineData(LeagueRoles.Steward, false)]
    [InlineData(LeagueRoles.Member, false)]
    [InlineData("", false)]
    public async Task Get_ShouldSetIncludeDetails(string leagueRole, bool includeDetails)
    {
        var testModel = new LeagueModel()
        {
            SubscriptionStatus = includeDetails ? Common.Enums.SubscriptionStatus.FreeTrial : null,
            SubscriptionExpires = includeDetails ? DateTime.UtcNow : null,
        };
        var mediator = MockHelpers.TestMediator<GetLeagueRequest, LeagueModel>(
            x => x.IncludeSubscriptionDetails == includeDetails,
            result: testModel);
        var controller = AddContexts.AddControllerContext(
            new LeaguesController(MockLogger, mediator), 
            new[] { LeagueRoles.GetLeagueRoleName(testLeagueName, leagueRole)! });
        var routeValuesMock = new Mock<IRouteValuesFeature>();
        routeValuesMock.SetupGet(x => x.RouteValues).Returns(new RouteValueDictionary(new Dictionary<string, object>() { { "leagueName", testLeagueName } }));
        controller.HttpContext.Features.Set(routeValuesMock.Object);
        var result = (await controller.Get(testLeagueId)).Result.Should().BeOfType<OkObjectResult>().Subject;
        var resultModel = result!.Value.Should().BeOfType<LeagueModel>().Subject;
        result.Value.Should().NotBeNull();
    }

    [Theory]
    [InlineData(LeagueRoles.Owner, true)]
    [InlineData(LeagueRoles.Admin, true)]
    [InlineData(LeagueRoles.Organizer, true)]
    [InlineData(LeagueRoles.Steward, false)]
    [InlineData(LeagueRoles.Member, false)]
    [InlineData("", false)]
    public async Task GetByName_ShouldSetIncludeDetails(string leagueRole, bool includeDetails)
    {
        var testModel = new LeagueModel()
        {
            SubscriptionStatus = includeDetails ? Common.Enums.SubscriptionStatus.FreeTrial : null,
            SubscriptionExpires = includeDetails ? DateTime.UtcNow : null,
        };
        var mediator = MockHelpers.TestMediator<GetLeagueByNameRequest, LeagueModel>(
            x => x.IncludeSubscriptionDetails == includeDetails,
            result: testModel);
        var controller = AddContexts.AddControllerContext(
            new LeaguesController(MockLogger, mediator),
            new[] { LeagueRoles.GetLeagueRoleName(testLeagueName, leagueRole)! });
        var routeValuesMock = new Mock<IRouteValuesFeature>();
        routeValuesMock.SetupGet(x => x.RouteValues).Returns(new RouteValueDictionary(new Dictionary<string, object>() { { "leagueName", testLeagueName } }));
        controller.HttpContext.Features.Set(routeValuesMock.Object);
        var result = (await controller.GetByName(testLeagueName)).Result.Should().BeOfType<OkObjectResult>().Subject;
        result.Value.Should().NotBeNull();
    }
}
