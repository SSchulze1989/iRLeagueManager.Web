using iRLeagueApiCore.Common.Models.Users;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Text;

namespace iRLeagueApiCore.Server.Handlers.Users;

public record RegisterUserRequest(RegisterModel Model, string LinkTemplate) : IRequest<(UserModel? user, IdentityResult result)>;

public enum UserRegistrationStatus
{
    Success,
    UserExists,
    CreateUserFailed,
}

public class RegisterUserHandler : UsersHandlerBase<RegisterUserHandler, RegisterUserRequest>, 
    IRequestHandler<RegisterUserRequest, (UserModel? user, IdentityResult result)>
{
    private readonly IEmailClient emailClient;

    public RegisterUserHandler(ILogger<RegisterUserHandler> logger, UserDbContext userDbContext, UserManager<ApplicationUser> userManager,
        IEnumerable<IValidator<RegisterUserRequest>> validators, IEmailClient emailClient) : base(logger, userManager, validators)
    {
        this.emailClient = emailClient;
    }

    public async Task<(UserModel? user, IdentityResult result)> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var model = request.Model;
        _logger.LogInformation("Registering new user {UserName}", model.Username);
        var userExists = await userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            _logger.LogInformation("User {UserName} already exists", model.Username);
            return (null, IdentityResult.Failed(userManager.ErrorDescriber.DuplicateUserName(model.Username)));
        }

        var user = CreateApplicationUser(model);
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to add user {UserName} due to errors: {Errors}", model.Username, result.Errors
                .Select(x => $"{x.Code}: {x.Description}"));
            return (null, result);
        }
        _logger.LogInformation("User {UserName} created succesfully", model.Username);

        var emailConfirmationToken = await GetEmailConfirmationToken(user);
        await SendConfirmationMail(user, emailConfirmationToken, request.LinkTemplate);

        var userModel = MapToUserModel(user, new());
        return (userModel, IdentityResult.Success);
    }

    private async Task SendConfirmationMail(ApplicationUser user, string token, string linkTemplate)
    {
        var subject = "Confirm your Emailaddress for iRLeagueManager.net";
        var body = GenerateMailBody(user, token, linkTemplate);
        await emailClient.SendNoReplyMailAsync(user.Email, subject, body);
    }
}
