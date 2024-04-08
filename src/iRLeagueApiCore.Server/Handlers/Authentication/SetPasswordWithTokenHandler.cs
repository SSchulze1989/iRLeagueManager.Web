using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Authentication;

public record SetPasswordWithTokenRequest(string UserId, SetPasswordTokenModel Model) : IRequest<bool>;

public sealed class SetPasswordWithTokenHandler : IRequestHandler<SetPasswordWithTokenRequest, bool>
{
    private readonly ILogger<SetPasswordWithTokenHandler> logger;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IEnumerable<IValidator<SetPasswordWithTokenRequest>> validators;

    public SetPasswordWithTokenHandler(ILogger<SetPasswordWithTokenHandler> logger, UserManager<ApplicationUser> userManager,
        IEnumerable<IValidator<SetPasswordWithTokenRequest>> validators)
    {
        this.logger = logger;
        this.userManager = userManager;
        this.validators = validators;
    }

    public async Task<bool> Handle(SetPasswordWithTokenRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new InvalidOperationException();
        logger.LogWarning("Resetting password for user {UserName}:{UserId}", user.UserName, user.Id);
        var result = await userManager.ResetPasswordAsync(user, request.Model.PasswordToken, request.Model.NewPassword);
        if (result.Succeeded == false)
        {
            logger.LogError("Password reset failed: {Errors}", result.Errors);
            return false;
        }
        logger.LogWarning("Password reset successfull");
        return true;
    }
}
