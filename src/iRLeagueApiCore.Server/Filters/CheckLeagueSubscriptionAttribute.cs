using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Responses;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace iRLeagueApiCore.Server.Filters;

internal sealed class CheckLeagueSubscriptionAttribute : ActionFilterAttribute
{
    private readonly LeagueDbContext dbContext;
    private readonly IMemoryCache memoryCache;

    public CheckLeagueSubscriptionAttribute(LeagueDbContext dbContext, IMemoryCache memoryCache)
    {
        this.dbContext = dbContext;
        this.memoryCache = memoryCache;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.RouteData.Values.TryGetValue("leagueName", out var leagueNameObject) == false)
        {
            throw new InvalidOperationException("Missing {leagueName} in action route");
        }
        var leagueName = (string)leagueNameObject!;
        var requireSubscription = context.ActionDescriptor.EndpointMetadata
            .OfType<RequireSubscriptionAttribute>()
            .Any();
        if (requireSubscription == false)
        {
            await next();
            return;
        }
        var league = await GetLeagueByName(leagueName) 
            ?? throw new InvalidOperationException("League data could not be found for given name");
        if (CheckSubscriptionStatus(league) == false)
        {
            if (league.Subscription != SubscriptionStatus.Expired)
            {
                await SetLeagueExpired(league.Id);
            }
            context.Result = new StatusCodeResult((int)HttpStatusCode.PaymentRequired);
            return;
        }
        await next();
    }

    private static bool CheckSubscriptionStatus(LeagueEntity league)
    {
        var subscriptionActive = league.Subscription switch
        {
            SubscriptionStatus.FreeTrial => league.Expires > DateTime.UtcNow,
            SubscriptionStatus.PaidPlan => league.Expires > DateTime.UtcNow,
            SubscriptionStatus.Lifetime => true,
            _ => false,
        };
        return subscriptionActive;
    }

    private async Task SetLeagueExpired(long leagueId)
    {
        var league = await dbContext.Leagues
            .FirstOrDefaultAsync(x => x.Id == leagueId)
            ?? throw new InvalidOperationException("League data could not be found for given name");
        // Check subscription status again to prevent missing updates during valid cache period
        var subscriptionActive = CheckSubscriptionStatus(league);
        if (subscriptionActive == false)
        {
            league.Subscription = SubscriptionStatus.Expired;
            await dbContext.SaveChangesAsync();
            memoryCache.Remove(CacheKeys.GetLeagueNameKey(league.Name));
        }
    }

    private async Task<LeagueEntity?> GetLeagueByName(string leagueName)
    {
        // hit cache and try to get league information without asking database
        if (memoryCache.TryGetValue(CacheKeys.GetLeagueNameKey(leagueName), out LeagueEntity cachedLeague))
        {
            return cachedLeague;
        }
        // get league from database and store in cache
        var league = await dbContext.Leagues
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == leagueName);
        if (league is not null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddSeconds(30));
            memoryCache.Set(CacheKeys.GetLeagueNameKey(leagueName), league, cacheEntryOptions);
        }
        return league;
    }
}
