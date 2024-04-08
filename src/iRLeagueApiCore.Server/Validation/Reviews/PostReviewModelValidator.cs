using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Services.ResultService.Extensions;

namespace iRLeagueApiCore.Server.Validation.Reviews;

public sealed class PostReviewModelValidator : AbstractValidator<PostReviewModel>
{
    private readonly LeagueDbContext dbContext;

    public PostReviewModelValidator(LeagueDbContext dbContext)
    {
        this.dbContext = dbContext;

        RuleFor(x => x.InvolvedMembers)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("At least one driver or team required")
            .When(x => x.InvolvedTeams.None())
            .MustAsync(EachMemberIsValid)
            .WithMessage("Member does not exist")
            .When(x => x.InvolvedMembers.Any());
        RuleFor(x => x.InvolvedTeams)
            .NotEmpty()
            .WithMessage("At least one driver or team required")
            .When(x => x.InvolvedMembers.None());
        RuleFor(x => x.IncidentKind)
            .NotEmpty()
            .WithMessage("Incident Kind is required");
    }

    private async Task<bool> EachMemberIsValid(IEnumerable<MemberInfoModel> members, CancellationToken cancellationToken)
    {
        var memberIds = members.Select(x => x.MemberId).ToList();
        return await dbContext.Members
            .AnyAsync(x => memberIds.Contains(x.Id), cancellationToken);
    }
}
