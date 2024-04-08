using iRLeagueApiCore.Server.Models;
using iRLeagueDatabaseCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace iRLeagueApiCore.Server.Filters;

/// <summary>
/// Automatically insert the league id corresponding the league name in route parameters
/// <para>
/// Requires <b>"{leagueName}"</b> in Route
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
internal sealed class SetTenantLeagueIdAttribute : ActionFilterAttribute
{
    private readonly LeagueDbContext _dbContext;
    private readonly RequestLeagueProvider leagueProvider;
    private readonly IMemoryCache memoryCache;
    public SetTenantLeagueIdAttribute(LeagueDbContext dbContext, RequestLeagueProvider leagueProvider, IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        this.leagueProvider = leagueProvider;
        this.memoryCache = memoryCache;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.RouteData.Values.TryGetValue("leagueName", out var leagueNameObject) == false)
        {
            throw new InvalidOperationException("Missing {leagueName} in action route");
        }
        var leagueName = (string)leagueNameObject!;
        var leagueId = await GetLeagueIdByName(leagueName);

        if (leagueId == 0)
        {
            context.Result = new NotFoundObjectResult($"League {leagueName} does not exist");
            return;
        }

        context.ActionArguments.Add("leagueId", leagueId);
        leagueProvider.SetLeague(leagueId, leagueName);

        await base.OnActionExecutionAsync(context, next);
    }

    private async Task<long> GetLeagueIdByName(string leagueName)
    {
        // hit cache and try to get league information without asking database
        if (memoryCache.TryGetValue(CacheKeys.GetLeagueNameKey(leagueName), out LeagueEntity cachedLeague))
        {
            return cachedLeague.Id;
        }
        // get league from database and store in cache
        var league = await _dbContext.Leagues
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == leagueName);
        if (league is null)
        {
            return 0;
        }
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.UtcNow.AddSeconds(30));
        memoryCache.Set(CacheKeys.GetLeagueNameKey(leagueName), league, cacheEntryOptions);
        return league.Id;
    }
}
