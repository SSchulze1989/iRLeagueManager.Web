using iRLeagueApiCore.Client.ResultsParsing;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[Route("/{leagueName}/[controller]")]
[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[TypeFilter(typeof(CheckLeagueSubscriptionAttribute))]
[RequireLeagueRole]
public sealed class ResultsController : LeagueApiController<ResultsController>
{
    public ResultsController(ILogger<ResultsController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    /// <summary>
    /// Get single result from specific resultId
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("{id:long}")]
    public async Task<ActionResult<EventResultModel>> Get([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new GetResultRequest(id);
        var getResult = await mediator.Send(request, cancellationToken);
        return Ok(getResult);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("latest")]
    public async Task<ActionResult<IEnumerable<EventResultModel>>> GetLatest([FromRoute] string leagueName, CancellationToken cancellationToken = default)
    {
        var request = new GetLatestResultRequest();
        var getResult = await mediator.Send(request, cancellationToken);
        return Ok(getResult);
    }

    /// <summary>
    /// Get all results from a season
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="seasonId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Seasons/{seasonId:long}/[controller]")]
    public async Task<ActionResult<IEnumerable<SeasonEventResultModel>>> GetFromSeason([FromRoute] string leagueName, [FromRoute] long seasonId,
        CancellationToken cancellationToken = default)
    {
        var request = new GetResultsFromSeasonRequest(seasonId);
        var getResults = await mediator.Send(request, cancellationToken);
        return Ok(getResults);
    }

    /// <summary>
    /// Get all results from a session
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="eventId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("/{leagueName}/Events/{eventId:long}/[controller]")]
    public async Task<ActionResult<IEnumerable<EventResultModel>>> GetFromEvent([FromRoute] string leagueName, [FromRoute] long eventId, 
        CancellationToken cancellationToken = default)
    {
        var request = new GetResultsFromEventRequest(eventId);
        var getResults = await mediator.Send(request, cancellationToken);
        return Ok(getResults);
    }

    [HttpDelete]
    [Route("/{leagueName}/Events/{eventId:long}/[controller]")]
    public async Task<ActionResult> DeleteFromEvent([FromRoute] string leagueName, [FromRoute] long eventId,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteResultRequest(eventId);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Upload a result json (must be exported from the iRacing GUI)
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="eventId"></param>
    /// <param name="subsessionId">complete json data exported from iRacing GUI</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    //[RequireSubscription]
    [Route("/{leagueName}/Events/{eventId:long}/[controller]/Upload")]
    public async Task<ActionResult<bool>> UploadResult([FromRoute] string leagueName, [FromRoute] long eventId,
        [FromBody] ParseSimSessionResult subsessionId, CancellationToken cancellationToken = default)
    {
        var request = new UploadResultRequest(eventId, subsessionId);
        var success = await mediator.Send(request, cancellationToken);
        if (success)
        {
            return Ok(true);
        }
        else
        {
            return BadRequest("Oops, something went wrong with the result upload!");
        }
    }

    /// <summary>
    /// Fetch a result from the iracing API and automatically update the associated event result
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="eventId"></param>
    /// <param name="subsessionId">complete json data exported from iRacing GUI</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    //[RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    //[RequireSubscription]
    [Route("/{leagueName}/Events/{eventId:long}/[controller]/Fetch/{subsessionId:int}")]
    public async Task<ActionResult<bool>> FetchFromIracing([FromRoute] string leagueName, [FromRoute] long eventId,
        [FromRoute] int subsessionId, CancellationToken cancellationToken = default)
    {
        var request = new FetchResultsFromIRacingAPIRequest(eventId, subsessionId);
        var success = await mediator.Send(request, cancellationToken);
        if (success)
        {
            return Ok(true);
        }
        else
        {
            return BadRequest("Oops, something went wrong with the result upload!");
        }
    }

    [HttpPost]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    [Route("/{leagueName}/Events/{eventId:long}/[controller]/Calculate")]
    public async Task<ActionResult<bool>> TriggerResultCalculation([FromRoute] string leagueName, [FromRoute] long eventId,
        CancellationToken cancellationToken = default)
    {
        var request = new TriggerResultCalculationCommand(eventId);
        await mediator.Send(request, cancellationToken);
        return Ok(true);
    }
}
