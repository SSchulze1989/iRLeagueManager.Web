using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal abstract class DatabaseAccessBase
{
    protected readonly LeagueDbContext dbContext;

    public DatabaseAccessBase(LeagueDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task SetLeague(long eventId, CancellationToken cancellationToken)
    {
        var @event = await dbContext.Events
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.EventId == eventId, cancellationToken);
        if (@event == null)
        {
            return;
        }
        dbContext.LeagueProvider.SetLeague(@event.LeagueId);
    }
}
