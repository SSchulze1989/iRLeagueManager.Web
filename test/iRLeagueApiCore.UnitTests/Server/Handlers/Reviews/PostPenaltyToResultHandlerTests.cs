using FluentValidation;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Reviews;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Reviews;
public sealed class PostPenaltyToResultHandlerTests : ReviewsHandlersTestsBase<PostPenaltyToResultHandler, PostPenaltyToResultRequest, PenaltyModel>
{
    protected override PostPenaltyToResultHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PostPenaltyToResultRequest> validator)
    {
        return new(logger, dbContext, new[] { validator });
    }

    protected override PostPenaltyToResultRequest DefaultRequest()
    {
        throw new NotImplementedException();
    }

    [Theory]
    [InlineData(PenaltyType.Position, 42, 0, 0)]
    [InlineData(PenaltyType.Points, 0, 42, 0)]
    [InlineData(PenaltyType.Time, 0, 0, 42)]
    public async Task ShouldAddNewPenalty(PenaltyType type, int positions, int points, int seconds)
    {
        var result = await dbContext.ScoredSessionResults
            .Include(x => x.ScoredResultRows)
                 .ThenInclude(x => x.Member)
            .FirstAsync();
        var row = result.ScoredResultRows.First();
        var request = new PostPenaltyToResultRequest(result.SessionResultId, row.ScoredResultRowId, new()
        {
            Reason = "Test Penalty",
            Type = type,
            Points = points,
            Positions = positions,
            Time = TimeSpan.FromSeconds(seconds),
        });
        var sut = CreateSut();

        var test = await sut.Handle(request, default);

        test.Reason.Should().Be(request.Model.Reason);
        test.Type.Should().Be(type);
        test.Points.Should().Be(points);
        test.Positions.Should().Be(positions);
        test.Time.Should().Be(TimeSpan.FromSeconds(seconds));
    }

    private PostPenaltyToResultHandler CreateSut()
    {
        return CreateTestHandler(dbContext, MockHelpers.TestValidator<PostPenaltyToResultRequest>());
    }
}
