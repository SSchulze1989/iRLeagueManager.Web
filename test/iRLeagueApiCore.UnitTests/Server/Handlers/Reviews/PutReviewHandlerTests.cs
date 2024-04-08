using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Handlers.Reviews;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Reviews;

public sealed class PutReviewHandlerTests : ReviewsHandlersTestsBase<PutReviewHandler, PutReviewRequest, ReviewModel>
{
    public PutReviewModel TestReviewModel => new PutReviewModel()
    {
        FullDescription = "Full Description",
        Corner = "1",
        OnLap = "2",
        IncidentNr = "3.4",
        IncidentKind = "Unfall",
        TimeStamp = System.TimeSpan.FromMinutes(1.2),
        InvolvedMembers = new MemberInfoModel[]
        {
                new MemberInfoModel() { MemberId = TestMemberId },
                new MemberInfoModel() { MemberId = TestMemberId2 },
        },
        VoteResults = new[]
        {
                new VoteModel() {
                    Id = TestReviewVoteId,
                    VoteCategoryId = TestVoteCategory,
                    Description = "Vote Description",
                    MemberAtFault = new MemberInfoModel() { MemberId = TestMemberId }
                },
            },
    };

    protected override PutReviewHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PutReviewRequest> validator)
    {
        return new PutReviewHandler(logger, dbContext, new[] { validator });
    }

    private PutReviewRequest DefaultRequest(long reviewId)
    {

        return new PutReviewRequest(reviewId, DefaultUser(), TestReviewModel);
    }

    protected override PutReviewRequest DefaultRequest()
    {
        return DefaultRequest(TestReviewId);
    }

    protected override void DefaultAssertions(PutReviewRequest request, ReviewModel result, LeagueDbContext dbContext)
    {
        var expected = request.Model;
        result.ReviewId.Should().Be(request.ReviewId);
        result.Corner.Should().Be(expected.Corner);
        result.OnLap.Should().Be(expected.OnLap);
        result.IncidentKind.Should().Be(expected.IncidentKind);
        result.IncidentNr.Should().Be(expected.IncidentNr);
        result.TimeStamp.Should().Be(expected.TimeStamp);
        result.InvolvedMembers.Should().HaveSameCount(expected.InvolvedMembers);
        foreach ((var member, var expectedMember) in result.InvolvedMembers.OrderBy(x => x.MemberId).Zip(expected.InvolvedMembers.OrderBy(x => x.MemberId)))
        {
            AssertMemberInfo(expectedMember, member);
        }
        result.VoteResults.Should().HaveSameCount(expected.VoteResults);
        foreach ((var vote, var expectedVote) in result.VoteResults.Zip(expected.VoteResults))
        {
            AssertVote(expectedVote, vote);
        }
        AssertChanged(request.User, DateTime.UtcNow, result);
        base.DefaultAssertions(request, result, dbContext);
    }

    private void AssertMemberInfo(MemberInfoModel? expected, MemberInfoModel? result)
    {
        result?.MemberId.Should().Be(expected?.MemberId);
    }

    private void AssertVote(VoteModel expected, VoteModel result)
    {
        if (expected.Id != default)
        {
            result.Id.Should().Be(expected.Id);
        }
        AssertMemberInfo(result.MemberAtFault, expected.MemberAtFault);
        result.Description.Should().Be(expected.Description);
        result.VoteCategoryId.Should().Be(expected.VoteCategoryId);
    }

    [Fact]
    public override async Task<ReviewModel> ShouldHandleDefault()
    {
        return await base.ShouldHandleDefault();
    }

    [Theory]
    [InlineData(0, defaultId)]
    [InlineData(defaultId, 0)]
    [InlineData(-42, defaultId)]
    [InlineData(defaultId, -42)]
    public async Task ShouldHandleNotFoundAsync(long? leagueId, long? reviewId)
    {
        leagueId ??= TestLeagueId;
        reviewId ??= TestReviewId;
        accessMockHelper.SetCurrentLeague(leagueId.Value);
        var request = DefaultRequest(reviewId.Value);
        await HandleNotFoundRequestAsync(request);
    }

    [Fact]
    public override async Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }
}
