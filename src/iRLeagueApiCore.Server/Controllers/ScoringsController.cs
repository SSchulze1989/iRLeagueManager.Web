using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Scorings;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole]
[Route("{leagueName}/[controller]")]
public sealed class ScoringsController : LeagueApiController<ScoringsController>
{
    public ScoringsController(ILogger<ScoringsController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    /// <summary>
    /// Get all scorings from league
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Route("")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScoringModel>>> Get([FromRoute] string leagueName, CancellationToken cancellationToken = default)
    {
        var request = new GetScoringsRequest();
        var getScorings = await mediator.Send(request, cancellationToken);
        return Ok(getScorings);
    }

    /// <summary>
    /// Get scoring from league by Id
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Route("{id:long}")]
    [HttpGet]
    public async Task<ActionResult<ScoringModel>> Get([FromRoute] string leagueName, [FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var request = new GetScoringRequest(id);
        var getScoring = await mediator.Send(request, cancellationToken);
        return Ok(getScoring);
    }

    /// <summary>
    /// Update existing scoring with Id
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Route("{id:long}")]
    [HttpPut]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult<ScoringModel>> Put([FromRoute] string leagueName, [FromRoute] long id, [FromBody] PutScoringModel model, 
        CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutScoringRequest(id, leagueUser, model);
        var getScoring = await mediator.Send(request, cancellationToken);
        return Ok(getScoring);
    }

    /// <summary>
    /// Delete existing scoring with id
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Route("{id:long}")]
    [HttpDelete]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var request = new DeleteScoringRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
