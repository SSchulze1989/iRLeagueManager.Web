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
public sealed class PointRulesController : LeagueApiController<PointRulesController>
{
    public PointRulesController(ILogger<PointRulesController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet]
    [Route("{id:long}")]
    public async Task<ActionResult<PointRuleModel>> Get([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new GetPointRuleRequest(id);
        var getPointRule = await mediator.Send(request, cancellationToken);
        return Ok(getPointRule);
    }

    [HttpPost]
    [Route("")]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult<PointRuleModel>> Post([FromRoute] string leagueName, PostPointRuleModel postPointRule,
        CancellationToken cancellationToken)
    {
        var leagueUser = new LeagueUser(leagueName, User);
        var request = new PostPointRuleRequest(leagueUser, postPointRule);
        var getPointRule = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getPointRule.PointRuleId }, getPointRule);
    }

    [HttpPut]
    [Route("{id:long}")]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult<PointRuleModel>> Put([FromRoute] string leagueName, [FromRoute] long id,
        [FromBody] PutPointRuleModel putPointRule, CancellationToken cancellationToken)
    {
                var leagueUser = new LeagueUser(leagueName, User);
        var request = new PutPointRuleRequest(id, leagueUser, putPointRule);
        var getPointRule = await mediator.Send(request, cancellationToken);
        _logger.LogInformation("Return entry for pointRule {PointRuleId} from {LeagueName}", getPointRule.PointRuleId, leagueName);
        return Ok(getPointRule);
    }

    [HttpDelete]
    [Route("{id:long}")]
    [RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Organizer)]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new DeletePointRuleRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
