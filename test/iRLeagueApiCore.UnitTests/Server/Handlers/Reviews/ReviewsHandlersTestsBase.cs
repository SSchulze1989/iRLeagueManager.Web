using iRLeagueDatabaseCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Reviews;
public abstract class ReviewsHandlersTestsBase<THandler, TRequest, TResult> :
    HandlersTestsBase<THandler, TRequest, TResult>
    where THandler : IRequestHandler<TRequest, TResult>
    where TRequest : class, IRequest<TResult>
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        var @event = dbContext.Events.First();
        await CreateScoredEventResults(@event);
        var reviews = accessMockHelper.CreateReviews(@event);
        dbContext.IncidentReviews.RemoveRange(dbContext.IncidentReviews);
        dbContext.IncidentReviews.AddRange(reviews);
        foreach(var review in reviews)
        {
            review.Comments = accessMockHelper.CreateComments(review).ToList();
            dbContext.ReviewComments.AddRange(review.Comments);
            review.AcceptedReviewVotes = accessMockHelper.AcceptedReviewVoteBuilder()
                .CreateMany()
                .ToList();
            dbContext.AcceptedReviewVotes.AddRange(review.AcceptedReviewVotes);
        }
        await dbContext.SaveChangesAsync();
    }

    protected async Task CreateScoredEventResults(EventEntity @event)
    {
        var season = await dbContext.Seasons
            .Where(x => x.Schedules.Any(y => y.Events.Any(z => z.EventId == @event.EventId)))
            .FirstAsync();
        var championships = await dbContext.Championships.ToListAsync();
        var champSeasons = championships.Select(x => accessMockHelper.CreateChampSeason(x, season)).ToList();
        dbContext.AddRange(champSeasons);
        foreach (var resultConfig in champSeasons.SelectMany(x => x.ResultConfigurations))
        {
            var result = accessMockHelper.CreateScoredResult(@event, resultConfig);
            dbContext.Add(result);
        }

        await dbContext.SaveChangesAsync();
    }
}
