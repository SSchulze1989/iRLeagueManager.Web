using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Events;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

/// <summary>
/// Endpoint for managing session entries
/// </summary>
[Authorize]
[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole]
[Route("{leagueName}/[controller]")]
public sealed class EventsController : LeagueApiController<EventsController>
{
    public EventsController(ILogger<EventsController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{id:long}")]
    public async Task<ActionResult<EventModel>> Get([FromRoute] string leagueName, [FromRoute] long id, [FromQuery] bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var request = new GetEventRequest(id, includeDetails);
        var getEvent = await mediator.Send(request, cancellationToken);
        return Ok(getEvent);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Schedules/{scheduleId:long}/Events")]
    public async Task<ActionResult<IEnumerable<EventModel>>> GetFromSchedule([FromRoute] string leagueName, [FromRoute] long scheduleId, [FromQuery] bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var request = new GetEventsFromScheduleRequest(scheduleId, includeDetails);
        var getEvents = await mediator.Send(request, cancellationToken);
        return Ok(getEvents);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Seasons/{seasonId:long}/Events")]
    public async Task<ActionResult<IEnumerable<EventModel>>> GetFromSeason([FromRoute] string leagueName,
        [FromRoute] long seasonId, [FromQuery] bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var request = new GetEventsFromSeasonRequest(seasonId, includeDetails);
        var getSessions = await mediator.Send(request, cancellationToken);
        return Ok(getSessions);
    }

    [HttpPost]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    [Route("/{leagueName}/Schedules/{scheduleId:long}/Events")]
    public async Task<ActionResult<EventModel>> PostToSchedule([FromRoute] string leagueName,
        [FromRoute] long scheduleId, [FromBody] PostEventModel postEvent, CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostEventToScheduleRequest(scheduleId, leagueUser, postEvent);
        var getEvent = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getEvent.Id }, getEvent);
    }

    [HttpPut]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    [Route("{id:long}")]
    public async Task<ActionResult<EventModel>> Put([FromRoute] string leagueName,
        [FromRoute] long id, [FromBody] PutEventModel putEvent, CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutEventRequest(id, leagueUser, putEvent);
        var getEvent = await mediator.Send(request, cancellationToken);
        return Ok(getEvent);
    }

    [HttpDelete]
    [RequireLeagueRole(LeagueRoles.Admin)]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete([FromRoute] string leagueName,
        [FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var request = new DeleteEventRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
