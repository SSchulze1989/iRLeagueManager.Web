using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Schedules;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole]
[Route("{leagueName}/[controller]")]
public sealed class SchedulesController : LeagueApiController<SchedulesController>
{
    public SchedulesController(ILogger<SchedulesController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<IEnumerable<ScheduleModel>>> GetAll([FromRoute] string leagueName, CancellationToken cancellationToken = default)
    {
        var request = new GetSchedulesRequest();
        var getSchedules = await mediator.Send(request, cancellationToken);
        return Ok(getSchedules);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{id:long}")]
    public async Task<ActionResult<ScheduleModel>> Get([FromRoute] string leagueName, [FromRoute] long id, 
        CancellationToken cancellationToken = default)
    {
        var request = new GetScheduleRequest(id);
        var getSchedule = await mediator.Send(request, cancellationToken);
        return Ok(getSchedule);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Seasons/{seasonId:long}/[controller]")]
    public async Task<ActionResult<IEnumerable<ScheduleModel>>> GetFromSeason([FromRoute] string leagueName, [FromRoute] long seasonId, 
        CancellationToken cancellationToken = default)
    {
        var request = new GetSchedulesFromSeasonRequest(seasonId);
        var getSchedules = await mediator.Send(request, cancellationToken);
        return Ok(getSchedules);
    }

    [HttpPost]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    [Route("/{leagueName}/Seasons/{seasonId:long}/[controller]")]
    public async Task<ActionResult<ScheduleModel>> Post([FromRoute] string leagueName, [FromRoute] long seasonId, [FromBody] PostScheduleModel postSchedule, 
        CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostScheduleRequest(seasonId, leagueUser, postSchedule);
        var getSchedule = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getSchedule.ScheduleId }, getSchedule);
    }

    [HttpPut]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    [Route("{id:long}")]
    public async Task<ActionResult<ScheduleModel>> Put([FromRoute] string leagueName, [FromRoute] long id, [FromBody] PutScheduleModel putSchedule, 
        CancellationToken cancellationToken = default)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutScheduleRequest(leagueUser, id, putSchedule);
        var getSchedule = await mediator.Send(request, cancellationToken);
        return Ok(getSchedule);
    }

    [HttpDelete]
    [RequireLeagueRole(LeagueRoles.Admin)]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var request = new DeleteScheduleRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
