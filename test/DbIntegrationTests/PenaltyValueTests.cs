using FluentAssertions;
using iRLeagueApiCore.Common.Enums;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace DbIntegrationTests;
public sealed class PenaltyValueTests : DatabaseTestBase
{
    [Fact]
    public async Task ShouldStorePointsPenaltyData()
    {
        //using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var resultRow = await DbContext.ScoredResultRows.FirstAsync();
        var addPenalty = new AddPenaltyEntity()
        {
            LeagueId = resultRow.LeagueId,
            ScoredResultRow = resultRow,
            Value = new() { Type = PenaltyType.Points, Points = 10 }
        };
        resultRow.AddPenalties.Add(addPenalty);
        await DbContext.SaveChangesAsync();

        var test = await DbContext.AddPenaltys.FirstAsync(x => x.AddPenaltyId == addPenalty.AddPenaltyId);
        test.Value.Type.Should().Be(addPenalty.Value.Type);
        test.Value.Points.Should().Be(addPenalty.Value.Points);
    }

    [Fact]
    public async Task ShouldStorePositionsPenaltyData()
    {
        //using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var resultRow = await DbContext.ScoredResultRows.FirstAsync();
        var addPenalty = new AddPenaltyEntity()
        {
            LeagueId = resultRow.LeagueId,
            ScoredResultRow = resultRow,
            Value = new() { Type = PenaltyType.Position, Positions = 1 }
        };
        resultRow.AddPenalties.Add(addPenalty);
        await DbContext.SaveChangesAsync();

        var test = await DbContext.AddPenaltys.FirstAsync(x => x.AddPenaltyId == addPenalty.AddPenaltyId);
        test.Value.Type.Should().Be(addPenalty.Value.Type);
        test.Value.Positions.Should().Be(addPenalty.Value.Positions);
    }

    [Fact]
    public async Task ShouldStoreTimePenaltyData()
    {
        //using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        AddPenaltyEntity addPenalty;
        using (var dbContext = GetTestDatabaseContext())
        {
            var resultRow = await dbContext.ScoredResultRows.FirstAsync();
            addPenalty = new AddPenaltyEntity()
            {
                LeagueId = resultRow.LeagueId,
                ScoredResultRow = resultRow,
                Value = new() { Type = PenaltyType.Time, Time = TimeSpan.FromSeconds(10) }
            };
            resultRow.AddPenalties.Add(addPenalty);
            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = GetTestDatabaseContext())
        {
            var test = await dbContext.AddPenaltys.FirstAsync(x => x.AddPenaltyId == addPenalty.AddPenaltyId);
            test.Value.Type.Should().Be(addPenalty.Value.Type);
            test.Value.Time.Should().Be(addPenalty.Value.Time);
        }
    }

    [Fact]
    public async Task ShouldBeAbleToHaveMultipleReviewPenalties()
    {
        var resultRow = await DbContext.ScoredResultRows.FirstAsync();
        var review = await DbContext.IncidentReviews.FirstAsync();
        var vote1 = new AcceptedReviewVoteEntity() { LeagueId = review.LeagueId, Review = review };
        var vote2 = new AcceptedReviewVoteEntity() { LeagueId = review.LeagueId, Review = review };
        var penalty1 = new ReviewPenaltyEntity() { LeagueId = review.LeagueId, Review = review, ResultRow = resultRow, ReviewVote = vote1 };
        var penalty2 = new ReviewPenaltyEntity() { LeagueId = review.LeagueId, Review = review, ResultRow = resultRow, ReviewVote = vote2 };
        DbContext.ReviewPenaltys.Add(penalty1);
        DbContext.ReviewPenaltys.Add(penalty2);
        await DbContext.SaveChangesAsync();

        var test = await DbContext.ReviewPenaltys
            .Where(x => x.ResultRowId == resultRow.ScoredResultRowId && x.ReviewId == review.ReviewId)
            .ToListAsync();
        test.Should().Satisfy(x => x.ReviewVoteId == vote1.ReviewVoteId, x => x.ReviewVoteId == vote2.ReviewVoteId);
    }
}
