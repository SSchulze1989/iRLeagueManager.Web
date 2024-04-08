using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Seasons;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole]
[Route("{leagueName}/[controller]")]
public sealed class SeasonsController : LeagueApiController<SeasonsController>
{
    public SeasonsController(ILogger<SeasonsController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("")]
    public async Task<ActionResult<IEnumerable<SeasonModel>>> GetAll([FromRoute] string leagueName, CancellationToken cancellationToken = default)
    {
        var request = new GetSeasonsRequest();
        var getSeasons = await mediator.Send(request, cancellationToken);
        return Ok(getSeasons);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{id:long}")]
    public async Task<ActionResult<SeasonModel>> Get([FromRoute] string leagueName, [FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var request = new GetSeasonRequest(id);
        var getSeason = await mediator.Send(request, cancellationToken);
        return Ok(getSeason);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("Current")]
    public async Task<ActionResult<SeasonModel>> GetCurrent([FromRoute] string leagueName, CancellationToken cancellationToken = default)
    {
        var request = new GetCurrentSeasonRequest();
        var getSeason = await mediator.Send(request, cancellationToken);
        return Ok(getSeason);
    }

    [HttpPost]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    [Route("")]
    public async Task<ActionResult<SeasonModel>> Post([FromRoute] string leagueName, [FromBody] PostSeasonModel postSeason, CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostSeasonRequest(leagueUser, postSeason);
        var getSeason = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getSeason.SeasonId }, getSeason);
    }

    [HttpPut]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    [Route("{id:long}")]
    public async Task<ActionResult<SeasonModel>> Put([FromRoute] string leagueName, [FromRoute] long id, [FromBody] PutSeasonModel putSeason, CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutSeasonRequest(leagueUser, id, putSeason);
        var getSeason = await mediator.Send(request, cancellationToken);
        return Ok(getSeason);
    }

    [HttpDelete]
    [RequireLeagueRole(LeagueRoles.Admin)]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var request = new DeleteSeasonRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
