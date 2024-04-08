using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Extensions;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace iRLeagueApiCore.Server.Controllers;

[Route("[controller]")]
public sealed class LeaguesController : LeagueApiController<LeaguesController>
{
    public LeaguesController(ILogger<LeaguesController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("")]
    public async Task<ActionResult<IEnumerable<LeagueModel>>> GetAll(CancellationToken cancellationToken = default)
    {
        var includeHidden = User.IsInRole("Admin");
        var owned = GetUserLeagues(User);
        var request = new GetLeaguesRequest(owned, includeHidden);
        var getLeagues = await mediator.Send(request, cancellationToken);
        return Ok(getLeagues);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{leagueId:long}")]
    public async Task<ActionResult<LeagueModel>> Get([FromRoute] long leagueId, CancellationToken cancellationToken = default)
    {
        var leagueUser = HttpContext.GetLeagueUser();
        var includeSubscriptionDetails = leagueUser.IsInRole(LeagueRoles.Admin, LeagueRoles.Organizer);
        var request = new GetLeagueRequest(leagueId, includeSubscriptionDetails);
        var getLeague = await mediator.Send(request, cancellationToken);
        return Ok(getLeague);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}")]
    public async Task<ActionResult<LeagueModel>> GetByName([FromRoute] string leagueName, CancellationToken cancellationToken = default)
    {
        var leagueUser = HttpContext.GetLeagueUser();
        var includeSubscriptionDetails = leagueUser.IsInRole(LeagueRoles.Admin, LeagueRoles.Organizer);
        var request = new GetLeagueByNameRequest(leagueName, includeSubscriptionDetails);
        var getLeague = await mediator.Send(request, cancellationToken);
        return Ok(getLeague);
    }

    [HttpPost]
    [Route("")]
    [Authorize]
    public async Task<ActionResult<LeagueModel>> Post([FromBody] PostLeagueModel postLeague, CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(string.Empty, User);
        var request = new PostLeagueRequest(leagueUser, postLeague);
        var getLeague = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueId = getLeague.Id }, getLeague);
    }

    [HttpPut]
    [Route("{leagueId:long}")]
    [TypeFilter(typeof(LeagueAuthorizeAttribute))]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult<LeagueModel>> Put([FromRoute] long leagueId, [FromBody] PutLeagueModel putLeague, CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(string.Empty, User);
        var request = new PutLeagueRequest(leagueId, leagueUser, putLeague);
        var getLeague = await mediator.Send(request, cancellationToken);
        return Ok(getLeague);
    }

    [HttpDelete]
    [Route("{leagueId:long}")]
    [TypeFilter(typeof(LeagueAuthorizeAttribute))]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult> Delete([FromRoute] long leagueId, CancellationToken cancellationToken = default)
    {
        var request = new DeleteLeagueRequest(leagueId);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpPost]
    [Route("{leagueId:long}")]
    [TypeFilter(typeof(LeagueAuthorizeAttribute))]
    [RequireLeagueRole(LeagueRoles.Admin)]
    public async Task<ActionResult> Initialize([FromRoute] long leagueId, CancellationToken cancellationToken = default)
    {
        var request = new PostIntitializeLeagueRequest(leagueId);
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    private static IEnumerable<string> GetUserLeagues(ClaimsPrincipal user)
    {
        var userRoles = user.Claims.Where(x => x.Type == ClaimTypes.Role);
        var leagueRoles = userRoles
            .Select(x => x.Value.Split(LeagueRoles.RoleDelimiter))
            .Where(x => x.Length == 2)
            .Select(x => new { League = x.ElementAt(0), Role = x.ElementAt(1) });
        var owned = leagueRoles
            .Select(x => x.League)
            .Distinct();
        return owned;
    }
}
