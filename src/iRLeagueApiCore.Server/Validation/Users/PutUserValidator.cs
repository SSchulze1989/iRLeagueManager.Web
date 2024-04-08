using iRLeagueApiCore.Server.Handlers.Users;

namespace iRLeagueApiCore.Server.Validation.Users;

public sealed class PutUserValidator : AbstractValidator<PutUserRequest>
{
    public PutUserValidator()
    {
        RuleFor(x => x.Model.Firstname)
            .NotEmpty()
            .WithMessage("Firstname is required");

        RuleFor(x => x.Model.Lastname)
            .NotEmpty()
            .WithMessage("Lastname is required");
    }
}
