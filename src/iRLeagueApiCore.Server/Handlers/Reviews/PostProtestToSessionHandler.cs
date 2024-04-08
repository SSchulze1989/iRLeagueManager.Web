using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record PostProtestToSessionRequest(long SessionId, LeagueUser User, PostProtestModel Model) : IRequest<ProtestModel>;

public class PostProtestToSessionHandler : ProtestsHandlerBase<PostProtestToSessionHandler, PostProtestToSessionRequest>, 
    IRequestHandler<PostProtestToSessionRequest, ProtestModel>
{
    public PostProtestToSessionHandler(ILogger<PostProtestToSessionHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PostProtestToSessionRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<ProtestModel> Handle(PostProtestToSessionRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var postProtest = await CreateProtestEntity(request.User, request.SessionId, cancellationToken);
        postProtest = await MapToProtestEntity(request.User, request.Model, postProtest, cancellationToken);
        dbContext.Protests.Add(postProtest);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getProtest = await MapToProtestModel(postProtest.ProtestId, includeAuthor: true, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Could not find created resource");
        return getProtest;
    }

    private async Task<ProtestEntity> CreateProtestEntity(LeagueUser user, long sessionId, CancellationToken cancellationToken)
    {
        var session = await dbContext.Sessions
            .Where(x => x.SessionId == sessionId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ResourceNotFoundException();
        var protest = new ProtestEntity()
        {
            LeagueId = session.LeagueId,
            Session = session,
        };
        return protest;
    }
}
