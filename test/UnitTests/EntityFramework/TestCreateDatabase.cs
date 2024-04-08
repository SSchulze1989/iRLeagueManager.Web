using iRLeagueApiCore.Common.Enums;
using iRLeagueDatabaseCore;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.EntityFramework;

public class TestCreateDatabase : IDisposable
{
    static string ClientUserName => "TestClient";
    static string ClientGuid => "6a6a6e09-f4b7-4ccb-a8ae-f2fc85d897dd";

    private static readonly int Seed = 12345;

    private long CurrentLeagueId { get; set; }
    private readonly Mock<ILeagueProvider> mockLeagueProvider = new();

    public TestCreateDatabase(ITestOutputHelper output)
    {
        output.WriteLine($"Randomizer seed: {Seed}");
        mockLeagueProvider.Setup(x => x.LeagueId).Returns(() => CurrentLeagueId);
    }

    static TestCreateDatabase()
    {
        var random = new Random(Seed);

        // Setup database
        var mockLeagueProvider = new Mock<ILeagueProvider>();
        using (var dbContext = GetStaticTestDatabaseContext(mockLeagueProvider.Object))
        {
            Populate(dbContext, random);
            dbContext.SaveChanges();
        }
    }

    private static LeagueDbContext GetStaticTestDatabaseContext(ILeagueProvider leagueProvider)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LeagueDbContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "TestDatabase")
           .UseLazyLoadingProxies();
        var dbContext = new LeagueDbContext(optionsBuilder.Options, leagueProvider);
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    private LeagueDbContext GetTestDatabaseContext()
    {
        return GetStaticTestDatabaseContext(mockLeagueProvider.Object);
    }

    public void Dispose()
    {

    }

    [Fact]
    public void TestPopulate()
    {
        using (var dbContext = GetTestDatabaseContext())
        {
            var league = dbContext.Leagues.FirstOrDefault();
            SetCurrentLeague(league);
            Assert.NotNull(league);
            Assert.Equal("TestLeague", league.Name);
            Assert.Equal(2, league.Seasons.Count());

            // validate structure
            foreach (var season in league.Seasons)
            {
                Assert.Equal(league, season.League);
                Assert.Equal(league.Id, season.LeagueId);
            }

            var seasonSchedules = league.Seasons.SelectMany(x => x.Schedules.Select(y => (x, y)));
            foreach ((var season, var schedule) in seasonSchedules)
            {
                Assert.Equal(season, schedule.Season);
                Assert.Equal(league.Id, schedule.LeagueId);
            }
        }
    }

    [Fact]
    public void TestCreateLeague()
    {
        using (var dbContext = GetTestDatabaseContext())
        {
            var league = new LeagueEntity()
            {
                Name = "TestLeague2",
                NameFull = "2nd League for unit testing"
            };
            dbContext.Leagues.Add(league);
            var season = new SeasonEntity()
            {
                SeasonName = "TestSeason",
                CreatedOn = DateTime.Now,
                CreatedByUserName = "TestUser",
                CreatedByUserId = "1"
            };
            league.Seasons.Add(season);

            dbContext.SaveChanges();
        }

        using (var dbContext = GetTestDatabaseContext())
        {
            Assert.Equal(2, dbContext.Leagues.Count());
            var league = dbContext.Leagues.OrderBy(x => x.Id).Last();
            SetCurrentLeague(league);
            Assert.Equal("TestLeague2", league.Name);
            Assert.Equal(1, league.Seasons.Count);
            Assert.Equal(league, league.Seasons.First().League);
        }

        // clean up after testing

        using (var dbContext = GetTestDatabaseContext())
        {
            var league = dbContext.Leagues.OrderBy(x => x.Id).Last();
            dbContext.Leagues.Remove(league);
            dbContext.SaveChanges();
        }
    }

    [Fact]
    public void TestLazyLoading()
    {
        using (var dbContext = GetTestDatabaseContext())
        {
            var league = dbContext.Leagues.First();
            SetCurrentLeague(league);
            Assert.NotNull(league);
            Assert.Equal(2, league.Seasons.Count);
        }
    }

    [Fact]
    public void TestLazyLoadingDisabled()
    {
        using (var dbContext = GetTestDatabaseContext())
        {
            dbContext.ChangeTracker.LazyLoadingEnabled = false;
            var league = dbContext.Leagues.First();
            Assert.NotNull(league);
            Assert.Equal(0, league.Seasons.Count);
        }
    }

    [Fact]
    public void TestEagerLoading()
    {
        using (var dbContext = GetTestDatabaseContext())
        {
            dbContext.ChangeTracker.LazyLoadingEnabled = false;
            SetCurrentLeague(dbContext.Leagues.First());
            var league = dbContext.Leagues
                .Include(e => e.Seasons)
                .First();
            Assert.NotNull(league);
            Assert.Equal(2, league.Seasons.Count);
        }

        using (var dbContext = GetTestDatabaseContext())
        {
            dbContext.ChangeTracker.LazyLoadingEnabled = false;
            var league = dbContext.Leagues.First();
            SetCurrentLeague(league);
            dbContext.Seasons.Load();

            Assert.NotNull(league);
            Assert.Equal(2, league.Seasons.Count);
        }
    }

    private static void Populate(LeagueDbContext context, Random random)
    {
        // Populate Tracks
        for (int i = 0; i < 2; i++)
        {
            var group = new TrackGroupEntity()
            {
                TrackName = $"Group{i}",
                Location = "Testlocation"
            };
            for (int j = 0; j < 3; j++)
            {
                var config = new TrackConfigEntity()
                {
                    ConfigName = $"Config{i}",
                    ConfigType = ConfigType.Road,
                    Turns = j * 3,
                    LengthKm = j * 1.0,
                    HasNightLighting = false
                };
                group.TrackConfigs.Add(config);
            }
            context.TrackGroups.Add(group);
        }

        // create models
        var league = new LeagueEntity()
        {
            Name = "TestLeague",
            NameFull = "League for unit testing"
        };
        var season1 = new SeasonEntity()
        {
            SeasonName = "Season One",
            CreatedOn = DateTime.Now,
            CreatedByUserName = ClientUserName,
            CreatedByUserId = ClientGuid,
            LastModifiedOn = DateTime.Now,
            LastModifiedByUserName = ClientUserName,
            LastModifiedByUserId = ClientGuid
        };
        var season2 = new SeasonEntity()
        {
            SeasonName = "Season Two",
            CreatedOn = DateTime.Now,
            CreatedByUserName = ClientUserName,
            CreatedByUserId = ClientGuid,
            LastModifiedOn = DateTime.Now,
            LastModifiedByUserName = ClientUserName,
            LastModifiedByUserId = ClientGuid
        };
        var schedule1 = new ScheduleEntity()
        {
            Name = "S1 Schedule",
            CreatedOn = DateTime.Now,
            CreatedByUserName = ClientUserName,
            CreatedByUserId = ClientGuid,
            LastModifiedOn = DateTime.Now,
            LastModifiedByUserName = ClientUserName,
            LastModifiedByUserId = ClientGuid
        };
        var schedule2 = new ScheduleEntity()
        {
            Name = "S2 Schedule 1",
            CreatedOn = DateTime.Now,
            CreatedByUserName = ClientUserName,
            CreatedByUserId = ClientGuid,
            LastModifiedOn = DateTime.Now,
            LastModifiedByUserName = ClientUserName,
            LastModifiedByUserId = ClientGuid
        };
        var schedule3 = new ScheduleEntity()
        {
            Name = "S2 Schedule 2",
            CreatedOn = DateTime.Now,
            CreatedByUserName = ClientUserName,
            CreatedByUserId = ClientGuid,
            LastModifiedOn = DateTime.Now,
            LastModifiedByUserName = ClientUserName,
            LastModifiedByUserId = ClientGuid
        };
        // Create sessions on schedule1
        for (int i = 0; i < 5; i++)
        {
            var session = new EventEntity()
            {
                Name = $"S1 Session {i}",
                CreatedOn = DateTime.Now,
                CreatedByUserName = ClientUserName,
                CreatedByUserId = ClientGuid,
                LastModifiedOn = DateTime.Now,
                LastModifiedByUserName = ClientUserName,
                LastModifiedByUserId = ClientGuid,
                Track = context.TrackGroups
                    .SelectMany(x => x.TrackConfigs)
                    .Skip(i)
                    .FirstOrDefault(),
            };
            schedule1.Events.Add(session);
        }
        league.Seasons.Add(season1);
        league.Seasons.Add(season2);
        season1.Schedules.Add(schedule1);
        season2.Schedules.Add(schedule2);
        season2.Schedules.Add(schedule3);
        context.Leagues.Add(league);

        GenerateMembers(context, random);

        // assign members to league
        foreach (var member in context.Members)
        {
            var leagueMember = new LeagueMemberEntity()
            {
                Member = member,
                League = league
            };
            context.Set<LeagueMemberEntity>().Add(leagueMember);
        }
    }

    private static void GenerateMembers(LeagueDbContext context, Random random)
    {
        var minMemberCount = 50;
        var maxMemberCount = 100;

        var memberCount = random.Next(maxMemberCount - minMemberCount + 1) + minMemberCount;
        var members = context.Set<MemberEntity>();

        for (int i = 0; i < memberCount; i++)
        {
            var member = new MemberEntity()
            {
                Firstname = GetRandomName(random),
                Lastname = GetRandomName(random),
                IRacingId = GetRandomIracingId(random)
            };
            members.Add(member);
        }
    }

    private static string GetRandomName(Random random)
    {
        var minLen = 3;
        var len = random.Next(10) + minLen;
        char[] name = new char[len];
        char[] characters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789ßÄÜÖäüö".ToCharArray();

        for (int i = 0; i < len; i++)
        {
            var offset = random.Next(characters.Length);
            name[i] = characters[offset];
        }
        return new string(name);
    }

    private static string GetRandomIracingId(Random random)
    {
        var len = 6;
        char[] id = new char[len];
        for (int i = 0; i < len; i++)
        {
            id[i] = (char)('0' + random.Next(10));
        }
        return new string(id);
    }

    [Fact]
    public void TestJustThisOneThing()
    {
        Assert.True(true);
    }

    /// <summary>
    /// Set the current league id for multi tenancy - must be called before any league specific entity is queried
    /// </summary>
    /// <param name="league"></param>
    private void SetCurrentLeague(LeagueEntity league)
    {
        CurrentLeagueId = league.Id;
    }
}
