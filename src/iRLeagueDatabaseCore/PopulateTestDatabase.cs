using iRLeagueApiCore.Common.Models;
using iRLeagueDatabaseCore.Models;

namespace DbIntegrationTests;

public static class PopulateTestDatabase
{
    public static string ClientUserName => "TestClient";
    public static string ClientGuid => "6a6a6e09-f4b7-4ccb-a8ae-f2fc85d897dd";

    public static void Populate(LeagueDbContext context, Random random)
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
                EventType = EventType.SingleRace
            };
            var subSession = new SessionEntity()
            {
                Name = "Race",
                SessionType = SessionType.Race,
            };
            @event.Sessions.Add(subSession);
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
                EventType = EventType.SingleRace
            };
            var session = new SessionEntity()
            {
                Name = "Race",
                SessionType = SessionType.Race,
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

        var members = context.Members
            .Local
            .ToList();
        var leagues = context.Leagues
            .Local
            .ToList();
        // assign members to league
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

        var pointsRule = new PointRuleEntity()
        {
            Name = "Points Rule",
            PointsPerPlace = new List<int>() { 5, 4, 3, 2, 1 },
            BonusPoints = new List<BonusPointModel>()
            {
                new() { Type = BonusPointType.Position, Value = 1, Points = 3 },
                new() { Type = BonusPointType.Position, Value = 2, Points = 2 },
                new() { Type = BonusPointType.Position, Value = 3, Points = 1 },
            },
            PointsSortOptions = new List<SortOptions>() {
                SortOptions.PosAsc,
                SortOptions.FinPosAsc,
            },
            FinalSortOptions = new List<SortOptions>()
            {
                SortOptions.PosDesc,
                SortOptions.FinPosDesc,
            },
        };
        league1.PointRules.Add(pointsRule);

        var championship = new ChampionshipEntity()
        {
            League = league1,
            Name = "Championship"
        };
        league1.Championships.Add(championship);
        foreach(var season in league1.Seasons)
        {
            var champSeason = new ChampSeasonEntity()
            {
                Championship = championship,
                Season = season,
            };
            championship.ChampSeasons.Add(champSeason);
            var resultFilter = new FilterOptionEntity()
            {
                Conditions = new[] { new FilterConditionModel()
                {
                    Action = MatchedValueAction.Remove,
                    ColumnPropertyName = "Shame",
                    Comparator = ComparatorType.ForEach,
                    FilterType = FilterType.Member,
                    FilterValues = new[] { "Eins", "Zwei", "Drei" },
                }},
            };
            var resultConfig = new ResultConfigurationEntity()
            {
                League = league1,
                ChampSeason = champSeason,
                ResultFilters = new[] { resultFilter },
            };
            league1.ResultConfigs.Add(resultConfig);
            for (int i = 0; i < 2; i++)
            {
                var scoring = new ScoringEntity()
                {
                    Name = $"Scoring {i + 1}",
                    PointsRule = pointsRule,
                };
                resultConfig.Scorings.Add(scoring);
            }
        }

        foreach ((var season, var index) in league1.Seasons.Select((x, i) => (x, i)))
        {
            foreach (var @event in season.Schedules.SelectMany(x => x.Events))
            {
                var result = new EventResultEntity();
                @event.EventResult = result;
                var scoredResult = new ScoredEventResultEntity();
                @event.ScoredEventResults.Add(scoredResult);
                foreach (var session in @event.Sessions)
                {
                    var sessionResult = new SessionResultEntity();
                    session.SessionResult = sessionResult;
                    result.SessionResults.Add(sessionResult);
                    var scoredSessionResult = new ScoredSessionResultEntity();
                    scoredResult.ScoredSessionResults.Add(scoredSessionResult);
                    var resultMembers = members.ToList();
                    for (int i = 0; i < 10; i++)
                    {
                        var member = resultMembers.PopRandom();
                        var resultRow = new ResultRowEntity()
                        {
                            Member = member,
                            StartPosition = i + 1,
                            FinishPosition = i + 1,
                            QualifyingTime = GetTimeSpan(random),
                            FastestLapTime = GetTimeSpan(random),
                            AvgLapTime = GetTimeSpan(random),
                            Interval = GetTimeSpan(random)
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
            }
        }

        // Create reviews
        for (int i = 0; i < 5; i++)
        {
            var voteCategory = new VoteCategoryEntity()
            {
                Text = $"Category {i + 1}",
                DefaultPenalty = i + 1,
            };
            league1.VoteCategories.Add(voteCategory);
        }
        foreach (var session in season1.Schedules
            .SelectMany(x => x.Events)
            .SelectMany(x => x.Sessions))
        {
            for (int i = 0; i < 5; i++)
            {
                var review = new IncidentReviewEntity()
                {
                    Corner = (i + 1).ToString(),
                    OnLap = (i + 1).ToString(),
                    FullDescription = $"Incident review #{i + 1} Event {session.Event.Name} - {session.Name}",
                    IncidentKind = "Contact",
                    IncidentNr = (i + 1).ToString(),
                };
                for (int j = 0; j < 3; j++)
                {
                    review.InvolvedMembers.Add(GetRandomMember(random, members));
                }
                for (int j = 0; j < 5; j++)
                {
                    var comment = new ReviewCommentEntity()
                    {
                        Date = DateTime.Now,
                        AuthorName = GetRandomMember(random, members).Firstname,
                        Text = $"Comment #{j + 1} ",
                        ReviewCommentVotes = new List<ReviewCommentVoteEntity>()
                        {
                            new ReviewCommentVoteEntity()
                            {
                                MemberAtFault = GetRandomMember(random, review.InvolvedMembers),
                                Description = "Vote",
                                VoteCategory = league1.VoteCategories.GetRandom(random),
                            }
                        },
                    };
                    review.Comments.Add(comment);
                }
                var acceptedCommentVote = review.Comments.First().ReviewCommentVotes.First();
                var acceptedVote = new AcceptedReviewVoteEntity()
                {
                    MemberAtFault = acceptedCommentVote.MemberAtFault,
                    Description = acceptedCommentVote.Description,
                    VoteCategory = acceptedCommentVote.VoteCategory,
                };
                session.IncidentReviews.Add(review);
            }
        }
    }

    private static MemberEntity GetRandomMember(Random random, IEnumerable<MemberEntity> memberList)
    {
        var memberCount = memberList.Count();
        if (memberCount == 0)
        {
            return null;
        }
        return memberList.ElementAt(random.Next(memberCount));
    }

    private static TimeSpan GetTimeSpan(Random random)
    {
        return new TimeSpan(0, 1, 2, 34, 567);
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
}

public static class PopulateDatabaseExtensions
{
    /// <summary>
    /// Returns a random entry from the list and removes it from the list at the same time
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">List to pop item from</param>
    /// <param name="random">Initialized random number generator</param>
    /// <returns></returns>
    public static T PopRandom<T>(this ICollection<T> collection, Random random = null)
    {
        random ??= new Random();
        var randomIndex = random.Next(collection.Count());
        var pop = collection.ElementAt(randomIndex);
        collection.Remove(pop);
        return pop;
    }

    public static T GetRandom<T>(this ICollection<T> collection, Random random = null)
    {
        random ??= new();
        return collection.ElementAt(random.Next(collection.Count()));
    }
}