using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Reviews;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Steward)]
[Route("{leagueName}/[controller]")]
public sealed class ReviewsController : LeagueApiController<ReviewsController>
{
    public ReviewsController(ILogger<ReviewsController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    private static bool IncludeComments(LeagueUser user)
    {
        return user.IsInRole(LeagueRoles.Admin, LeagueRoles.Steward);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{id:long}")]
    public async Task<ActionResult<ReviewModel>> Get([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var includeComments = IncludeComments(new LeagueUser(leagueName, User));
        var request = new GetReviewRequest(id, includeComments);
        var getReview = await mediator.Send(request, cancellationToken);
        return Ok(getReview);
    }

    [HttpPost]
    [Route("/{leagueName}/Sessions/{sessionId:long}/[controller]")]
    public async Task<ActionResult<ReviewModel>> Post([FromRoute] string leagueName, [FromRoute] long sessionId,
        PostReviewModel postReview, CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostReviewToSessionRequest(sessionId, leagueUser, postReview);
        var getReview = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getReview.ReviewId }, getReview);
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<ActionResult<ReviewModel>> Put([FromRoute] string leagueName, [FromRoute] long id,
        PutReviewModel putReview, CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutReviewRequest(id, leagueUser, putReview);
        var getReview = await mediator.Send(request, cancellationToken);
        return Ok(getReview);
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new DeleteReviewRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Sessions/{sessionId:long}/[controller]")]
    public async Task<ActionResult<IEnumerable<ReviewModel>>> GetFromSession([FromRoute] string leagueName, [FromRoute] long sessionId, 
        CancellationToken cancellationToken)
    {
        var includeComments = IncludeComments(new LeagueUser(leagueName, User));
        var request = new GetReviewsFromSessionRequest(sessionId, includeComments);
        var getReviews = await mediator.Send(request, cancellationToken);
        return Ok(getReviews);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Events/{eventId:long}/[controller]")]
    public async Task<ActionResult<IEnumerable<ReviewModel>>> GetFromEvent([FromRoute] string leagueName, [FromRoute] long eventId, 
        CancellationToken cancellationToken)
    {
        var includeComments = IncludeComments(new LeagueUser(leagueName, User));
        var request = new GetReviewsFromEventRequest(eventId, includeComments);
        var getReviews = await mediator.Send(request, cancellationToken);
        return Ok(getReviews);
    }

    [HttpPost]
    [Route("{id:long}/MoveToSession/{sessionId:long}")]
    public async Task<ActionResult<ReviewModel>> MoveReviewToSession([FromRoute] string leagueName, [FromRoute] long id,
        [FromRoute] long sessionId, CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new MoveReviewToSessionRequest(sessionId, id, leagueUser);
        var getReview = await mediator.Send(request, cancellationToken);
        return Ok(getReview);
    }
}
