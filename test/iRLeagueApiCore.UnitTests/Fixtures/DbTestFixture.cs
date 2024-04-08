using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Server.Models;
using iRLeagueApiCore.UnitTests.Extensions;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace iRLeagueApiCore.UnitTests.Fixtures;

public sealed class DbTestFixture : DataAccessTestsBase
{
    public static string ClientUserName => "TestClient";
    public static string ClientGuid => "6a6a6e09-f4b7-4ccb-a8ae-f2fc85d897dd";
    public LeagueDbContext DbContext => dbContext;

    public DbTestFixture()
    {
    }

    public LeagueDbContext CreateDbContext()
    {
        var dbContext = accessMockHelper.CreateMockDbContext(databaseName);
        return dbContext;
    }

    public void Populate(LeagueDbContext context, Random random)
    {
        // Create random users
        var users = new List<LeagueUser>();
        for (int i = 0; i < 10; i++)
        {
            var user = LeagueUser.Empty;
            user.Id = Guid.NewGuid().ToString();
            user.Name = GetRandomName(random);
            users.Add(user);
        }
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
        var league1 = new LeagueEntity()
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
            LastModifiedByUserId = ClientGuid,
            League = league1
        };
        var season2 = new SeasonEntity()
        {
            SeasonName = "Season Two",
            CreatedOn = DateTime.Now,
            CreatedByUserName = ClientUserName,
            CreatedByUserId = ClientGuid,
            LastModifiedOn = DateTime.Now,
            LastModifiedByUserName = ClientUserName,
            LastModifiedByUserId = ClientGuid,
            League = league1
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
        // Create events on schedule1
        for (int i = 0; i < 5; i++)
        {
            var @event = new EventEntity()
            {
                Name = $"S1 Event {i + 1}",
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
            var session = new SessionEntity()
            {
                Name = "Race",
                CreatedOn = DateTime.Now,
                CreatedByUserName = ClientUserName,
                CreatedByUserId = ClientGuid,
                LastModifiedOn = DateTime.Now,
                LastModifiedByUserName = ClientUserName,
                LastModifiedByUserId = ClientGuid,
                SessionType = SessionType.Race,
                Laps = random.Next(22) + 10,
                Duration = TimeSpan.FromMinutes(random.Next(20) + 10),
            };
            @event.Sessions.Add(session);
            schedule1.Events.Add(@event);
        }
        for (int i = 0; i < 2; i++)
        {
            var @event = new EventEntity()
            {
                Name = $"S2 Event {i + 1}",
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
            var session = new SessionEntity()
            {
                Name = "Race",
                CreatedOn = DateTime.Now,
                CreatedByUserName = ClientUserName,
                CreatedByUserId = ClientGuid,
                LastModifiedOn = DateTime.Now,
                LastModifiedByUserName = ClientUserName,
                LastModifiedByUserId = ClientGuid,
                SessionType = SessionType.Race,
                Duration = TimeSpan.FromMinutes(random.Next(20) + 10),
            };
            @event.Sessions.Add(session);
            schedule2.Events.Add(@event);
        }
        context.Leagues.Add(league1);
        league1.Seasons.Add(season1);
        league1.Seasons.Add(season2);
        season1.Schedules.Add(schedule1);
        season2.Schedules.Add(schedule2);
        season2.Schedules.Add(schedule3);

        // create league2
        var league2 = new LeagueEntity()
        {
            Name = "TestLeague2",
            NameFull = "Second League for unit testing"
        };
        var l2season1 = new SeasonEntity()
        {
            SeasonName = "L2 Season One",
            CreatedOn = DateTime.Now,
            CreatedByUserName = ClientUserName,
            CreatedByUserId = ClientGuid,
            LastModifiedOn = DateTime.Now,
            LastModifiedByUserName = ClientUserName,
            LastModifiedByUserId = ClientGuid,
            League = league1
        };
        var l2schedule1 = new ScheduleEntity()
        {
            Name = "L2S1 Schedule 1",
            CreatedOn = DateTime.Now,
            CreatedByUserName = ClientUserName,
            CreatedByUserId = ClientGuid,
            LastModifiedOn = DateTime.Now,
            LastModifiedByUserName = ClientUserName,
            LastModifiedByUserId = ClientGuid
        };

        context.Leagues.Add(league2);
        league2.Seasons.Add(l2season1);
        l2season1.Schedules.Add(l2schedule1);

        GenerateMembers(context, random);

        // assign members to league
        var members = context.Members
            .Local
            .ToList();
        var leagues = context.Leagues
            .Local
            .ToList();
        foreach (var member in members)
        {
            foreach (var league in leagues)
            {
                var leagueMember = new LeagueMemberEntity()
                {
                    Member = member,
                };
                league.LeagueMembers.Add(leagueMember);
            }
        }

        // create results
        var resultConfig = new ResultConfigurationEntity()
        {
            Name = "Resultconfig 1",
            DisplayName = "Overall",
            ResultsPerTeam = random.Next(1, 10),
        };
        var scoring = new ScoringEntity()
        {
            Name = "Scoring 1",
            ShowResults = true
        };
        var pointRule = new PointRuleEntity()
        {
            Name = "Pointrule 1",
            BonusPoints = new BonusPointModel[]
            {
                new() { Type = BonusPointType.Position, Value = 1, Points = 5 },
                new() { Type = BonusPointType.Position, Value = 2, Points = 3 },
                new() { Type = BonusPointType.Position, Value = 3, Points = 1 },
            },
            FinalSortOptions = new SortOptions[] { SortOptions.TotalPtsAsc, SortOptions.PenPtsDesc },
            MaxPoints = 30,
            PointDropOff = 1,
            PointsPerPlace = new int[] { 5, 4, 3, 2, 1 },
            PointsSortOptions = new SortOptions[] { SortOptions.PosAsc, SortOptions.IntvlAsc },
        };
        resultConfig.Scorings.Add(scoring);
        league1.ResultConfigs.Add(resultConfig);
        league1.PointRules.Add(pointRule);

        foreach (var @event in league1.Seasons
            .SelectMany(x => x.Schedules)
            .SelectMany(x => x.Events)
            .SkipLast(1))
        {
            var eventResult = new EventResultEntity();
            var scoredResult = new ScoredEventResultEntity()
            {
                Name = resultConfig.DisplayName,
            };
            foreach (var session in @event.Sessions)
            {
                var sessionResult = new SessionResultEntity();
                sessionResult.Session = session;
                eventResult.SessionResults.Add(sessionResult);
                var scoredSessionResult = new ScoredSessionResultEntity();
                scoredSessionResult.Name = session.Name;
                scoredResult.ScoredSessionResults.Add(scoredSessionResult);
                for (int i = 0; i < 10; i++)
                {
                    var resultRow = new ResultRowEntity()
                    {
                        StartPosition = i + 1,
                        FinishPosition = i + 1,
                        Member = members.ElementAt(i),
                        QualifyingTime = GetTimeSpan(random),
                        FastestLapTime = GetTimeSpan(random),
                        AvgLapTime = GetTimeSpan(random),
                        Interval = GetTimeSpan(random),
                    };
                    sessionResult.ResultRows.Add(resultRow);
                    var scoredResultRow = new ScoredResultRowEntity(resultRow)
                    {
                        FinalPosition = i + 1,
                        RacePoints = 10 - i,
                        TotalPoints = 10 - i
                    };
                    scoredSessionResult.ScoredResultRows.Add(scoredResultRow);
                }
            }
            @event.EventResult = eventResult;
            @event.ScoredEventResults.Add(scoredResult);
        }

        // Create reviews
        for (int i = 0; i < 3; i++)
        {
            var cat = new VoteCategoryEntity()
            {
                Index = i,
                Text = $"Cat {i + 1}",
                DefaultPenalty = i + 1,
            };
            league1.VoteCategories.Add(cat);
        }
        foreach (var session in league1.Seasons
            .SelectMany(x => x.Schedules)
            .SelectMany(x => x.Events)
            .SelectMany(x => x.Sessions))
        {
            var involvedMembers = new[]
            {
                    members.Random(random),
                    members.Random(random),
                    members.Random(random),
                }.Distinct().ToList();
            var review = CreateVersion(new IncidentReviewEntity()
            {
                AuthorName = GetRandomName(random),
                AuthorUserId = Guid.NewGuid().ToString(),
                Corner = random.Next(1, 12).ToString(),
                FullDescription = "Full Description",
                IncidentKind = "Incident Kind",
                IncidentNr = random.Next(1, 12).ToString(),
                InvolvedMembers = involvedMembers,
                OnLap = random.Next(1, 12).ToString(),
                TimeStamp = TimeSpan.FromMinutes(1),
                ResultLongText = "Long text for much more details on the result",
                Comments = new[] { RandomComment(random, users.Random(random), involvedMembers), RandomComment(random, users.Random(random), involvedMembers) }.ToList(),
                AcceptedReviewVotes = new[] {new AcceptedReviewVoteEntity()
                    {
                        MemberAtFault = involvedMembers.Random(random),
                        Description = "Description",
                    } }.ToList(),
            }, users.Random(random));
            session.IncidentReviews.Add(review);
        }
    }

    private static TimeSpan GetTimeSpan(Random random)
    {
        return new TimeSpan(0, 1, 2, 34, 567);
    }
    private void GenerateMembers(LeagueDbContext context, Random random)
    {
        var minMemberCount = 50;
        var maxMemberCount = 100;

        var memberCount = random.Next(maxMemberCount - minMemberCount + 1) + minMemberCount;

        for (int i = 0; i < memberCount; i++)
        {
            var member = new MemberEntity()
            {
                Firstname = GetRandomName(random),
                Lastname = GetRandomName(random),
                IRacingId = GetRandomIracingId(random)
            };
            context.Members.Add(member);
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

    private string GetRandomIracingId(Random random)
    {
        return fixture.Create<long>().ToString();
    }

    private static ReviewCommentEntity RandomComment(Random random, LeagueUser user, IEnumerable<MemberEntity> involvedMembers)
    {
        var comment = CreateVersion(new ReviewCommentEntity()
        {
            AuthorName = user.Name,
            AuthorUserId = user.Id,
            Date = DateTime.Now,
            Text = "Random Comment Text: " + GetRandomName(random),
            ReviewCommentVotes = new[] { new ReviewCommentVoteEntity()
                {
                    MemberAtFault = involvedMembers.Random(random),
                    Description = "Description",
                } }.ToList(),
        }, user);
        return comment;
    }

    private static T CreateVersion<T>(T entity, LeagueUser user) where T : IVersionEntity
    {
        entity.Version = 1;
        entity.CreatedByUserId = user.Id;
        entity.CreatedByUserName = user.Name;
        entity.CreatedOn = DateTime.Now;
        entity.LastModifiedByUserId = user.Id;
        entity.LastModifiedByUserName = user.Name;
        entity.LastModifiedOn = DateTime.Now;
        return entity;
    }

    public void Dispose()
    {
    }
}
