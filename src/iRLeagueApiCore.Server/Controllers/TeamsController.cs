using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Teams;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[Authorize]
[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[Route("{leagueName}/[controller]")]
public sealed class TeamsController : LeagueApiController<TeamsController>
{
    public TeamsController(ILogger<TeamsController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<IEnumerable<TeamModel>>> GetAll([FromRoute] string leagueName, CancellationToken cancellationToken)
    {
        var request = new GetTeamsFromLeagueRequest();
        var getTeams = await mediator.Send(request, cancellationToken);
        return Ok(getTeams);
    }

    [HttpGet]
    [Route("{id:long}")]
    public async Task<ActionResult<TeamModel>> Get([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new GetTeamRequest(id);
        var getTeam = await mediator.Send(request, cancellationToken);
        return Ok(getTeam);
    }

    [HttpPost]
    [Route("")]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult<TeamModel>> Post([FromRoute] string leagueName,
        [FromBody] PostTeamModel postTeam, CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostTeamRequest(leagueUser, postTeam);
        var getTeam = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getTeam.TeamId }, getTeam);
    }

    [HttpPut]
    [Route("{id:long}")]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult<TeamModel>> Put([FromRoute] string leagueName, [FromRoute] long id,
        [FromBody] PutTeamModel putTeam, CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutTeamRequest(id, leagueUser, putTeam);
        var getTeam = await mediator.Send(request, cancellationToken);
        return Ok(getTeam);
    }

    [HttpDelete]
    [Route("{id:long}")]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new DeleteTeamRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
