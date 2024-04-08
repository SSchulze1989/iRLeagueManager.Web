using iRLeagueApiCore.Common.Models.Results;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

/// <summary>
/// Endpoint for retrieving information about cars
/// </summary>
[Authorize]
[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[TypeFilter(typeof(SetTenantLeagueIdAttribute))]
[Route("{leagueName}/[controller]")]
public class CarsController : LeagueApiController<CarsController>
{
    public CarsController(ILogger<CarsController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    [HttpGet]
    [Route("/{leagueName}/Events/{eventId:long}/[controller]")]
    [AllowAnonymous]
    public async Task<ActionResult<CarListModel>> Get([FromRoute] string leagueName, [FromRoute] long eventId,
        CancellationToken cancellationToken)
    {
        var request = new GetCarsFromEventRequest(eventId);
        var getMembers = await mediator.Send(request, cancellationToken);
        return Ok(getMembers);
    }
}
