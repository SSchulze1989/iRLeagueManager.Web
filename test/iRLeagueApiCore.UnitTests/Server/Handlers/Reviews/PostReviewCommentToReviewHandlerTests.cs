using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Handlers.Reviews;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Reviews;

public sealed class PostReviewCommentToReviewHandlerTests : ReviewsHandlersTestsBase<PostReviewCommentToReviewHandler, PostReviewCommentToReviewRequest, ReviewCommentModel>
{
    private PutReviewCommentModel TestReviewComment => new PutReviewCommentModel()
    {
        Text = "Test Comment",
        Votes = new[] { new VoteModel()
            {
                Description = "Test Vote",
                MemberAtFault = new MemberInfoModel() { MemberId = TestMemberId},
            } },
    };

    protected override PostReviewCommentToReviewHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PostReviewCommentToReviewRequest> validator)
    {
        return new PostReviewCommentToReviewHandler(logger, dbContext, new[] { validator });
    }

    private PostReviewCommentToReviewRequest DefaultRequest(long reviewId)
    {

        return new PostReviewCommentToReviewRequest(reviewId, DefaultUser(), TestReviewComment);
    }

    protected override PostReviewCommentToReviewRequest DefaultRequest()
    {
        return DefaultRequest(TestReviewId);
    }

    protected override void DefaultAssertions(PostReviewCommentToReviewRequest request, ReviewCommentModel result, LeagueDbContext dbContext)
    {
        var expected = request.Model;
        result.LeagueId.Should().Be(result.LeagueId);
        result.ReviewId.Should().NotBe(0);
        result.Text.Should().Be(expected.Text);
        result.Votes.Should().HaveSameCount(expected.Votes);
        foreach ((var vote, var expectedVote) in result.Votes.Zip(expected.Votes))
        {
            AssertCommentVote(expectedVote, vote);
        }
        AssertCreated(request.User, DateTime.UtcNow, result);
        base.DefaultAssertions(request, result, dbContext);
    }

    private void AssertCommentVote(VoteModel expected, VoteModel result)
    {
        result.Description.Should().Be(expected.Description);
        AssertMemberInfo(expected.MemberAtFault, result.MemberAtFault);
    }

    private void AssertMemberInfo(MemberInfoModel? expected, MemberInfoModel? result)
    {
        result?.MemberId.Should().Be(expected?.MemberId);
    }

    [Fact]
    public override async Task<ReviewCommentModel> ShouldHandleDefault()
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
