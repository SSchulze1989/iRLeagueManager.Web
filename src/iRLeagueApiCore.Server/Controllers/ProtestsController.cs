using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Reviews;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[Route("{leagueName}/[controller]")]
public class ProtestsController : LeagueApiController<ProtestsController>
{
    public ProtestsController(ILogger<ProtestsController> logger, IMediator mediator) : 
        base(logger, mediator)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("/{leagueName}/Sessions/{sessionId:long}/[controller]")]
    public async Task<ActionResult<ProtestModel>> Post([FromRoute] string leagueName, [FromRoute] long sessionId, PostProtestModel postReview, 
        CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostProtestToSessionRequest(sessionId, leagueUser, postReview);
        var getProtest = await mediator.Send(request, cancellationToken);
        return Created(string.Empty, getProtest);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Events/{eventId:long}/Protests")]
    public async Task<ActionResult<IEnumerable<ProtestModel>>> GetFromEvent([FromRoute] string leagueName, [FromRoute] long eventId, 
        CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new GetProtestsFromEventRequest(leagueUser, eventId);
        var getProtests = await mediator.Send(request, cancellationToken);
        return Ok(getProtests);
    }

    [HttpDelete]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Steward)]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new DeleteProtestRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
