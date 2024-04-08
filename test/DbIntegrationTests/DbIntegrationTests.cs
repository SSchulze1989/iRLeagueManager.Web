using FluentAssertions;
using iRLeagueApiCore.Common.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;
using Xunit.Abstractions;

namespace DbIntegrationTests;

[Collection("DbIntegration")]
public class DbIntegrationTests : DatabaseTestBase
{
    public DbIntegrationTests(ITestOutputHelper output)
    {
        output.WriteLine($"Randomizer seed: {Seed}");
    }

    [Fact]
    public void ShouldPopulate()
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
    public async Task ShouldDeleteLeage()
    {
        using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        using var dbContext = GetTestDatabaseContext();

        var league = await dbContext.Leagues
            .FirstAsync();
        dbContext.Leagues.Remove(league);
    }

    [Fact]
    public void CreateLeague()
    {
        using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            const string leagueName = "TestCreateLeague";
            using (var dbContext = GetTestDatabaseContext())
            {
                var league = new LeagueEntity()
                {
                    Name = leagueName,
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
                var league = dbContext.Leagues.OrderBy(x => x.Id).Last();
                CurrentLeagueId = league.Id;
                Assert.Equal(leagueName, league.Name);
                Assert.Equal(1, league.Seasons.Count);
                Assert.Equal(league, league.Seasons.First().League);
            }
        }
    }

    [Fact]
    public void ShouldUseLazyLoading()
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
    public void ShouldNotUseLazyLoading_WhenDisabled()
    {
        using (var dbContext = GetTestDatabaseContext())
        {
            dbContext.ChangeTracker.LazyLoadingEnabled = false;
            var league = dbContext.Leagues.First();
            CurrentLeagueId = league.Id;
            Assert.NotNull(league);
            Assert.Equal(0, league.Seasons.Count);
        }
    }

    [Fact]
    public void ShouldUseEagerLoading_WhenLazyLoadingDisabled()
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
            dbContext.Seasons.Load();

            Assert.NotNull(league);
            Assert.Equal(2, league.Seasons.Count);
        }
    }

    [Fact]
    public async Task ShouldConvertTimeSpanPrecision()
    {
        TimeSpan testTimeSpan = TimeSpan.FromMinutes(1.23);

        using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        using (var context = GetTestDatabaseContext())
        {
            CurrentLeagueId = (await context.Leagues.FirstAsync()).Id;
            var session = await context.Sessions.FirstAsync();
            session.Duration = testTimeSpan;
            context.SaveChanges();
        }

        using (var context = GetTestDatabaseContext())
        {
            var session = await context.Sessions.FirstAsync();
            Assert.Equal(testTimeSpan, session.Duration);
        }
    }

    [Fact]
    public async Task LeagueShouldHaveScorings()
    {
        using var context = GetTestDatabaseContext();
        SetCurrentLeague(await context.Leagues.FirstAsync());
        var league = await context.Leagues
            .Include(x => x.Scorings)
            .FirstAsync();
        Assert.NotEmpty(league.Scorings);
    }

    [Fact]
    public async Task ShouldAddFilterToConfiguration()
    {
        using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        using (var context = GetTestDatabaseContext())
        {
            SetCurrentLeague(await context.Leagues.FirstAsync());
            var filterOption = new FilterOptionEntity();
            var config = await context.ResultConfigurations.FirstAsync();
            config.PointFilters.Add(filterOption);
            await context.SaveChangesAsync();
        }

        using (var context = GetTestDatabaseContext())
        {
            var config = await context.ResultConfigurations.FirstAsync();
            Assert.NotEmpty(config.PointFilters);
        }
    }

    [Fact]
    public async Task ShouldCascadeDeleteFilter()
    {
        using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        using var context = GetTestDatabaseContext();

        SetCurrentLeague(await context.Leagues.FirstAsync());
        var filterOption = new FilterOptionEntity();
        var config = await context.ResultConfigurations.FirstAsync();
        config.PointFilters.Add(filterOption);
        await context.SaveChangesAsync();

        config.PointFilters.Remove(filterOption);
        var filterOptionEntry = context.Entry(filterOption);
        await context.SaveChangesAsync();

        Assert.Equal(EntityState.Detached, filterOptionEntry.State);
    }

    [Fact]
    public async Task ShouldSetFilterValues()
    {
        using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        using var context = GetTestDatabaseContext();

        SetCurrentLeague(await context.Leagues.FirstAsync());
        var filterOption = new FilterOptionEntity()
        {
            Conditions = new[] {
                new FilterConditionModel() { FilterValues = new[] { "Value1", "Value2" } },
            },
        };
        var config = await context.ResultConfigurations.FirstAsync();
        config.PointFilters.Add(filterOption);
        await context.SaveChangesAsync();
        var testFilterOption = await context.FilterOptions
            .SingleAsync(x => x.FilterOptionId == filterOption.FilterOptionId);

        testFilterOption.Conditions.First().FilterValues.Should().BeEquivalentTo(filterOption.Conditions.First().FilterValues);
    }

    [Fact]
    public async Task ShouldConsiderMultiTenancyWithCorrectTenant()
    {
        using var context = GetTestDatabaseContext();
        var league = await context.Leagues.FirstAsync();
        var trueSeasonCount = await context.Seasons
            .IgnoreQueryFilters()
            .Where(x => x.LeagueId == league.Id)
            .CountAsync();
        SetCurrentLeague(league);

        var seasons = await context.Seasons.ToListAsync();

        seasons.Should().HaveCount(trueSeasonCount);
    }

    [Fact]
    public async Task ShouldConsiderMultiTenancyWithDifferentTenant()
    {
        using var context = GetTestDatabaseContext();
        CurrentLeagueId = 0;
        var league = await context.Leagues.FirstAsync();

        var seasons = await context.Seasons.ToListAsync();

        seasons.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldAddAutoPenalty()
    {
        //using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        using (var context = GetTestDatabaseContext())
        {
            SetCurrentLeague(await context.Leagues.FirstAsync());
            var autoPenalty = new AutoPenaltyConfigEntity()
            {
                Conditions =
                {
                    new() {
                        FilterType = iRLeagueApiCore.Common.Enums.FilterType.ColumnProperty,
                        FilterValues = { "4" },
                        ColumnPropertyName = "Incidents",
                        Comparator = iRLeagueApiCore.Common.Enums.ComparatorType.ForEach,
                        Action = iRLeagueApiCore.Common.Enums.MatchedValueAction.Remove,
                    },
                },
                Description = "Test Penalty",
                Points = 1,
                Time = new TimeSpan(1, 2, 3),
                Positions = 2,
                Type = iRLeagueApiCore.Common.Enums.PenaltyType.Time,
            };
            var pointRule = await context.PointRules.FirstAsync();
            pointRule.AutoPenalties.Add(autoPenalty);
            await context.SaveChangesAsync();
        }

        using (var context = GetTestDatabaseContext())
        {
            SetCurrentLeague(await context.Leagues.FirstAsync());
            var autoPenalty = context.AutoPenaltyConfigs.First();
            autoPenalty.Description.Should().Be("Test Penalty");
            autoPenalty.Points.Should().Be(1);
            autoPenalty.Positions.Should().Be(2);
            autoPenalty.Time.Should().Be(new TimeSpan(1, 2, 3));
            autoPenalty.Type.Should().Be(iRLeagueApiCore.Common.Enums.PenaltyType.Time);
            autoPenalty.Conditions.Should().HaveCount(1);
            autoPenalty.Conditions.First().FilterType.Should().Be(iRLeagueApiCore.Common.Enums.FilterType.ColumnProperty);
            autoPenalty.Conditions.First().Comparator.Should().Be(iRLeagueApiCore.Common.Enums.ComparatorType.ForEach);
            autoPenalty.Conditions.First().ColumnPropertyName.Should().Be("Incidents");
            autoPenalty.Conditions.First().FilterValues.Should().BeEquivalentTo(new[] { "4" });
            autoPenalty.Conditions.First().Action.Should().Be(iRLeagueApiCore.Common.Enums.MatchedValueAction.Remove);
        }
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