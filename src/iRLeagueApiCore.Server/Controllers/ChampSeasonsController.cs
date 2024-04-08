using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Championships;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[Authorize]
[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole]
[Route("{leagueName}/[controller]")]
public class ChampSeasonsController : LeagueApiController<ChampSeasonsController>
{
    public ChampSeasonsController(ILogger<ChampSeasonsController> logger, IMediator mediator) :
        base(logger, mediator)
    {
    }

    /// <summary>
    /// Get single champSeason by id
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("{id:long}")]
    public async Task<ActionResult<ChampSeasonModel>> Get([FromRoute] string leagueName, [FromRoute] long id, 
        CancellationToken cancellationToken = default)
    {
        var request = new GetChampSeasonRequest(id);
        var getChampSeason = await mediator.Send(request, cancellationToken);
        return Ok(getChampSeason);
    }

    /// <summary>
    /// Get all champSeasons from a championship
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Championships/{id:long}/ChampSeasons")]
    public async Task<ActionResult<IEnumerable<ChampSeasonModel>>> GetFromChampionship([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken = default)
    {
        var request = new GetChampSeasonsFromChampionshipRequest(id);
        var getChampSeasons = await mediator.Send(request, cancellationToken);
        return Ok(getChampSeasons);
    }

    /// <summary>
    /// Get all champSeasons from a season
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Seasons/{id:long}/ChampSeasons")]
    public async Task<ActionResult<IEnumerable<ChampSeasonModel>>> GetFromSeason([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken = default)
    {
        var request = new GetChampSeasonsFromSeasonRequest(id);
        var getChampSeasons = await mediator.Send(request, cancellationToken);
        return Ok(getChampSeasons);
    }

    /// <summary>
    /// Get a single champseason from a season and championship
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="championshipId"></param>
    /// <param name="seasonId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("/{leagueName}/Seasons/{seasonId:long}/Championships/{championshipId:long}")]
    public async Task<ActionResult<ChampSeasonModel>> GetFromSeasonAndChampionship([FromRoute] string leagueName, [FromRoute] long championshipId,
        [FromRoute] long seasonId, CancellationToken cancellationToken = default)
    {
        var request = new GetChampSeasonFromSeasonChampionshipRequest(seasonId, championshipId);
        var getChampSeason = await mediator.Send(request, cancellationToken);
        return Ok(getChampSeason);
    }

    /// <summary>
    /// Post a new champSeason for a championship to the selected season
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="championshipId"></param>
    /// <param name="seasonId"></param>
    /// <param name="postChampSeason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("/{leagueName}/Seasons/{seasonId:long}/Championships/{championshipId:long}")]
    public async Task<ActionResult<ChampSeasonModel>> Post([FromRoute] string leagueName, [FromRoute] long championshipId, [FromRoute] long seasonId, 
        [FromBody] PostChampSeasonModel postChampSeason, CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostChampSeasonRequest(championshipId, seasonId, leagueUser, postChampSeason);
        var getChampSeason = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getChampSeason.ChampSeasonId }, getChampSeason);
    }

    /// <summary>
    /// Update a single champSeason
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="putChampSeason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    [Route("{id:long}")]
    public async Task<ActionResult<ChampSeasonModel>> Put([FromRoute] string leagueName, [FromRoute] long id, 
        [FromBody] PutChampSeasonModel putChampSeason, CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutChampSeasonRequest(id, leagueUser, putChampSeason);
        var getChampSeason = await mediator.Send(request, cancellationToken);
        return Ok(getChampSeason);
    }

    /// <summary>
    /// Delete a champSeason permanently
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [RequireLeagueRole(LeagueRoles.Admin)]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var request = new DeleteChampSeasonRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
