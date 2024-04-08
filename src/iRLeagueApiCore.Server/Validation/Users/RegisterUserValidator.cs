using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Users;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Validation.Users;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    private readonly UserManager<ApplicationUser> userManager;

    public RegisterUserValidator(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;

        RuleFor(x => x.Model.Username)
            .NotEmpty()
            .MinimumLength(4)
            .Matches("^[a-zA-Z0-9_-]{4,}$")
            .WithMessage("Username can only contain following characters: \"a-zA-Z0-9_-\"")
            .MustAsync(UsernameUnique)
            .WithMessage("Username already exists");

        RuleFor(x => x.Model.Firstname)
            .NotEmpty()
            .WithMessage("Firstname may not be empty")
            .Must(x => x.IndexOfAny(new[] { ';', '!', ',', '$', '&', '§', '"', '/', '{', '}', '[', ']', '\\' }) == -1)
            .WithMessage("Firstname may not contain special characters");

        RuleFor(x => x.Model.Lastname)
            .NotEmpty()
            .WithMessage("Lastname may not be empty")
            .Must(x => x.IndexOfAny(new[] { ';', '!', ',', '$', '&', '§', '"', '/', '{', '}', '[', ']', '\\' }) == -1)
            .WithMessage("Lastname may not contain special characters");

        RuleFor(x => x.Model.Email)
            .EmailAddress()
            .MustAsync(EmailAddressUnique)
            .WithMessage("This email address is already in use by another user");

        RuleFor(x => x.Model.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])?(?=.*[#$^+=!*()@%&]).{8,255}$")
            .WithMessage("Password must contain upercase, lowercase and at least one non-alphanumeric letter: \"#$^+=!*()@%&\"");
    }

    private async Task<bool> UsernameUnique(string username, CancellationToken cancellationToken)
    {
        return await userManager.FindByNameAsync(username) is null;
    }

    private async Task<bool> EmailAddressUnique(string email, CancellationToken cancellationToken)
    {
        return await userManager.FindByEmailAsync(email) is null;
    }
}
