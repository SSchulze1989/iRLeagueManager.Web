using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace iRLeagueApiCore.Server.Validation.Authentication;

public sealed class SetPasswordWithTokenRequestValidator : AbstractValidator<SetPasswordWithTokenRequest>
{
    private readonly UserManager<ApplicationUser> userManager;

    public SetPasswordWithTokenRequestValidator(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;

        RuleFor(x => x.Model.NewPassword)
            .NotEmpty()
            .WithMessage("Password cannot be empty")
            .Must(PasswordIsValid)
            .WithMessage("Invalid password. Password must have at least 6 characters, must contain one upper- and lowercase letter and one special character ($!%*?&)");
        RuleFor(x => x.UserId)
            .MustAsync(UserExists)
            .WithMessage("User with the given user id does not exist");
    }

    private bool PasswordIsValid(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*?&/\\])[A-Za-z\d@$!%*?&/\\]{6,}$");
    }

    private async Task<bool> UserExists(string userId, CancellationToken cancellationToken)
    {
        return await userManager.FindByIdAsync(userId) is not null;
    }
}
