using FluentValidation;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Reviews;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Reviews;
public sealed class GetPenaltiesFromSessionResultHandlerTests : ReviewsHandlersTestsBase<GetPenaltiesFromSessionResultHandler, GetPenaltiesFromSessionResultRequest, IEnumerable<PenaltyModel>>
{
    protected override GetPenaltiesFromSessionResultHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetPenaltiesFromSessionResultRequest> validator)
    {
        return new(logger, dbContext, new[] { validator });
    }

    protected override GetPenaltiesFromSessionResultRequest DefaultRequest()
    {
        throw new NotImplementedException();
    }

    [Theory]
    [InlineData(PenaltyType.Points, 42, 0, 0)]
    [InlineData(PenaltyType.Position, 0, 42, 0)]
    [InlineData(PenaltyType.Time, 0, 0, 42)]
    public async Task ShouldReturnPenaltyData(PenaltyType type, int points, int positions, int seconds)
    {
        var result = await dbContext.ScoredSessionResults
            .Include(x => x.ScoredResultRows)
            .FirstAsync();
        var vote = await dbContext.AcceptedReviewVotes
            .Include(x => x.Review)
            .FirstAsync();
        var reviewPenalty = new ReviewPenaltyEntity()
        {
            LeagueId = vote.LeagueId,
            ResultRow = result.ScoredResultRows.First(),
            Review = vote.Review,
            ReviewVote = vote,
            Value = new()
            {
                Type = type,
                Points = points,
                Positions = positions,
                Time = TimeSpan.FromSeconds(seconds)
            },
        };
        var addPenalty = new AddPenaltyEntity()
        {
            LeagueId = result.LeagueId,
            ScoredResultRow = result.ScoredResultRows.First(),
            Reason = "Test Result",
            Value = new()
            {
                Type = type,
                Points = points,
                Positions = positions,
                Time = TimeSpan.FromSeconds(seconds)
            },
        };
        dbContext.ReviewPenaltys.Add(reviewPenalty);
        dbContext.AddPenaltys.Add(addPenalty);
        await dbContext.SaveChangesAsync();
        var request = new GetPenaltiesFromSessionResultRequest(result.SessionResultId);
        var sut = CreateSut();

        var test = await sut.Handle(request, default);

        test.Should().Satisfy(
            x =>
                x.ResultRowId == reviewPenalty.ResultRowId &&
                x.ReviewId == reviewPenalty.ReviewId &&
                x.ReviewVoteId == reviewPenalty.ReviewVoteId &&
                x.Type == type &&
                x.Points == points &&
                x.Positions == positions &&
                x.Time == TimeSpan.FromSeconds(seconds),
            x =>
                x.AddPenaltyId == addPenalty.AddPenaltyId &&
                x.Reason == addPenalty.Reason &&
                x.Type == type &&
                x.Points == points &&
                x.Positions == positions &&
                x.Time == TimeSpan.FromSeconds(seconds)
            );
    }

    private GetPenaltiesFromSessionResultHandler CreateSut()
    {
        return CreateTestHandler(dbContext, MockHelpers.TestValidator<GetPenaltiesFromSessionResultRequest>());
    }
}
