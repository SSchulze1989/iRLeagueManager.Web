using iRLeagueApiCore.Server.Handlers.Users;

namespace iRLeagueApiCore.Server.Validation.Users;

public sealed class AddLeagueRoleRequestValidator : AbstractValidator<AddLeagueRoleRequest>
{
    public AddLeagueRoleRequestValidator()
    {
        RuleFor(x => x.RoleName)
            .Must(LeagueRoles.IsValidRole)
            .WithMessage(request =>
                $"Invalid value. Valid roles: [{string.Join<LeagueRoleValue>(", ", LeagueRoles.RolesAvailable)}]");
    }
}
