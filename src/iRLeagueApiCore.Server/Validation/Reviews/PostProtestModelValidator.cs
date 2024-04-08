using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Validation.Reviews;

public class PostProtestModelValidator : AbstractValidator<PostProtestModel>
{
    private readonly LeagueDbContext dbContext;

    public PostProtestModelValidator(LeagueDbContext dbContext)
    {
        this.dbContext = dbContext;

        RuleFor(x => x.AuthorMemberId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Author is required")
            .MustAsync(MemberExists)
            .WithMessage("Member id does not exist");
        RuleFor(x => x.ConfirmIRacingId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("IRacing id is required to confirm identity")
            .MustAsync(IRacingIdMatchAuthor)
            .WithMessage("IRacing id does not match selected member");
        RuleFor(x => x.OnLap)
            .NotEmpty()
            .WithMessage("Lap is required");
        RuleFor(x => x.Corner)
            .NotEmpty()
            .WithMessage("Turn is required");
        RuleFor(x => x.FullDescription)
            .NotEmpty()
            .WithMessage("Description is required");
        RuleFor(x => x.InvolvedMembers)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("At least one involved Driver is required")
            .ForEach(x => x.MustAsync(MemberExists))
            .WithMessage("One or more members are invalid");
    }

    public async Task<bool> MemberExists(MemberInfoModel member, CancellationToken cancellationToken)
    {
        return await MemberExists(member.MemberId, cancellationToken);
    }

    public async Task<bool> MemberExists(long memberId, CancellationToken cancellationToken)
    {
        return await dbContext.Members
            .AnyAsync(x => x.Id == memberId, cancellationToken);
    }

    public async Task<bool> IRacingIdMatchAuthor(PostProtestModel protest, string iRacingId, CancellationToken cancellationToken)
    {
        var member = await dbContext.Members
            .Where(x => x.Id == protest.AuthorMemberId)
            .FirstOrDefaultAsync(cancellationToken);
        return member?.IRacingId == iRacingId;
    }
}
