using iRLeagueApiCore.Server.Handlers.Users;

namespace iRLeagueApiCore.Server.Validation.Users;

public sealed class RemoveLeagueRoleRequestValidator : AbstractValidator<RemoveLeagueRoleRequest>
{
    public RemoveLeagueRoleRequestValidator()
    {
        RuleFor(x => x.RoleName)
            .Must(LeagueRoles.IsValidRole)
            .WithMessage(request =>
                $"Invalid value. Valid roles: [{string.Join<LeagueRoleValue>(", ", LeagueRoles.RolesAvailable)}]");
    }
}
