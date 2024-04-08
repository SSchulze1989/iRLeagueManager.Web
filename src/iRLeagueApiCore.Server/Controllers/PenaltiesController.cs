using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Steward)]
[Route("{leagueName}/[controller]")]
public sealed class PenaltiesController : LeagueApiController<PenaltiesController>
{
    public PenaltiesController(ILogger<PenaltiesController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/ScoredSessionResults/{id:long}/[controller]")]
    public async Task<ActionResult<IEnumerable<PenaltyModel>>> GetPenaltiesFromSessionResult([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new GetPenaltiesFromSessionResultRequest(id);
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Results/{id:long}/[controller]")]
    public async Task<ActionResult<IEnumerable<PenaltyModel>>> GetPenaltiesFromEventResult([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new GetPenaltiesFromEventResultRequest(id);
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("/{leagueName}/ScoredSessionResults/{resultId:long}/Rows/{resultRowId:long}/[controller]")]
    public async Task<ActionResult<PenaltyModel>> PostPenaltyToResult([FromRoute] string leagueName, [FromRoute] long resultId, [FromRoute] long resultRowId,
        [FromBody] PostPenaltyModel postPenalty, CancellationToken cancellationToken)
    {
        var request = new PostPenaltyToResultRequest(resultId, resultRowId, postPenalty);
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<ActionResult<PenaltyModel>> PutPenalty([FromRoute] string leagueName, [FromRoute] long id,
        [FromBody] PutPenaltyModel putPenalty, CancellationToken cancellationToken)
    {
        var request = new PutPenaltyRequest(id, putPenalty);
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<ActionResult> DeletePenalty([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new DeletePenaltyRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
