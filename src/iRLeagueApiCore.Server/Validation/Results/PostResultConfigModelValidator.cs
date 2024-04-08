using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Validation.Results;

public sealed class PostResultConfigModelValidator : AbstractValidator<PostResultConfigModel>
{
    public PostResultConfigModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Name must not be empty");
        //RuleFor(x => x.DisplayName).NotEmpty();
    }
}
