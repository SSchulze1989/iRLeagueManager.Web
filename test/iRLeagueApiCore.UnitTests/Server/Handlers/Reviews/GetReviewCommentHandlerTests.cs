using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Handlers.Reviews;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Reviews;

public sealed class GetReviewCommentHandlerTests : ReviewsHandlersTestsBase<GetReviewCommentHandler, GetReviewCommentRequest, ReviewCommentModel>
{
    protected override GetReviewCommentHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetReviewCommentRequest> validator)
    {
        return new GetReviewCommentHandler(logger, dbContext, new[] { validator });
    }

    private GetReviewCommentRequest DefaultRequest(long commentId)
    {

        return new GetReviewCommentRequest(commentId);
    }

    protected override GetReviewCommentRequest DefaultRequest()
    {
        return DefaultRequest(TestCommentId);
    }

    protected override void DefaultAssertions(GetReviewCommentRequest request, ReviewCommentModel result, LeagueDbContext dbContext)
    {
        var commentEntity = dbContext.ReviewComments
            .Include(x => x.ReviewCommentVotes)
                .ThenInclude(x => x.MemberAtFault)
            .SingleOrDefault(x => x.CommentId == request.CommentId);
        commentEntity.Should().NotBeNull();
        AssertReviewComment(commentEntity!, result);
        base.DefaultAssertions(request, result, dbContext);
    }

    private void AssertReviewComment(ReviewCommentEntity expected, ReviewCommentModel result)
    {
        result.LeagueId.Should().Be(expected.LeagueId);
        result.CommentId.Should().Be(expected.CommentId);
        result.AuthorName.Should().Be(expected.AuthorName);
        result.AuthorUserId.Should().Be(expected.AuthorUserId);
        result.ReviewId.Should().Be(expected.ReviewId);
        result.Text.Should().Be(expected.Text);
        result.Votes.Should().HaveSameCount(expected.ReviewCommentVotes);
        foreach ((var vote, var expectedVote) in result.Votes.Zip(expected.ReviewCommentVotes))
        {
            AssertCommentVote(expectedVote, vote);
        }
        AssertVersion(expected, result);
    }

    private void AssertCommentVote(ReviewCommentVoteEntity expected, VoteModel result)
    {
        result.Id.Should().Be(expected.ReviewVoteId);
        result.Description.Should().Be(expected.Description);
        AssertMemberInfo(expected.MemberAtFault, result.MemberAtFault);
    }

    private void AssertMemberInfo(MemberEntity? expected, MemberInfoModel? result)
    {

        result?.MemberId.Should().Be(expected?.Id);
        result?.FirstName.Should().Be(expected?.Firstname);
        result?.LastName.Should().Be(expected?.Lastname);
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
    public async Task ShouldHandleNotFoundAsync(long? leagueId, long? commentId)
    {
        leagueId ??= TestLeagueId;
        commentId ??= TestCommentId;
        accessMockHelper.SetCurrentLeague(leagueId.Value);
        var request = DefaultRequest(commentId.Value);
        await HandleNotFoundRequestAsync(request);
    }

    [Fact]
    public override async Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }
}
