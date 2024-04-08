using iRLeagueApiCore.Server.Controllers;
using iRLeagueApiCore.Server.Handlers.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace iRLeagueApiCore.Server.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class DefaultExceptionFilterAttribute : Attribute, IExceptionFilter
{
    private readonly ILogger<DefaultExceptionFilterAttribute> _logger;

    public DefaultExceptionFilterAttribute(ILogger<DefaultExceptionFilterAttribute> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            ValidationException ex => ValidationActionResult(ex),
            ResourceNotFoundException ex => ResourceNotFoundActionResult(ex),
            UnauthorizedAccessException ex => UnauthorizedActionResult(ex, context.HttpContext),
            _ => context.Result
        };
    }

    private IActionResult ValidationActionResult(ValidationException validationException)
    {
        _logger.LogInformation("Bad request - errors: {ValidationErrors}",
                    validationException.Errors.Select(x => x.ErrorMessage));
        return validationException.ToActionResult();
    }

    private IActionResult ResourceNotFoundActionResult(ResourceNotFoundException notFoundException)
    {
        _logger.LogInformation("Resource not found");
        return new NotFoundResult();
    }

    private IActionResult UnauthorizedActionResult(UnauthorizedAccessException unauthorizedAccess, HttpContext context)
    {
        _logger.LogInformation("Permission denied for {User}", context.User.Identity?.Name ?? "Anonymous");
        return new ForbidResult();
    }
}
