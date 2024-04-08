using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore.Design;
using Moq;

namespace iRLeagueDatabaseCore;

internal class MigrationLeagueDbContextFactory : IDesignTimeDbContextFactory<LeagueDbContext>
{
    public LeagueDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("EFCORETOOLSDB")
            ?? throw new InvalidOperationException("No connection string for migration provided. Please set $env:EFCORETOOLSDB");
        var optionsBuilder = new DbContextOptionsBuilder<LeagueDbContext>();
        optionsBuilder.UseMySQL(connectionString);
        var leagueProvider = Mock.Of<ILeagueProvider>();

        var dbContext = new LeagueDbContext(optionsBuilder.Options, leagueProvider);
        return dbContext;
    }
}
