using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Reviews;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Steward)]
[Route("{leagueName}/[controller]")]
public sealed class ReviewCommentsController : LeagueApiController<ReviewCommentsController>
{
    public ReviewCommentsController(ILogger<ReviewCommentsController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet]
    [Route("{id:long}")]
    public async Task<ActionResult<ReviewCommentModel>> Get([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new GetReviewCommentRequest(id);
        var getReviewComment = await mediator.Send(request, cancellationToken);
        return Ok(getReviewComment);
    }

    /// <summary>
    /// Post new Comment to an existing review
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="reviewId"></param>
    /// <param name="postComment"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("/{leagueName}/Reviews/{reviewId:long}/[controller]")]
    public async Task<ActionResult<ReviewCommentModel>> PostToReview([FromRoute] string leagueName, [FromRoute] long reviewId,
        [FromBody] PostReviewCommentModel postComment, CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostReviewCommentToReviewRequest(reviewId, leagueUser, postComment);
        var getComment = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getComment.CommentId }, getComment);
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<ActionResult<ReviewCommentModel>> Put([FromRoute] string leagueName, [FromRoute] long id,
        PutReviewCommentModel putReviewComment, CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutReviewCommentRequest(id, leagueUser, putReviewComment);
        var getReviewComment = await mediator.Send(request, cancellationToken);
        return Ok(getReviewComment);
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new DeleteReviewCommentRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
