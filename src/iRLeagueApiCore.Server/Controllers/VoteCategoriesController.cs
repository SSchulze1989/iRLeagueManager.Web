using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Reviews;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[RequireLeagueRole(LeagueRoles.Admin, LeagueRoles.Steward)]
[Route("{leagueName}/[controller]")]
public sealed class VoteCategoriesController : LeagueApiController<VoteCategoriesController>
{
    public VoteCategoriesController(ILogger<VoteCategoriesController> logger, IMediator mediator) :
        base(logger, mediator)
    {
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("")]
    public async Task<ActionResult<IEnumerable<VoteCategoryModel>>> GetAll([FromRoute] string leagueName, [FromRoute] long id,
            CancellationToken cancellationToken)
    {
        var request = new GetLeagueVoteCategoriesRequest();
        var getVoteCategories = await mediator.Send(request, cancellationToken);
        return Ok(getVoteCategories);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{id:long}")]
    public async Task<ActionResult<ReviewModel>> Get([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new GetVoteCategoryRequest(id);
        var getVoteCategory = await mediator.Send(request, cancellationToken);
        return Ok(getVoteCategory);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<VoteCategoryModel>> Post([FromRoute] string leagueName,
        [FromBody] PostVoteCategoryModel postVoteCategory, CancellationToken cancellationToken)
    {
        var request = new PostVoteCategoryRequest(postVoteCategory);
        var getVoteCategory = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { leagueName, id = getVoteCategory.Id }, getVoteCategory);
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<ActionResult<VoteCategoryModel>> Put([FromRoute] string leagueName, [FromRoute] long id,
        [FromBody] PutVoteCategoryModel putVoteCategory, CancellationToken cancellationToken)
    {
        var request = new PutVoteCategoryRequest(id, putVoteCategory);
        var getVoteCategory = await mediator.Send(request, cancellationToken);
        return Ok(getVoteCategory);
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete([FromRoute] string leagueName, [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var request = new DeleteVoteCategoryRequest(id);
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
