using iRLeagueApiCore.Server.Authentication;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Users;

public record ConfirmEmailRequest(string UserId, string EmailConfirmationToken) : IRequest<(bool success, string status)>;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailRequest, (bool success, string status)>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IEnumerable<IValidator<ConfirmEmailRequest>> validators;

    public ConfirmEmailHandler(UserManager<ApplicationUser> userManager, IEnumerable<IValidator<ConfirmEmailRequest>> validators)
    {
        this.userManager = userManager;
        this.validators = validators;
    }

    public async Task<(bool success, string status)> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ResourceNotFoundException();
        var result = await userManager.ConfirmEmailAsync(user, request.EmailConfirmationToken);
        if (result.Succeeded == false)
        {
            return (false, string.Join("\n",  result.Errors.Select(x => x.Description)));
        }
        return (true, "Success");
    }
}
