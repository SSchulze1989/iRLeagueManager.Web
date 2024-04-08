using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Results;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole]
[Route("{leagueName}/[controller]")]
public sealed class ResultConfigsController : LeagueApiController<ResultConfigsController>
{
    public ResultConfigsController(ILogger<ResultConfigsController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ResultConfigModel>>> GetAll([FromRoute] string leagueName, CancellationToken cancellationToken)
    {
        var request = new GetResultConfigsFromLeagueRequest();
        var getResultConfigs = await mediator.Send(request, cancellationToken);
        return Ok(getResultConfigs);
    }

    [HttpGet]
    [Route("{id:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<ResultConfigModel>> Get([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new GetResultConfigRequest(id);
        var getResultConfig = await mediator.Send(request, cancellationToken);
        return Ok(getResultConfig);
    }

    [HttpGet]
    [Route("/{leagueName}/Seasons/{seasonId:long}/ResultConfigs")]
    [AllowAnonymous]
    public async Task<ActionResult<ResultConfigModel>> GetFromSeason([FromRoute] string leagueName, [FromRoute] long seasonId,
        CancellationToken cancellationToken)
    {
        var request = new GetResultConfigsFromSeasonRequest(seasonId);
        var getResultConfigs = await mediator.Send(request, cancellationToken);
        return Ok(getResultConfigs);
    }

    [HttpPost]
    [Route("/{leagueName}/ChampSeasons/{champSeasonId:long}/ResultConfigs")]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult<ResultConfigModel>> Post([FromRoute] string leagueName, [FromRoute] long champSeasonId,
        [FromBody] PostResultConfigModel postResultConfig, CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostResultConfigToChampSeasonRequest(champSeasonId, leagueUser, postResultConfig);
        var getResultConfig = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getResultConfig.ResultConfigId }, getResultConfig);
    }

    [HttpPut]
    [Route("{id:long}")]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult<ResultConfigModel>> Put([FromRoute] string leagueName, [FromRoute] long id,
        [FromBody] PutResultConfigModel putResultConfig, CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutResultConfigRequest(id, leagueUser, putResultConfig);
        var getResultConfig = await mediator.Send(request, cancellationToken);
        return Ok(getResultConfig);
    }

    [HttpDelete]
    [Route("{id:long}")]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new DeleteResultConfigRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
