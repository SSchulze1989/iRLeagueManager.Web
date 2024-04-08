using iRLeagueApiCore.Common.Models.Users;
using iRLeagueApiCore.Server.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace iRLeagueApiCore.Server.Handlers.Users;

public class UsersHandlerBase<THandler, TRequest>
{
    protected readonly ILogger<THandler> _logger;
    protected readonly UserManager<ApplicationUser> userManager;
    protected IEnumerable<IValidator<TRequest>> validators;

    public UsersHandlerBase(ILogger<THandler> logger, UserManager<ApplicationUser> userManager, IEnumerable<IValidator<TRequest>> validators)
    {
        _logger = logger;
        this.userManager = userManager;
        this.validators = validators;
    }

    protected async Task<ApplicationUser?> GetUserAsync(string? userId)
    {
        return await userManager.FindByIdAsync(userId);
    }

    protected ApplicationUser CreateApplicationUser(RegisterModel model)
    {
        var fullname = GetUserFullName(model.Firstname, model.Lastname);
        var user = new ApplicationUser()
        {
            UserName = model.Username,
            FullName = fullname,
            Email = model.Email,
            HideFullName = false,
            SecurityStamp = Guid.NewGuid().ToString(),
        };
        return user;
    }

    protected UserModel MapToUserModel(ApplicationUser user, UserModel model)
    {
        (var firstname, var lastname) = GetUserFirstnameLastname(user.FullName);
        model.UserId = user.Id;
        model.UserName = user.UserName;
        model.Firstname = user.HideFullName ? string.Empty : firstname;
        model.Lastname = user.HideFullName ? string.Empty : lastname;
        return model;
    }

    protected PrivateUserModel MapToPrivateUserModel(ApplicationUser user, PrivateUserModel model)
    {
        MapToUserModel(user, model);
        (var firstname, var lastname) = GetUserFirstnameLastname(user.FullName);
        model.Firstname = firstname;
        model.Lastname = lastname;
        model.Email = user.Email;
        model.HideFirstnameLastname = user.HideFullName;
        return model;
    }

    protected async Task<LeagueUserModel> MapToLeagueUserModelAsync(ApplicationUser user, string leagueName, LeagueUserModel model)
    {
        MapToUserModel(user, model);
        model.LeagueRoles = GetLeagueRoles(leagueName, await userManager.GetRolesAsync(user));
        return model;
    }

    protected async Task<AdminUserModel> MapToAdminUserModelAsync(ApplicationUser user, AdminUserModel model)
    {
        MapToPrivateUserModel(user, model);
        model.Roles = await userManager.GetRolesAsync(user);
        return model;
    }

    protected async Task<string> GetEmailConfirmationToken(ApplicationUser user)
    {
        return await userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    protected string GenerateMailBody(ApplicationUser user, string emailConfirmationToken, string linkTemplate)
    {
        var confirmUrl = GenerateEmailConfirmationLink(user.Id, emailConfirmationToken, linkTemplate);
        var body = $"""
            <p>Hello {user.UserName},</p>
            <p>Thank you for your registration with <a href="https://irleaguemanager.net">iRLeagueManager.net</a> to bring your league results hosting to the next level!</p>
            <p>
                To finish activation of your account we only need you to confirm your email adress by clicking the link below:<br/>
                <a href="{confirmUrl}">{confirmUrl}</a>
            </p>
            <p>After you finished the confirmation you can log into your account with your username and the password that you set when you registered on the webpage.</p>
            <small>
                In case you got this mail even if you did not register with yourself on iRLeagueManager.net or any connected service, please just ignore it.<br/>
                For further questions please contact <a href="mailto:simon@irleaguemanager.net">simon@irleaguemanager.net</a><br/>
                Please do not reply to this mail.
            </small>
            """;
        return body;
    }

    protected string GenerateEmailConfirmationLink(string userId, string token, string template)
    {
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var url = template
            .Replace("{userId}", userId)
            .Replace("{token}", encodedToken);
        return url;
    }

    protected (string firstname, string lastname) GetUserFirstnameLastname(string? Fullname)
    {
        var parts = Fullname?.Split(';') ?? Array.Empty<string>();
        return (parts.ElementAtOrDefault(0) ?? string.Empty, parts.ElementAtOrDefault(1) ?? string.Empty);
    }

    protected string GetUserFullName(string firstname, string lastname)
    {
        return $"{firstname};{lastname}";
    }

    protected IEnumerable<string> GetLeagueRoles(string leagueName, IEnumerable<string> userRoles)
    {
        return userRoles
            .Where(x => LeagueRoles.IsLeagueRoleName(leagueName, x))
            .Select(x => LeagueRoles.GetRoleName(x)!)
            .ToList();
    }
}
