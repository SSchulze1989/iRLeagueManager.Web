using iRLeagueApiCore.Common.Models.Tracks;
using iRLeagueApiCore.Server.Handlers.Tracks;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[Route("[controller]")]
public sealed class TracksController : LeagueApiController<TracksController>
{
    public TracksController(ILogger<TracksController> logger, IMediator mediator) :
        base(logger, mediator)
    {
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult<IEnumerable<TrackGroupModel>>> Get(CancellationToken cancellationToken)
    {
        var request = new GetTracksRequest();
        var getTracks = await mediator.Send(request, cancellationToken);
        return Ok(getTracks);
    }

    [HttpPost("UpdateTracks")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ImportTracks([FromBody] IracingAuthModel authData, CancellationToken cancellationToken)
    {
        var request = new ImportTracksCommand(authData);
        await mediator.Send(request, cancellationToken);
        return Ok();
    }
}
