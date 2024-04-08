using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Championships;
using iRLeagueApiCore.Server.Handlers.Events;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[Authorize]
[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole]
[Route("{leagueName}/[controller]")]
public class ChampionshipsController : LeagueApiController<ChampionshipsController>
{
    public ChampionshipsController(ILogger<ChampionshipsController> logger, IMediator mediator) : 
        base(logger, mediator)
    {
    }

    /// <summary>
    /// Get single championship by id
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("{id:long}")]
    public async Task<ActionResult<ChampionshipModel>> Get([FromRoute] string leagueName, [FromRoute] long id, 
        CancellationToken cancellationToken = default)
    {
        var request = new GetChampionshipRequest(id);
        var getChampionship = await mediator.Send(request, cancellationToken);
        return Ok(getChampionship);
    }

    /// <summary>
    /// Get all championships from a league
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("")]
    public async Task<ActionResult<IEnumerable<ChampionshipModel>>> GetFromLeague([FromRoute] string leagueName, 
        CancellationToken cancellationToken = default)
    {
        var request = new GetChampionshipsFromLeagueRequest();
        var getChampionships = await mediator.Send(request, cancellationToken);
        return Ok(getChampionships);
    }

    /// <summary>
    /// Post a new championship
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="postChampionship"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("")]
    public async Task<ActionResult<ChampionshipModel>> Post([FromRoute] string leagueName, [FromBody] PostChampionshipModel postChampionship,
        CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostChampionshipRequest(leagueUser, postChampionship);
        var getChampionship = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getChampionship.ChampionshipId }, getChampionship);
    }

    /// <summary>
    /// Update a single championship
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="putChampionship"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    [Route("{id:long}")]
    public async Task<ActionResult<ChampionshipModel>> Put([FromRoute] string leagueName, [FromRoute] long id, 
        [FromBody] PutChampionshipModel putChampionship, CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutChampionshipRequest(id, leagueUser, putChampionship);
        var getChampionship = await mediator.Send(request, cancellationToken);
        return Ok(getChampionship);
    }

    /// <summary>
    /// Delete a championship permanently
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [RequireLeagueRole(LeagueRoles.Admin)]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id, 
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteChampionshipRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
