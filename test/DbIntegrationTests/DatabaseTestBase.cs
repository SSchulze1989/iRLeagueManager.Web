using iRLeagueDatabaseCore.Models;
using iRLeagueDatabaseCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using AutoFixture;
using System.Linq;

namespace DbIntegrationTests;
public class DatabaseTestBase
{
    static IConfiguration _config;
    protected static readonly int Seed = 12345;
    protected long CurrentLeagueId { get; set; }
    protected readonly Mock<ILeagueProvider> mockLeagueProvider = new();
    protected Fixture Fixture { get; } = new Fixture();
    protected LeagueDbContext DbContext { get; }

    static DatabaseTestBase()
    {
        var random = new Random(Seed);
        _config = new ConfigurationBuilder()
            .AddUserSecrets<DbIntegrationTests>()
            .Build();

        // Setup database
        var leagueProvider = Mock.Of<ILeagueProvider>();
        using var dbContext = GetStaticTestDatabaseContext(leagueProvider);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        PopulateTestDatabase.Populate(dbContext, random);
        dbContext.SaveChanges();
    }

    public DatabaseTestBase()
    {
        mockLeagueProvider.Setup(x => x.LeagueId).Returns(() => CurrentLeagueId);
        DbContext = GetTestDatabaseContext();
        CurrentLeagueId = DbContext.Leagues.First().Id;
    }

    protected static LeagueDbContext GetStaticTestDatabaseContext(ILeagueProvider leagueProvider)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LeagueDbContext>();
        optionsBuilder.UseMySQL(_config.GetConnectionString("ModelDb"))
            .UseLazyLoadingProxies();
        optionsBuilder.EnableSensitiveDataLogging();
        var dbContext = new LeagueDbContext(optionsBuilder.Options, leagueProvider);
        return dbContext;
    }

    protected LeagueDbContext GetTestDatabaseContext()
    {
        return GetStaticTestDatabaseContext(mockLeagueProvider.Object);
    }
}
