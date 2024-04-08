using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Results;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Results;
public sealed class GetLatestResultHandlerTests : ResultHandlersTestsBase<GetLatestResultHandler, GetLatestResultRequest, IEnumerable<EventResultModel>>
{
    protected override GetLatestResultHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetLatestResultRequest> validator)
    {
        return new(logger, dbContext, new[] { validator });
    }

    protected override GetLatestResultRequest DefaultRequest()
    {
        return new();
    }

    [Fact]
    public async Task ShouldReturnLatestEventWithResult()
    {
        var eventWithResult = await dbContext.Events.FirstAsync();
        var request = DefaultRequest();
        var sut = CreateSut();

        var test = await sut.Handle(request, default);

        test.Should().NotBeEmpty();
        test.Should().OnlyContain(x => x.EventId == eventWithResult.EventId);
    }

    [Fact]
    public async Task ShouldReturnLatestEventWithResult_EvenWhenPreviousEventHasNoResult()
    {
        var eventWithResult = await dbContext.Events.OrderBy(x => x.Date).LastAsync();
        await CreateScoredEventResults(eventWithResult);
        var request = DefaultRequest();
        var sut = CreateSut();

        var test = await sut.Handle(request, default);

        test.Should().NotBeEmpty();
        test.Should().OnlyContain(x => x.EventId == eventWithResult.EventId);
    }

    [Fact]
    public async Task ShouldReturnEmptyResult_WhenNoResultExists()
    {
        foreach(var scoredResult in dbContext.ScoredEventResults)
        {
            dbContext.Remove(scoredResult);
        }
        await dbContext.SaveChangesAsync();
        var request = DefaultRequest();
        var sut = CreateSut();

        var test = await sut.Handle(request, default);

        test.Should().BeEmpty();
    }

    private GetLatestResultHandler CreateSut()
    {
        return CreateTestHandler(dbContext, MockHelpers.TestValidator<GetLatestResultRequest>());
    }
}
