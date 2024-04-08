using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Controllers;
using iRLeagueApiCore.Server.Exceptions;
using iRLeagueApiCore.Server.Handlers.Seasons;
using iRLeagueApiCore.UnitTests.Fixtures;
using MediatR;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iRLeagueApiCore.UnitTests.Server.Controllers;

public sealed class SeasonDbTestFixture
{
    private readonly ILogger<SeasonsController> logger;

    const string testLeagueName = "TestLeague";
    const long testLeagueId = 1;
    const long testSeasonId = 1;
    const string testSeasonName = "Season 1";

    public SeasonDbTestFixture()
    {
        logger = Mock.Of<ILogger<SeasonsController>>();
    }

    private static SeasonModel DefaultGetModel()
    {
        return new SeasonModel()
        {
            LeagueId = testLeagueId,
            SeasonId = testSeasonId,
            SeasonName = testSeasonName
        };
    }

    private static PostSeasonModel DefaultPostModel()
    {
        return new PostSeasonModel();
    }

    private static PutSeasonModel DefaultPutModel()
    {
        return new PutSeasonModel();
    }

    private static ResourceNotFoundException NotFound()
    {
        return new ResourceNotFoundException();
    }

    private static ValidationException ValidationFailed()
    {
        return new ValidationException("Test validation failed");
    }

    [Fact]
    public async Task GetSeasonsValid()
    {
        var expectedResult = new SeasonModel[] { DefaultGetModel() };
        var mediator = MockHelpers.TestMediator<GetSeasonsRequest, IEnumerable<SeasonModel>>(result: expectedResult);
        var controller = AddContexts.AddMemberControllerContext(new SeasonsController(logger, mediator));

        var result = await controller.GetAll(testLeagueName);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResult, okResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetSeasonsRequest>(), default));
    }

    [Fact]
    public async Task GetSeasonValid()
    {
        var expectedResult = DefaultGetModel();
        var mediator = MockHelpers.TestMediator<GetSeasonRequest, SeasonModel>(x =>
            x.SeasonId == testSeasonId, expectedResult);
        var controller = AddContexts.AddMemberControllerContext(new SeasonsController(logger, mediator));

        var result = await controller.Get(testLeagueName, testSeasonId);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResult, okResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetSeasonRequest>(), default));
    }

    [Fact]
    public async Task PostSeasonValid()
    {
        var expectedResult = DefaultGetModel();
        var mediator = MockHelpers.TestMediator<PostSeasonRequest, SeasonModel>(result: expectedResult);
        var controller = AddContexts.AddMemberControllerContext(new SeasonsController(logger, mediator));

        var result = await controller.Post(testLeagueName, DefaultPostModel());
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(expectedResult, createdResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<PostSeasonRequest>(), default));
    }

    [Fact]
    public async Task PutSeasonValid()
    {
        var expectedResult = DefaultGetModel();
        var mediator = MockHelpers.TestMediator<PutSeasonRequest, SeasonModel>(x =>
            x.SeasonId == testSeasonId, expectedResult);
        var controller = AddContexts.AddMemberControllerContext(new SeasonsController(logger, mediator));

        var result = await controller.Put(testLeagueName, testSeasonId, DefaultPutModel());
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResult, okResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<PutSeasonRequest>(), default));
    }

    [Fact]
    public async Task DeleteSeasonValid()
    {
        var mediator = MockHelpers.TestMediator<DeleteSeasonRequest, Unit>(x =>
            x.SeasonId == testSeasonId);
        var controller = AddContexts.AddMemberControllerContext(new SeasonsController(logger, mediator));

        var result = await controller.Delete(testLeagueName, testSeasonId);
        Assert.IsType<NoContentResult>(result);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<DeleteSeasonRequest>(), default));
    }
}
