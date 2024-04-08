using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Users;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Handlers.Admin;
using Microsoft.AspNetCore.Mvc;

namespace iRLeagueApiCore.Server.Controllers;

[TypeFilter(typeof(LeagueAuthorizeAttribute))]
[RequireLeagueRole(LeagueRoles.Admin)]
[Route("{leagueName}/[controller]")]
public sealed class AdminController : LeagueApiController<AdminController>
{
    public AdminController(ILogger<AdminController> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    /// <summary>
    /// List the users with at least one role in the current league
    /// </summary>
    /// <param name="leagueName">[Required] Name of the league</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("ListUsers")]
    public async Task<ActionResult<IEnumerable<AdminUserModel>>> ListUsers([FromRoute] string leagueName, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ListUsersRequest(leagueName);
            var getUsers = await mediator.Send(request, cancellationToken);
            if (getUsers.Count() == 0)
            {
                return NotFound();
            }
            return Ok(getUsers);
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation("Bad request - errors: {ValidationErrors}", ex.Errors.Select(x => x.ErrorMessage));
            return ex.ToActionResult();
        }
    }

    /// <summary>
    /// Give a league role to a user
    /// </summary>
    /// <param name="leagueName">Name of the league</param>
    /// <param name="userRole"><c>RoleName</c> of the role to give to the user named <c>UserName</c></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Action result</returns>
    [HttpPost("GiveRole")]
    public async Task<ActionResult> GiveRole([FromRoute] string leagueName, [FromBody] UserRoleModel userRole, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Give league role {LeagueRole} to user {RoleUser} for {LeagueName} by {UserName}",
                userRole.RoleName, userRole.UserName, leagueName, GetUsername());
            var request = new GiveRoleRequest(leagueName, userRole);
            await mediator.Send(request, cancellationToken);
            return OkMessage($"Role {userRole.RoleName} given to user {userRole.UserName}");
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation("Bad request - errors: {ValidationErrors}", ex.Errors.Select(x => x.ErrorMessage));
            return ex.ToActionResult();
        }
    }
}
