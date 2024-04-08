using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Controllers;
using iRLeagueApiCore.Server.Exceptions;
using iRLeagueApiCore.Server.Handlers.Scorings;
using iRLeagueApiCore.UnitTests.Fixtures;
using MediatR;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iRLeagueApiCore.UnitTests.Server.Controllers;

public sealed class ScoringControllerTests
{
    private readonly ILogger<ScoringsController> logger;

    const string testLeagueName = "TestLeague";
    const long testLeagueId = 1;
    const long testScoringId = 1;

    public ScoringControllerTests()
    {
        logger = Mock.Of<ILogger<ScoringsController>>();
    }

    private static ScoringModel DefaultGetModel()
    {
        return new ScoringModel()
        {
            Id = testScoringId,
            LeagueId = testLeagueId,
        };
    }

    private static PutScoringModel DefaultPutModel()
    {
        return new PutScoringModel();
    }

    [Fact]
    public async Task GetScoringsValid()
    {
        var expectedResult = new ScoringModel[] { DefaultGetModel() };
        var mediator = MockHelpers.TestMediator<GetScoringsRequest, IEnumerable<ScoringModel>>(result: expectedResult);
        var controller = AddContexts.AddMemberControllerContext(new ScoringsController(logger, mediator));

        var result = await controller.Get(testLeagueName);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResult, okResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetScoringsRequest>(), default));
    }

    [Fact]
    public async Task GetScoringValid()
    {
        var expectedResult = DefaultGetModel();
        var mediator = MockHelpers.TestMediator<GetScoringRequest, ScoringModel>(x =>
            x.ScoringId == testScoringId, expectedResult);
        var controller = AddContexts.AddMemberControllerContext(new ScoringsController(logger, mediator));

        var result = await controller.Get(testLeagueName, testScoringId);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResult, okResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetScoringRequest>(), default));
    }

    [Fact]
    public async Task PutScoringValid()
    {
        var expectedResult = DefaultGetModel();
        var mediator = MockHelpers.TestMediator<PutScoringRequest, ScoringModel>(x =>
            x.ScoringId == testScoringId, expectedResult);
        var controller = AddContexts.AddMemberControllerContext(new ScoringsController(logger, mediator));

        var result = await controller.Put(testLeagueName, testScoringId, DefaultPutModel());
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResult, okResult.Value);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<PutScoringRequest>(), default));
    }

    [Fact]
    public async Task DeleteScoringValid()
    {
        var mediator = MockHelpers.TestMediator<DeleteScoringRequest, Unit>(x =>
            x.ScoringId == testScoringId);
        var controller = AddContexts.AddMemberControllerContext(new ScoringsController(logger, mediator));

        var result = await controller.Delete(testLeagueName, testScoringId);
        Assert.IsType<NoContentResult>(result);
        var mediatorMock = Mock.Get(mediator);
        mediatorMock.Verify(x => x.Send(It.IsAny<DeleteScoringRequest>(), default));
    }
}
