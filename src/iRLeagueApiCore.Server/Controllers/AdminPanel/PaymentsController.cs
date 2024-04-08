using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.AdminPanel;
using iRLeagueApiCore.Server.Models.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers.AdminPanel;

[ApiController]
[TypeFilter(typeof(DefaultExceptionFilterAttribute))]
[Authorize(Roles = "Admin")]
[Route("AdminPanel/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator mediator;

    public PaymentsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Get payments from customers
    /// </summary>
    /// <param name="leagueId">[Optional] If set: only show payments done for this league</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentModel>>> Get([FromQuery] long? leagueId, CancellationToken cancellationToken)
    {
        var request = new GetAllPaymentsRequest(leagueId);
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get single payment by id
    /// </summary>
    /// <param name="id">Id of the payment</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<PaymentModel>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetPaymentRequest(id);
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new payment 
    /// </summary>
    /// <param name="leagueId">Id of league for which the payment will be set</param>
    /// <param name="model">Payment data</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<PaymentModel>> Post([FromQuery] long leagueId, [FromBody] PostPaymentModel model, CancellationToken cancellationToken)
    {
        var request = new PostPaymentRequest(leagueId, model);
        var result = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Deactivate a running payment (when Subscription is cancelled)
    /// </summary>
    /// <param name="id">Id of the payment</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id:Guid}/Deactivate")]
    public async Task<ActionResult<PaymentModel>> Deactivate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeactivatePaymentRequest(id);
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Directly set the current league subscription status
    /// </summary>
    /// <param name="leagueId">Id of the league</param>
    /// <param name="model">Status and expiration of subscription</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("SetSubscription/{leagueId:long}")]
    public async Task<ActionResult<LeagueModel>> SetSubscription([FromRoute] long leagueId, [FromBody] SetLeagueSubscriptionModel model,
        CancellationToken cancellationToken)
    {
        var request = new SetLeagueSubscriptionRequest(leagueId, model);
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}
