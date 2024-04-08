using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Handlers.Reviews;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Reviews;

public sealed class GetReviewHandlerTests : ReviewsHandlersTestsBase<GetReviewHandler, GetReviewRequest, ReviewModel>
{
    protected override GetReviewHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetReviewRequest> validator)
    {
        return new GetReviewHandler(logger, dbContext, new[] { validator });
    }

    private GetReviewRequest DefaultRequest(long reviewId, bool includeComments = true)
    {
        return new GetReviewRequest(reviewId, includeComments);
    }

    protected override GetReviewRequest DefaultRequest()
    {
        return DefaultRequest(TestReviewId);
    }

    protected override void DefaultAssertions(GetReviewRequest request, ReviewModel result, LeagueDbContext dbContext)
    {
        result.ReviewId.Should().Be(request.ReviewId);
        var reviewEntity = dbContext.IncidentReviews
            .Include(x => x.Session)
                .ThenInclude(x => x.Event)
                    .ThenInclude(x => x.Schedule)
            .Include(x => x.Comments)
                .ThenInclude(x => x.ReviewCommentVotes)
                    .ThenInclude(x => x.MemberAtFault)
            .SingleOrDefault(x => x.ReviewId == request.ReviewId);
        reviewEntity.Should().NotBeNull();
        result.SeasonId.Should().Be(reviewEntity!.Session.Event.Schedule.SeasonId);
        result.ReviewComments.Should().HaveSameCount(reviewEntity.Comments);
        foreach ((var comment, var expected) in result.ReviewComments.Zip(reviewEntity.Comments))
        {
            AssertReviewComment(expected, comment);
        }
        AssertVersion(reviewEntity, result);
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
        AssertMemberInfo(expected.MemberAtFault, result.MemberAtFault!);
    }

    private void AssertMemberInfo(MemberEntity? expected, MemberInfoModel? result)
    {
        result?.MemberId.Should().Be(expected?.Id);
        result?.FirstName.Should().Be(expected?.Firstname);
        result?.LastName.Should().Be(expected?.Lastname);
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

    [Fact]
    public async Task ShouldNotIncludeCommentsWhenFalse()
    {
        var request = DefaultRequest(TestReviewId, false);
        await HandleSpecialAsync(request, (request, model, context) =>
        {
            model.ReviewId.Should().Be(TestReviewId);
            model.VoteResults.Should().NotBeEmpty();
            model.ReviewComments.Should().BeEmpty();
        });
    }
}
