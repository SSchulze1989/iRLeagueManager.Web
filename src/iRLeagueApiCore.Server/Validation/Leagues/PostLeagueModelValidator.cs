using iRLeagueApiCore.Common.Models;
using System.Text.RegularExpressions;

namespace iRLeagueApiCore.Server.Validation.Leagues;

public sealed class PostLeagueModelValidator : AbstractValidator<PostLeagueModel>
{
    private readonly LeagueDbContext dbContext;

    public PostLeagueModelValidator(LeagueDbContext dbContext)
    {
        this.dbContext = dbContext;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name required")
            .Must(LeagueNameValid)
            .WithMessage("Name invalid. Can only contain: 'a-zA-Z0-9_-' and must be between 3-85 characters long")
            .MustAsync(LeagueNameNotTaken)
            .WithMessage("Name already taken. Please use another name");
    }

    private static bool LeagueNameValid(string leagueName)
    {
        return Regex.IsMatch(leagueName, @"^[a-zA-Z0-9_-]{3,85}$") // only valid characters
            && Regex.IsMatch(leagueName, @"[a-zA-Z]{1,}"); // at least one non-numeric character
    }

    private async Task<bool> LeagueNameNotTaken(string leagueName, CancellationToken cancellationToken = default)
    {
        return await dbContext.Leagues
            .AnyAsync(x => x.Name == leagueName) == false;
    }
}
