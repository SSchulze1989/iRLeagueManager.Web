using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Validation.Events;

public sealed class PutEventModelValidator : AbstractValidator<PutEventModel>
{
    public PutEventModelValidator(PostEventModelValidator postValidator)
    {
        Include(postValidator);
    }
}
