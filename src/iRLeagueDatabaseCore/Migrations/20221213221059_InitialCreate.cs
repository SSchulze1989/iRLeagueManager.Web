using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace iRLeagueDatabaseCore.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(85)", maxLength: 85, nullable: false),
                    NameFull = table.Column<string>(type: "longtext", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                    table.UniqueConstraint("AK_Leagues_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Firstname = table.Column<string>(type: "longtext", nullable: true),
                    Lastname = table.Column<string>(type: "longtext", nullable: true),
                    IRacingId = table.Column<string>(type: "longtext", nullable: true),
                    DanLisaId = table.Column<string>(type: "longtext", nullable: true),
                    DiscordId = table.Column<string>(type: "longtext", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StandingConfigurations",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    StandingConfigId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    ResultKind = table.Column<int>(type: "int", nullable: false),
                    UseCombinedResult = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    WeeksCounted = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandingConfigurations", x => new { x.LeagueId, x.StandingConfigId });
                    table.UniqueConstraint("AK_StandingConfigurations_StandingConfigId", x => x.StandingConfigId);
                });

            migrationBuilder.CreateTable(
                name: "TrackGroups",
                columns: table => new
                {
                    TrackGroupId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    TrackName = table.Column<string>(type: "longtext", nullable: true),
                    Location = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackGroups", x => x.TrackGroupId);
                });

            migrationBuilder.CreateTable(
                name: "CustomIncidents",
                columns: table => new
                {
                    IncidentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "longtext", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomIncidents", x => new { x.LeagueId, x.IncidentId });
                    table.UniqueConstraint("AK_CustomIncidents_IncidentId", x => x.IncidentId);
                    table.ForeignKey(
                        name: "FK_CustomIncidents_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointRules",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    PointRuleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    PointsPerPlace = table.Column<string>(type: "longtext", nullable: true),
                    BonusPoints = table.Column<string>(type: "longtext", nullable: true),
                    MaxPoints = table.Column<int>(type: "int", nullable: false),
                    PointDropOff = table.Column<int>(type: "int", nullable: false),
                    PointsSortOptions = table.Column<string>(type: "longtext", nullable: true),
                    FinalSortOptions = table.Column<string>(type: "longtext", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointRules", x => new { x.LeagueId, x.PointRuleId });
                    table.UniqueConstraint("AK_PointRules_PointRuleId", x => x.PointRuleId);
                    table.ForeignKey(
                        name: "FK_PointRules_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultConfigurations",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ResultConfigId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SourceResultConfigId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    DisplayName = table.Column<string>(type: "longtext", nullable: true),
                    ResultKind = table.Column<string>(type: "longtext", nullable: false),
                    ResultsPerTeam = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultConfigurations", x => new { x.LeagueId, x.ResultConfigId });
                    table.UniqueConstraint("AK_ResultConfigurations_ResultConfigId", x => x.ResultConfigId);
                    table.ForeignKey(
                        name: "FK_ResultConfigurations_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultConfigurations_ResultConfigurations_LeagueId_SourceRes~",
                        columns: x => new { x.LeagueId, x.SourceResultConfigId },
                        principalTable: "ResultConfigurations",
                        principalColumns: new[] { "LeagueId", "ResultConfigId" });
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    Profile = table.Column<string>(type: "longtext", nullable: true),
                    TeamColor = table.Column<string>(type: "longtext", nullable: true),
                    TeamHomepage = table.Column<string>(type: "longtext", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => new { x.LeagueId, x.TeamId });
                    table.UniqueConstraint("AK_Teams_TeamId", x => x.TeamId);
                    table.ForeignKey(
                        name: "FK_Teams_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoteCategories",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    CatId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(type: "longtext", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    DefaultPenalty = table.Column<int>(type: "int", nullable: false),
                    ImportId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteCategories", x => new { x.LeagueId, x.CatId });
                    table.UniqueConstraint("AK_VoteCategories_CatId", x => x.CatId);
                    table.ForeignKey(
                        name: "FK_VoteCategories_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackConfigs",
                columns: table => new
                {
                    TrackId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    TrackGroupId = table.Column<long>(type: "bigint", nullable: false),
                    ConfigName = table.Column<string>(type: "longtext", nullable: true),
                    LengthKm = table.Column<double>(type: "double", nullable: false),
                    Turns = table.Column<int>(type: "int", nullable: false),
                    ConfigType = table.Column<string>(type: "longtext", nullable: false),
                    HasNightLighting = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LegacyTrackId = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackConfigs", x => x.TrackId);
                    table.ForeignKey(
                        name: "FK_TrackConfigs_TrackGroups_TrackGroupId",
                        column: x => x.TrackGroupId,
                        principalTable: "TrackGroups",
                        principalColumn: "TrackGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilterOptions",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    FilterOptionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PointFilterResultConfigId = table.Column<long>(type: "bigint", nullable: true),
                    ResultFilterResultConfigId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterOptions", x => new { x.LeagueId, x.FilterOptionId });
                    table.UniqueConstraint("AK_FilterOptions_FilterOptionId", x => x.FilterOptionId);
                    table.ForeignKey(
                        name: "FK_FilterOptions_ResultConfigurations_LeagueId_PointFilterResul~",
                        columns: x => new { x.LeagueId, x.PointFilterResultConfigId },
                        principalTable: "ResultConfigurations",
                        principalColumns: new[] { "LeagueId", "ResultConfigId" });
                    table.ForeignKey(
                        name: "FK_FilterOptions_ResultConfigurations_LeagueId_ResultFilterResu~",
                        columns: x => new { x.LeagueId, x.ResultFilterResultConfigId },
                        principalTable: "ResultConfigurations",
                        principalColumns: new[] { "LeagueId", "ResultConfigId" });
                });

            migrationBuilder.CreateTable(
                name: "StandingConfigs_ResultConfigs",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    StandingConfigId = table.Column<long>(type: "bigint", nullable: false),
                    ResultConfigId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandingConfigs_ResultConfigs", x => new { x.ResultConfigId, x.LeagueId, x.StandingConfigId });
                    table.ForeignKey(
                        name: "FK_StandingConfigs_ResultConfigs_ResultConfigurations_LeagueId_~",
                        columns: x => new { x.LeagueId, x.ResultConfigId },
                        principalTable: "ResultConfigurations",
                        principalColumns: new[] { "LeagueId", "ResultConfigId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandingConfigs_ResultConfigs_StandingConfigurations_LeagueI~",
                        columns: x => new { x.LeagueId, x.StandingConfigId },
                        principalTable: "StandingConfigurations",
                        principalColumns: new[] { "LeagueId", "StandingConfigId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeagueMembers",
                columns: table => new
                {
                    MemberId = table.Column<long>(type: "bigint", nullable: false),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueMembers", x => new { x.LeagueId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_LeagueMembers_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueMembers_Teams_LeagueId_TeamId",
                        columns: x => new { x.LeagueId, x.TeamId },
                        principalTable: "Teams",
                        principalColumns: new[] { "LeagueId", "TeamId" });
                });

            migrationBuilder.CreateTable(
                name: "FilterConditions",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ConditionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FilterOptionId = table.Column<long>(type: "bigint", nullable: false),
                    FilterType = table.Column<string>(type: "longtext", nullable: false),
                    ColumnPropertyName = table.Column<string>(type: "longtext", nullable: true),
                    Comparator = table.Column<string>(type: "longtext", nullable: false),
                    Action = table.Column<string>(type: "longtext", nullable: false),
                    FilterValues = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterConditions", x => new { x.LeagueId, x.ConditionId });
                    table.UniqueConstraint("AK_FilterConditions_ConditionId", x => x.ConditionId);
                    table.ForeignKey(
                        name: "FK_FilterConditions_FilterOptions_LeagueId_FilterOptionId",
                        columns: x => new { x.LeagueId, x.FilterOptionId },
                        principalTable: "FilterOptions",
                        principalColumns: new[] { "LeagueId", "FilterOptionId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcceptedReviewVotes",
                columns: table => new
                {
                    ReviewVoteId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewId = table.Column<long>(type: "bigint", nullable: false),
                    MemberAtFaultId = table.Column<long>(type: "bigint", nullable: true),
                    VoteCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedReviewVotes", x => new { x.LeagueId, x.ReviewVoteId });
                    table.UniqueConstraint("AK_AcceptedReviewVotes_ReviewVoteId", x => x.ReviewVoteId);
                    table.ForeignKey(
                        name: "FK_AcceptedReviewVotes_Members_MemberAtFaultId",
                        column: x => x.MemberAtFaultId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AcceptedReviewVotes_VoteCategories_LeagueId_VoteCategoryId",
                        columns: x => new { x.LeagueId, x.VoteCategoryId },
                        principalTable: "VoteCategories",
                        principalColumns: new[] { "LeagueId", "CatId" });
                });

            migrationBuilder.CreateTable(
                name: "AddPenaltys",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ScoredResultRowId = table.Column<long>(type: "bigint", nullable: false),
                    PenaltyPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddPenaltys", x => new { x.LeagueId, x.ScoredResultRowId });
                });

            migrationBuilder.CreateTable(
                name: "DriverStatisticRows",
                columns: table => new
                {
                    StatisticSetId = table.Column<long>(type: "bigint", nullable: false),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    MemberId = table.Column<long>(type: "bigint", nullable: false),
                    FirstResultRowId = table.Column<long>(type: "bigint", nullable: true),
                    LastResultRowId = table.Column<long>(type: "bigint", nullable: true),
                    StartIRating = table.Column<int>(type: "int", nullable: false),
                    EndIRating = table.Column<int>(type: "int", nullable: false),
                    StartSRating = table.Column<double>(type: "double", nullable: false),
                    EndSRating = table.Column<double>(type: "double", nullable: false),
                    FirstSessionId = table.Column<long>(type: "bigint", nullable: true),
                    FirstSessionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    FirstRaceId = table.Column<long>(type: "bigint", nullable: true),
                    FirstRaceDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastSessionId = table.Column<long>(type: "bigint", nullable: true),
                    LastSessionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastRaceId = table.Column<long>(type: "bigint", nullable: true),
                    LastRaceDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RacePoints = table.Column<double>(type: "double", nullable: false),
                    TotalPoints = table.Column<double>(type: "double", nullable: false),
                    BonusPoints = table.Column<double>(type: "double", nullable: false),
                    Races = table.Column<int>(type: "int", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    Poles = table.Column<int>(type: "int", nullable: false),
                    Top3 = table.Column<int>(type: "int", nullable: false),
                    Top5 = table.Column<int>(type: "int", nullable: false),
                    Top10 = table.Column<int>(type: "int", nullable: false),
                    Top15 = table.Column<int>(type: "int", nullable: false),
                    Top20 = table.Column<int>(type: "int", nullable: false),
                    Top25 = table.Column<int>(type: "int", nullable: false),
                    RacesInPoints = table.Column<int>(type: "int", nullable: false),
                    RacesCompleted = table.Column<int>(type: "int", nullable: false),
                    Incidents = table.Column<double>(type: "double", nullable: false),
                    PenaltyPoints = table.Column<double>(type: "double", nullable: false),
                    FastestLaps = table.Column<int>(type: "int", nullable: false),
                    IncidentsUnderInvestigation = table.Column<int>(type: "int", nullable: false),
                    IncidentsWithPenalty = table.Column<int>(type: "int", nullable: false),
                    LeadingLaps = table.Column<double>(type: "double", nullable: false),
                    CompletedLaps = table.Column<double>(type: "double", nullable: false),
                    CurrentSeasonPosition = table.Column<int>(type: "int", nullable: false),
                    DrivenKm = table.Column<double>(type: "double", nullable: false),
                    LeadingKm = table.Column<double>(type: "double", nullable: false),
                    AvgFinishPosition = table.Column<double>(type: "double", nullable: false),
                    AvgFinalPosition = table.Column<double>(type: "double", nullable: false),
                    AvgStartPosition = table.Column<double>(type: "double", nullable: false),
                    AvgPointsPerRace = table.Column<double>(type: "double", nullable: false),
                    AvgIncidentsPerRace = table.Column<double>(type: "double", nullable: false),
                    AvgIncidentsPerLap = table.Column<double>(type: "double", nullable: false),
                    AvgIncidentsPerKm = table.Column<double>(type: "double", nullable: false),
                    AvgPenaltyPointsPerRace = table.Column<double>(type: "double", nullable: false),
                    AvgPenaltyPointsPerLap = table.Column<double>(type: "double", nullable: false),
                    AvgPenaltyPointsPerKm = table.Column<double>(type: "double", nullable: false),
                    AvgIRating = table.Column<double>(type: "double", nullable: false),
                    AvgSRating = table.Column<double>(type: "double", nullable: false),
                    BestFinishPosition = table.Column<double>(type: "double", nullable: false),
                    WorstFinishPosition = table.Column<double>(type: "double", nullable: false),
                    FirstRaceFinishPosition = table.Column<double>(type: "double", nullable: false),
                    LastRaceFinishPosition = table.Column<double>(type: "double", nullable: false),
                    BestFinalPosition = table.Column<int>(type: "int", nullable: false),
                    WorstFinalPosition = table.Column<int>(type: "int", nullable: false),
                    FirstRaceFinalPosition = table.Column<int>(type: "int", nullable: false),
                    LastRaceFinalPosition = table.Column<int>(type: "int", nullable: false),
                    BestStartPosition = table.Column<double>(type: "double", nullable: false),
                    WorstStartPosition = table.Column<double>(type: "double", nullable: false),
                    FirstRaceStartPosition = table.Column<double>(type: "double", nullable: false),
                    LastRaceStartPosition = table.Column<double>(type: "double", nullable: false),
                    Titles = table.Column<int>(type: "int", nullable: false),
                    HardChargerAwards = table.Column<int>(type: "int", nullable: false),
                    CleanestDriverAwards = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverStatisticRows", x => new { x.LeagueId, x.StatisticSetId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_DriverStatisticRows_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventResultConfigs",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    EventRefId = table.Column<long>(type: "bigint", nullable: false),
                    ResultConfigRefId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventResultConfigs", x => new { x.EventRefId, x.LeagueId, x.ResultConfigRefId });
                    table.ForeignKey(
                        name: "FK_EventResultConfigs_ResultConfigurations_LeagueId_ResultConfi~",
                        columns: x => new { x.LeagueId, x.ResultConfigRefId },
                        principalTable: "ResultConfigurations",
                        principalColumns: new[] { "LeagueId", "ResultConfigId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventResults",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    RequiresRecalculation = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventResults", x => new { x.LeagueId, x.EventId });
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    TrackId = table.Column<long>(type: "bigint", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true),
                    EventType = table.Column<string>(type: "longtext", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    IrSessionId = table.Column<string>(type: "longtext", nullable: true),
                    IrResultLink = table.Column<string>(type: "longtext", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => new { x.LeagueId, x.EventId });
                    table.UniqueConstraint("AK_Events_EventId", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_TrackConfigs_TrackId",
                        column: x => x.TrackId,
                        principalTable: "TrackConfigs",
                        principalColumn: "TrackId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "IRSimSessionDetails",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    SessionDetailsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    IRSubsessionId = table.Column<long>(type: "bigint", nullable: false),
                    IRSeasonId = table.Column<long>(type: "bigint", nullable: false),
                    IRSeasonName = table.Column<string>(type: "longtext", nullable: true),
                    IRSeasonYear = table.Column<int>(type: "int", nullable: false),
                    IRSeasonQuarter = table.Column<int>(type: "int", nullable: false),
                    IRRaceWeek = table.Column<int>(type: "int", nullable: false),
                    IRSessionId = table.Column<long>(type: "bigint", nullable: false),
                    LicenseCategory = table.Column<int>(type: "int", nullable: false),
                    SessionName = table.Column<string>(type: "longtext", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CornersPerLap = table.Column<int>(type: "int", nullable: false),
                    KmDistPerLap = table.Column<double>(type: "double", nullable: false),
                    MaxWeeks = table.Column<int>(type: "int", nullable: false),
                    EventStrengthOfField = table.Column<int>(type: "int", nullable: false),
                    EventAverageLap = table.Column<long>(type: "bigint", nullable: false),
                    EventLapsComplete = table.Column<int>(type: "int", nullable: false),
                    NumCautions = table.Column<int>(type: "int", nullable: false),
                    NumCautionLaps = table.Column<int>(type: "int", nullable: false),
                    NumLeadChanges = table.Column<int>(type: "int", nullable: false),
                    TimeOfDay = table.Column<int>(type: "int", nullable: false),
                    DamageModel = table.Column<int>(type: "int", nullable: false),
                    IRTrackId = table.Column<int>(type: "int", nullable: false),
                    TrackName = table.Column<string>(type: "longtext", nullable: true),
                    ConfigName = table.Column<string>(type: "longtext", nullable: true),
                    TrackCategoryId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "longtext", nullable: true),
                    WeatherType = table.Column<int>(type: "int", nullable: false),
                    TempUnits = table.Column<int>(type: "int", nullable: false),
                    TempValue = table.Column<int>(type: "int", nullable: false),
                    RelHumidity = table.Column<int>(type: "int", nullable: false),
                    Fog = table.Column<int>(type: "int", nullable: false),
                    WindDir = table.Column<int>(type: "int", nullable: false),
                    WindUnits = table.Column<int>(type: "int", nullable: false),
                    Skies = table.Column<int>(type: "int", nullable: false),
                    WeatherVarInitial = table.Column<int>(type: "int", nullable: false),
                    WeatherVarOngoing = table.Column<int>(type: "int", nullable: false),
                    SimStartUtcTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    SimStartUtcOffset = table.Column<long>(type: "bigint", nullable: false),
                    LeaveMarbles = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PracticeRubber = table.Column<int>(type: "int", nullable: false),
                    QualifyRubber = table.Column<int>(type: "int", nullable: false),
                    WarmupRubber = table.Column<int>(type: "int", nullable: false),
                    RaceRubber = table.Column<int>(type: "int", nullable: false),
                    PracticeGripCompound = table.Column<int>(type: "int", nullable: false),
                    QualifyGripCompund = table.Column<int>(type: "int", nullable: false),
                    WarmupGripCompound = table.Column<int>(type: "int", nullable: false),
                    RaceGripCompound = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRSimSessionDetails", x => new { x.LeagueId, x.SessionDetailsId });
                    table.UniqueConstraint("AK_IRSimSessionDetails_SessionDetailsId", x => x.SessionDetailsId);
                    table.ForeignKey(
                        name: "FK_IRSimSessionDetails_Events_LeagueId_EventId",
                        columns: x => new { x.LeagueId, x.EventId },
                        principalTable: "Events",
                        principalColumns: new[] { "LeagueId", "EventId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoredEventResults",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ResultId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    ResultConfigId = table.Column<long>(type: "bigint", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    EventResultEntityEventId = table.Column<long>(type: "bigint", nullable: true),
                    EventResultEntityLeagueId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoredEventResults", x => new { x.LeagueId, x.ResultId });
                    table.UniqueConstraint("AK_ScoredEventResults_ResultId", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_ScoredEventResults_EventResults_EventResultEntityLeagueId_Ev~",
                        columns: x => new { x.EventResultEntityLeagueId, x.EventResultEntityEventId },
                        principalTable: "EventResults",
                        principalColumns: new[] { "LeagueId", "EventId" });
                    table.ForeignKey(
                        name: "FK_ScoredEventResults_Events_LeagueId_EventId",
                        columns: x => new { x.LeagueId, x.EventId },
                        principalTable: "Events",
                        principalColumns: new[] { "LeagueId", "EventId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoredEventResults_ResultConfigurations_LeagueId_ResultConfi~",
                        columns: x => new { x.LeagueId, x.ResultConfigId },
                        principalTable: "ResultConfigurations",
                        principalColumns: new[] { "LeagueId", "ResultConfigId" });
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    SessionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    SessionNr = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    SessionType = table.Column<string>(type: "longtext", nullable: false),
                    StartOffset = table.Column<long>(type: "bigint", nullable: false),
                    Duration = table.Column<long>(type: "bigint", nullable: false),
                    Laps = table.Column<int>(type: "int", nullable: false),
                    ImportId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => new { x.LeagueId, x.SessionId });
                    table.UniqueConstraint("AK_Sessions_SessionId", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Sessions_Events_LeagueId_EventId",
                        columns: x => new { x.LeagueId, x.EventId },
                        principalTable: "Events",
                        principalColumns: new[] { "LeagueId", "EventId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncidentReviews",
                columns: table => new
                {
                    ReviewId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    SessionId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorUserId = table.Column<string>(type: "longtext", nullable: true),
                    AuthorName = table.Column<string>(type: "longtext", nullable: true),
                    IncidentKind = table.Column<string>(type: "longtext", nullable: true),
                    FullDescription = table.Column<string>(type: "longtext", nullable: true),
                    OnLap = table.Column<string>(type: "longtext", nullable: true),
                    Corner = table.Column<string>(type: "longtext", nullable: true),
                    TimeStamp = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    ResultLongText = table.Column<string>(type: "longtext", nullable: true),
                    IncidentNr = table.Column<string>(type: "longtext", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReviews", x => new { x.LeagueId, x.ReviewId });
                    table.UniqueConstraint("AK_IncidentReviews_ReviewId", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_IncidentReviews_Sessions_LeagueId_SessionId",
                        columns: x => new { x.LeagueId, x.SessionId },
                        principalTable: "Sessions",
                        principalColumns: new[] { "LeagueId", "SessionId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionResults",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    SessionId = table.Column<long>(type: "bigint", nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    IRSimSessionDetailsId = table.Column<long>(type: "bigint", nullable: true),
                    SimSessionType = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionResults", x => new { x.LeagueId, x.SessionId });
                    table.ForeignKey(
                        name: "FK_SessionResults_EventResults_LeagueId_EventId",
                        columns: x => new { x.LeagueId, x.EventId },
                        principalTable: "EventResults",
                        principalColumns: new[] { "LeagueId", "EventId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionResults_IRSimSessionDetails_LeagueId_IRSimSessionDeta~",
                        columns: x => new { x.LeagueId, x.IRSimSessionDetailsId },
                        principalTable: "IRSimSessionDetails",
                        principalColumns: new[] { "LeagueId", "SessionDetailsId" });
                    table.ForeignKey(
                        name: "FK_SessionResults_Sessions_LeagueId_SessionId",
                        columns: x => new { x.LeagueId, x.SessionId },
                        principalTable: "Sessions",
                        principalColumns: new[] { "LeagueId", "SessionId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncidentReviewsInvolvedMembers",
                columns: table => new
                {
                    InvolvedMembersId = table.Column<long>(type: "bigint", nullable: false),
                    InvolvedReviewsLeagueId = table.Column<long>(type: "bigint", nullable: false),
                    InvolvedReviewsReviewId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReviewsInvolvedMembers", x => new { x.InvolvedMembersId, x.InvolvedReviewsLeagueId, x.InvolvedReviewsReviewId });
                    table.ForeignKey(
                        name: "FK_IncidentReviewsInvolvedMembers_IncidentReviews_InvolvedRevie~",
                        columns: x => new { x.InvolvedReviewsLeagueId, x.InvolvedReviewsReviewId },
                        principalTable: "IncidentReviews",
                        principalColumns: new[] { "LeagueId", "ReviewId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReviewsInvolvedMembers_Members_InvolvedMembersId",
                        column: x => x.InvolvedMembersId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewComments",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    CommentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ReviewId = table.Column<long>(type: "bigint", nullable: true),
                    ReplyToCommentId = table.Column<long>(type: "bigint", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    AuthorUserId = table.Column<string>(type: "longtext", nullable: true),
                    AuthorName = table.Column<string>(type: "longtext", nullable: true),
                    Text = table.Column<string>(type: "longtext", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewComments", x => new { x.LeagueId, x.CommentId });
                    table.UniqueConstraint("AK_ReviewComments_CommentId", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_ReviewComments_IncidentReviews_LeagueId_ReviewId",
                        columns: x => new { x.LeagueId, x.ReviewId },
                        principalTable: "IncidentReviews",
                        principalColumns: new[] { "LeagueId", "ReviewId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewComments_ReviewComments_LeagueId_ReplyToCommentId",
                        columns: x => new { x.LeagueId, x.ReplyToCommentId },
                        principalTable: "ReviewComments",
                        principalColumns: new[] { "LeagueId", "CommentId" });
                });

            migrationBuilder.CreateTable(
                name: "ResultRows",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ResultRowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SubSessionId = table.Column<long>(type: "bigint", nullable: false),
                    MemberId = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    PointsEligible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    StartPosition = table.Column<double>(type: "double", nullable: false),
                    FinishPosition = table.Column<double>(type: "double", nullable: false),
                    CarNumber = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    Car = table.Column<string>(type: "longtext", nullable: true),
                    CarClass = table.Column<string>(type: "longtext", nullable: true),
                    CompletedLaps = table.Column<double>(type: "double", nullable: false),
                    LeadLaps = table.Column<double>(type: "double", nullable: false),
                    FastLapNr = table.Column<int>(type: "int", nullable: false),
                    Incidents = table.Column<double>(type: "double", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QualifyingTime = table.Column<long>(type: "bigint", nullable: false),
                    Interval = table.Column<long>(type: "bigint", nullable: false),
                    AvgLapTime = table.Column<long>(type: "bigint", nullable: false),
                    FastestLapTime = table.Column<long>(type: "bigint", nullable: false),
                    PositionChange = table.Column<double>(type: "double", nullable: false),
                    IRacingId = table.Column<string>(type: "longtext", nullable: true),
                    SimSessionType = table.Column<int>(type: "int", nullable: false),
                    OldIRating = table.Column<int>(type: "int", nullable: false),
                    NewIRating = table.Column<int>(type: "int", nullable: false),
                    SeasonStartIRating = table.Column<int>(type: "int", nullable: false),
                    License = table.Column<string>(type: "longtext", nullable: true),
                    OldSafetyRating = table.Column<double>(type: "double", nullable: false),
                    NewSafetyRating = table.Column<double>(type: "double", nullable: false),
                    OldCpi = table.Column<int>(type: "int", nullable: false),
                    NewCpi = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    ClubName = table.Column<string>(type: "longtext", nullable: true),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    CompletedPct = table.Column<double>(type: "double", nullable: false),
                    QualifyingTimeAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Division = table.Column<int>(type: "int", nullable: false),
                    OldLicenseLevel = table.Column<int>(type: "int", nullable: false),
                    NewLicenseLevel = table.Column<int>(type: "int", nullable: false),
                    NumPitStops = table.Column<int>(type: "int", nullable: false),
                    PittedLaps = table.Column<string>(type: "longtext", nullable: true),
                    NumOfftrackLaps = table.Column<int>(type: "int", nullable: false),
                    OfftrackLaps = table.Column<string>(type: "longtext", nullable: true),
                    NumContactLaps = table.Column<int>(type: "int", nullable: false),
                    ContactLaps = table.Column<string>(type: "longtext", nullable: true),
                    RacePoints = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultRows", x => new { x.LeagueId, x.ResultRowId });
                    table.UniqueConstraint("AK_ResultRows_ResultRowId", x => x.ResultRowId);
                    table.ForeignKey(
                        name: "FK_ResultRows_LeagueMembers_LeagueId_MemberId",
                        columns: x => new { x.LeagueId, x.MemberId },
                        principalTable: "LeagueMembers",
                        principalColumns: new[] { "LeagueId", "MemberId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultRows_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ResultRows_SessionResults_LeagueId_SubSessionId",
                        columns: x => new { x.LeagueId, x.SubSessionId },
                        principalTable: "SessionResults",
                        principalColumns: new[] { "LeagueId", "SessionId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultRows_Teams_LeagueId_TeamId",
                        columns: x => new { x.LeagueId, x.TeamId },
                        principalTable: "Teams",
                        principalColumns: new[] { "LeagueId", "TeamId" });
                });

            migrationBuilder.CreateTable(
                name: "ReviewCommentVotes",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewVoteId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CommentId = table.Column<long>(type: "bigint", nullable: false),
                    MemberAtFaultId = table.Column<long>(type: "bigint", nullable: true),
                    VoteCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewCommentVotes", x => new { x.LeagueId, x.ReviewVoteId });
                    table.UniqueConstraint("AK_ReviewCommentVotes_ReviewVoteId", x => x.ReviewVoteId);
                    table.ForeignKey(
                        name: "FK_ReviewCommentVotes_Members_MemberAtFaultId",
                        column: x => x.MemberAtFaultId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewCommentVotes_ReviewComments_LeagueId_CommentId",
                        columns: x => new { x.LeagueId, x.CommentId },
                        principalTable: "ReviewComments",
                        principalColumns: new[] { "LeagueId", "CommentId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCommentVotes_VoteCategories_LeagueId_VoteCategoryId",
                        columns: x => new { x.LeagueId, x.VoteCategoryId },
                        principalTable: "VoteCategories",
                        principalColumns: new[] { "LeagueId", "CatId" });
                });

            migrationBuilder.CreateTable(
                name: "LeagueStatisticSetsStatisticSets",
                columns: table => new
                {
                    DependendStatisticSetsLeagueId = table.Column<long>(type: "bigint", nullable: false),
                    DependendStatisticSetsId = table.Column<long>(type: "bigint", nullable: false),
                    LeagueStatisticSetsLeagueId = table.Column<long>(type: "bigint", nullable: false),
                    LeagueStatisticSetsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueStatisticSetsStatisticSets", x => new { x.DependendStatisticSetsLeagueId, x.DependendStatisticSetsId, x.LeagueStatisticSetsLeagueId, x.LeagueStatisticSetsId });
                });

            migrationBuilder.CreateTable(
                name: "ReviewPenaltys",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ResultRowId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewId = table.Column<long>(type: "bigint", nullable: false),
                    PenaltyPoints = table.Column<int>(type: "int", nullable: false),
                    ReviewVoteId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewPenaltys", x => new { x.LeagueId, x.ResultRowId, x.ReviewId });
                    table.ForeignKey(
                        name: "FK_ReviewPenaltys_AcceptedReviewVotes_LeagueId_ReviewVoteId",
                        columns: x => new { x.LeagueId, x.ReviewVoteId },
                        principalTable: "AcceptedReviewVotes",
                        principalColumns: new[] { "LeagueId", "ReviewVoteId" });
                    table.ForeignKey(
                        name: "FK_ReviewPenaltys_IncidentReviews_LeagueId_ReviewId",
                        columns: x => new { x.LeagueId, x.ReviewId },
                        principalTable: "IncidentReviews",
                        principalColumns: new[] { "LeagueId", "ReviewId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    ScheduleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    ImportId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => new { x.LeagueId, x.ScheduleId });
                    table.UniqueConstraint("AK_Schedules_ScheduleId", x => x.ScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "Scorings",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ScoringId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ResultConfigId = table.Column<long>(type: "bigint", nullable: false),
                    PointsRuleId = table.Column<long>(type: "bigint", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    MaxResultsPerGroup = table.Column<int>(type: "int", nullable: false),
                    ExtScoringSourceId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    UseResultSetTeam = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UpdateTeamOnRecalculation = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowResults = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsCombinedResult = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ScheduleEntityLeagueId = table.Column<long>(type: "bigint", nullable: true),
                    ScheduleEntityScheduleId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scorings", x => new { x.LeagueId, x.ScoringId });
                    table.UniqueConstraint("AK_Scorings_ScoringId", x => x.ScoringId);
                    table.ForeignKey(
                        name: "FK_Scorings_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scorings_PointRules_LeagueId_PointsRuleId",
                        columns: x => new { x.LeagueId, x.PointsRuleId },
                        principalTable: "PointRules",
                        principalColumns: new[] { "LeagueId", "PointRuleId" });
                    table.ForeignKey(
                        name: "FK_Scorings_ResultConfigurations_LeagueId_ResultConfigId",
                        columns: x => new { x.LeagueId, x.ResultConfigId },
                        principalTable: "ResultConfigurations",
                        principalColumns: new[] { "LeagueId", "ResultConfigId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scorings_Schedules_ScheduleEntityLeagueId_ScheduleEntitySche~",
                        columns: x => new { x.ScheduleEntityLeagueId, x.ScheduleEntityScheduleId },
                        principalTable: "Schedules",
                        principalColumns: new[] { "LeagueId", "ScheduleId" });
                    table.ForeignKey(
                        name: "FK_Scorings_Scorings_LeagueId_ExtScoringSourceId",
                        columns: x => new { x.LeagueId, x.ExtScoringSourceId },
                        principalTable: "Scorings",
                        principalColumns: new[] { "LeagueId", "ScoringId" });
                });

            migrationBuilder.CreateTable(
                name: "ScoredSessionResults",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    SessionResultId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ResultId = table.Column<long>(type: "bigint", nullable: false),
                    ScoringId = table.Column<long>(type: "bigint", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true),
                    SessionNr = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    FastestLap = table.Column<long>(type: "bigint", nullable: false),
                    FastestQualyLap = table.Column<long>(type: "bigint", nullable: false),
                    FastestAvgLap = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    Discriminator = table.Column<string>(type: "longtext", nullable: true),
                    FastestAvgLapDriverMemberId = table.Column<long>(type: "bigint", nullable: true),
                    FastestLapDriverMemberId = table.Column<long>(type: "bigint", nullable: true),
                    FastestQualyLapDriverMemberId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoredSessionResults", x => new { x.LeagueId, x.SessionResultId });
                    table.UniqueConstraint("AK_ScoredSessionResults_SessionResultId", x => x.SessionResultId);
                    table.ForeignKey(
                        name: "FK_ScoredSessionResults_Members_FastestAvgLapDriverMemberId",
                        column: x => x.FastestAvgLapDriverMemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScoredSessionResults_Members_FastestLapDriverMemberId",
                        column: x => x.FastestLapDriverMemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScoredSessionResults_Members_FastestQualyLapDriverMemberId",
                        column: x => x.FastestQualyLapDriverMemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScoredSessionResults_ScoredEventResults_LeagueId_ResultId",
                        columns: x => new { x.LeagueId, x.ResultId },
                        principalTable: "ScoredEventResults",
                        principalColumns: new[] { "LeagueId", "ResultId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoredSessionResults_Scorings_LeagueId_ScoringId",
                        columns: x => new { x.LeagueId, x.ScoringId },
                        principalTable: "Scorings",
                        principalColumns: new[] { "LeagueId", "ScoringId" });
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    SeasonId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    SeasonName = table.Column<string>(type: "longtext", nullable: true),
                    HideCommentsBeforeVoted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Finished = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SeasonStart = table.Column<DateTime>(type: "datetime", nullable: true),
                    SeasonEnd = table.Column<DateTime>(type: "datetime", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true),
                    MainScoringLeagueId = table.Column<long>(type: "bigint", nullable: true),
                    MainScoringScoringId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => new { x.LeagueId, x.SeasonId });
                    table.UniqueConstraint("AK_Seasons_SeasonId", x => x.SeasonId);
                    table.ForeignKey(
                        name: "FK_Seasons_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Seasons_Scorings_MainScoringLeagueId_MainScoringScoringId",
                        columns: x => new { x.MainScoringLeagueId, x.MainScoringScoringId },
                        principalTable: "Scorings",
                        principalColumns: new[] { "LeagueId", "ScoringId" });
                });

            migrationBuilder.CreateTable(
                name: "ScoredResultRows",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    ScoredResultRowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SessionResultId = table.Column<long>(type: "bigint", nullable: false),
                    MemberId = table.Column<long>(type: "bigint", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    ImportId = table.Column<long>(type: "bigint", nullable: true),
                    BonusPoints = table.Column<double>(type: "double", nullable: false),
                    PenaltyPoints = table.Column<double>(type: "double", nullable: false),
                    FinalPosition = table.Column<int>(type: "int", nullable: false),
                    FinalPositionChange = table.Column<double>(type: "double", nullable: false),
                    TotalPoints = table.Column<double>(type: "double", nullable: false),
                    PointsEligible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    StartPosition = table.Column<double>(type: "double", nullable: false),
                    FinishPosition = table.Column<double>(type: "double", nullable: false),
                    CarNumber = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    Car = table.Column<string>(type: "longtext", nullable: true),
                    CarClass = table.Column<string>(type: "longtext", nullable: true),
                    CompletedLaps = table.Column<double>(type: "double", nullable: false),
                    LeadLaps = table.Column<double>(type: "double", nullable: false),
                    FastLapNr = table.Column<int>(type: "int", nullable: false),
                    Incidents = table.Column<double>(type: "double", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QualifyingTime = table.Column<long>(type: "bigint", nullable: false),
                    Interval = table.Column<long>(type: "bigint", nullable: false),
                    AvgLapTime = table.Column<long>(type: "bigint", nullable: false),
                    FastestLapTime = table.Column<long>(type: "bigint", nullable: false),
                    PositionChange = table.Column<double>(type: "double", nullable: false),
                    IRacingId = table.Column<string>(type: "longtext", nullable: true),
                    SimSessionType = table.Column<int>(type: "int", nullable: false),
                    OldIRating = table.Column<int>(type: "int", nullable: false),
                    NewIRating = table.Column<int>(type: "int", nullable: false),
                    SeasonStartIRating = table.Column<int>(type: "int", nullable: false),
                    License = table.Column<string>(type: "longtext", nullable: true),
                    OldSafetyRating = table.Column<double>(type: "double", nullable: false),
                    NewSafetyRating = table.Column<double>(type: "double", nullable: false),
                    OldCpi = table.Column<int>(type: "int", nullable: false),
                    NewCpi = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    ClubName = table.Column<string>(type: "longtext", nullable: true),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    CompletedPct = table.Column<double>(type: "double", nullable: false),
                    QualifyingTimeAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Division = table.Column<int>(type: "int", nullable: false),
                    OldLicenseLevel = table.Column<int>(type: "int", nullable: false),
                    NewLicenseLevel = table.Column<int>(type: "int", nullable: false),
                    NumPitStops = table.Column<int>(type: "int", nullable: false),
                    PittedLaps = table.Column<string>(type: "longtext", nullable: true),
                    NumOfftrackLaps = table.Column<int>(type: "int", nullable: false),
                    OfftrackLaps = table.Column<string>(type: "longtext", nullable: true),
                    NumContactLaps = table.Column<int>(type: "int", nullable: false),
                    ContactLaps = table.Column<string>(type: "longtext", nullable: true),
                    RacePoints = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoredResultRows", x => new { x.LeagueId, x.ScoredResultRowId });
                    table.UniqueConstraint("AK_ScoredResultRows_ScoredResultRowId", x => x.ScoredResultRowId);
                    table.ForeignKey(
                        name: "FK_ScoredResultRows_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScoredResultRows_ScoredSessionResults_LeagueId_SessionResult~",
                        columns: x => new { x.LeagueId, x.SessionResultId },
                        principalTable: "ScoredSessionResults",
                        principalColumns: new[] { "LeagueId", "SessionResultId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoredResultRows_Teams_LeagueId_TeamId",
                        columns: x => new { x.LeagueId, x.TeamId },
                        principalTable: "Teams",
                        principalColumns: new[] { "LeagueId", "TeamId" });
                });

            migrationBuilder.CreateTable(
                name: "ScoredResultsCleanestDrivers",
                columns: table => new
                {
                    CleanestDriversId = table.Column<long>(type: "bigint", nullable: false),
                    CleanestDriverResultsLeagueId = table.Column<long>(type: "bigint", nullable: false),
                    CleanestDriverResultsSessionResultId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoredResultsCleanestDrivers", x => new { x.CleanestDriversId, x.CleanestDriverResultsLeagueId, x.CleanestDriverResultsSessionResultId });
                    table.ForeignKey(
                        name: "FK_ScoredResultsCleanestDrivers_Members_CleanestDriversId",
                        column: x => x.CleanestDriversId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoredResultsCleanestDrivers_ScoredSessionResults_CleanestDr~",
                        columns: x => new { x.CleanestDriverResultsLeagueId, x.CleanestDriverResultsSessionResultId },
                        principalTable: "ScoredSessionResults",
                        principalColumns: new[] { "LeagueId", "SessionResultId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoredResultsHardChargers",
                columns: table => new
                {
                    HardChargersId = table.Column<long>(type: "bigint", nullable: false),
                    HardChargerResultsLeagueId = table.Column<long>(type: "bigint", nullable: false),
                    HardChargerResultsSessionResultId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoredResultsHardChargers", x => new { x.HardChargersId, x.HardChargerResultsLeagueId, x.HardChargerResultsSessionResultId });
                    table.ForeignKey(
                        name: "FK_ScoredResultsHardChargers_Members_HardChargersId",
                        column: x => x.HardChargersId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoredResultsHardChargers_ScoredSessionResults_HardChargerRe~",
                        columns: x => new { x.HardChargerResultsLeagueId, x.HardChargerResultsSessionResultId },
                        principalTable: "ScoredSessionResults",
                        principalColumns: new[] { "LeagueId", "SessionResultId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Standings",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    StandingId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    StandingConfigId = table.Column<long>(type: "bigint", nullable: true),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    IsTeamStanding = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ImportId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Standings", x => new { x.LeagueId, x.StandingId });
                    table.UniqueConstraint("AK_Standings_StandingId", x => x.StandingId);
                    table.ForeignKey(
                        name: "FK_Standings_Events_LeagueId_EventId",
                        columns: x => new { x.LeagueId, x.EventId },
                        principalTable: "Events",
                        principalColumns: new[] { "LeagueId", "EventId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Standings_Seasons_LeagueId_SeasonId",
                        columns: x => new { x.LeagueId, x.SeasonId },
                        principalTable: "Seasons",
                        principalColumns: new[] { "LeagueId", "SeasonId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Standings_StandingConfigurations_LeagueId_StandingConfigId",
                        columns: x => new { x.LeagueId, x.StandingConfigId },
                        principalTable: "StandingConfigurations",
                        principalColumns: new[] { "LeagueId", "StandingConfigId" });
                });

            migrationBuilder.CreateTable(
                name: "StatisticSets",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    UpdateInterval = table.Column<long>(type: "bigint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    RequiresRecalculation = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "longtext", nullable: true),
                    CurrentChampId = table.Column<long>(type: "bigint", nullable: true),
                    SeasonId = table.Column<long>(type: "bigint", nullable: true),
                    StandingId = table.Column<long>(type: "bigint", nullable: true),
                    FinishedRaces = table.Column<int>(type: "int", nullable: true),
                    IsSeasonFinished = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    ImportSource = table.Column<string>(type: "longtext", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    FirstDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticSets", x => new { x.LeagueId, x.Id });
                    table.UniqueConstraint("AK_StatisticSets_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatisticSets_Members_CurrentChampId",
                        column: x => x.CurrentChampId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StatisticSets_Seasons_LeagueId_SeasonId",
                        columns: x => new { x.LeagueId, x.SeasonId },
                        principalTable: "Seasons",
                        principalColumns: new[] { "LeagueId", "SeasonId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoredTeamResultRowsResultRows",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    TeamResultRowRefId = table.Column<long>(type: "bigint", nullable: false),
                    TeamParentRowRefId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoredTeamResultRowsResultRows", x => new { x.TeamParentRowRefId, x.LeagueId, x.TeamResultRowRefId });
                    table.ForeignKey(
                        name: "FK_ScoredTeamResultRowsResultRows_ScoredResultRows_LeagueId_Te~1",
                        columns: x => new { x.LeagueId, x.TeamResultRowRefId },
                        principalTable: "ScoredResultRows",
                        principalColumns: new[] { "LeagueId", "ScoredResultRowId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoredTeamResultRowsResultRows_ScoredResultRows_LeagueId_Tea~",
                        columns: x => new { x.LeagueId, x.TeamParentRowRefId },
                        principalTable: "ScoredResultRows",
                        principalColumns: new[] { "LeagueId", "ScoredResultRowId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandingRows",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    StandingRowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StandingId = table.Column<long>(type: "bigint", nullable: false),
                    MemberId = table.Column<long>(type: "bigint", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: false),
                    LastPosition = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    CarClass = table.Column<string>(type: "longtext", nullable: true),
                    RacePoints = table.Column<int>(type: "int", nullable: false),
                    RacePointsChange = table.Column<int>(type: "int", nullable: false),
                    PenaltyPoints = table.Column<int>(type: "int", nullable: false),
                    PenaltyPointsChange = table.Column<int>(type: "int", nullable: false),
                    TotalPoints = table.Column<int>(type: "int", nullable: false),
                    TotalPointsChange = table.Column<int>(type: "int", nullable: false),
                    Races = table.Column<int>(type: "int", nullable: false),
                    RacesCounted = table.Column<int>(type: "int", nullable: false),
                    DroppedResultCount = table.Column<int>(type: "int", nullable: false),
                    CompletedLaps = table.Column<int>(type: "int", nullable: false),
                    CompletedLapsChange = table.Column<int>(type: "int", nullable: false),
                    LeadLaps = table.Column<int>(type: "int", nullable: false),
                    LeadLapsChange = table.Column<int>(type: "int", nullable: false),
                    FastestLaps = table.Column<int>(type: "int", nullable: false),
                    FastestLapsChange = table.Column<int>(type: "int", nullable: false),
                    PolePositions = table.Column<int>(type: "int", nullable: false),
                    PolePositionsChange = table.Column<int>(type: "int", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    WinsChange = table.Column<int>(type: "int", nullable: false),
                    Top3 = table.Column<int>(type: "int", nullable: false),
                    Top5 = table.Column<int>(type: "int", nullable: false),
                    Top10 = table.Column<int>(type: "int", nullable: false),
                    Incidents = table.Column<int>(type: "int", nullable: false),
                    IncidentsChange = table.Column<int>(type: "int", nullable: false),
                    PositionChange = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandingRows", x => new { x.LeagueId, x.StandingRowId });
                    table.UniqueConstraint("AK_StandingRows_StandingRowId", x => x.StandingRowId);
                    table.ForeignKey(
                        name: "FK_StandingRows_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StandingRows_Standings_LeagueId_StandingId",
                        columns: x => new { x.LeagueId, x.StandingId },
                        principalTable: "Standings",
                        principalColumns: new[] { "LeagueId", "StandingId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandingRows_Teams_LeagueId_TeamId",
                        columns: x => new { x.LeagueId, x.TeamId },
                        principalTable: "Teams",
                        principalColumns: new[] { "LeagueId", "TeamId" });
                });

            migrationBuilder.CreateTable(
                name: "StandingRows_ScoredResultRows",
                columns: table => new
                {
                    LeagueId = table.Column<long>(type: "bigint", nullable: false),
                    StandingRowRefId = table.Column<long>(type: "bigint", nullable: false),
                    ScoredResultRowRefId = table.Column<long>(type: "bigint", nullable: false),
                    IsScored = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandingRows_ScoredResultRows", x => new { x.ScoredResultRowRefId, x.LeagueId, x.StandingRowRefId });
                    table.ForeignKey(
                        name: "FK_StandingRows_ScoredResultRows_ScoredResultRows_LeagueId_Scor~",
                        columns: x => new { x.LeagueId, x.ScoredResultRowRefId },
                        principalTable: "ScoredResultRows",
                        principalColumns: new[] { "LeagueId", "ScoredResultRowId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandingRows_ScoredResultRows_StandingRows_LeagueId_Standing~",
                        columns: x => new { x.LeagueId, x.StandingRowRefId },
                        principalTable: "StandingRows",
                        principalColumns: new[] { "LeagueId", "StandingRowId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedReviewVotes_LeagueId_ReviewId",
                table: "AcceptedReviewVotes",
                columns: new[] { "LeagueId", "ReviewId" });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedReviewVotes_LeagueId_VoteCategoryId",
                table: "AcceptedReviewVotes",
                columns: new[] { "LeagueId", "VoteCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedReviewVotes_MemberAtFaultId",
                table: "AcceptedReviewVotes",
                column: "MemberAtFaultId");

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedReviewVotes_ReviewId",
                table: "AcceptedReviewVotes",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedReviewVotes_VoteCategoryId",
                table: "AcceptedReviewVotes",
                column: "VoteCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AddPenaltys_LeagueId_ScoredResultRowId",
                table: "AddPenaltys",
                columns: new[] { "LeagueId", "ScoredResultRowId" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverStatisticRows_LeagueId_FirstRaceId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "FirstRaceId" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverStatisticRows_LeagueId_FirstResultRowId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "FirstResultRowId" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverStatisticRows_LeagueId_FirstSessionId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "FirstSessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverStatisticRows_LeagueId_LastRaceId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "LastRaceId" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverStatisticRows_LeagueId_LastResultRowId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "LastResultRowId" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverStatisticRows_LeagueId_LastSessionId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "LastSessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverStatisticRows_MemberId",
                table: "DriverStatisticRows",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverStatisticRows_StatisticSetId",
                table: "DriverStatisticRows",
                column: "StatisticSetId");

            migrationBuilder.CreateIndex(
                name: "IX_EventResultConfigs_LeagueId_EventRefId",
                table: "EventResultConfigs",
                columns: new[] { "LeagueId", "EventRefId" });

            migrationBuilder.CreateIndex(
                name: "IX_EventResultConfigs_LeagueId_ResultConfigRefId",
                table: "EventResultConfigs",
                columns: new[] { "LeagueId", "ResultConfigRefId" });

            migrationBuilder.CreateIndex(
                name: "IX_EventResults_EventId",
                table: "EventResults",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_LeagueId_ScheduleId",
                table: "Events",
                columns: new[] { "LeagueId", "ScheduleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_TrackId",
                table: "Events",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterConditions_LeagueId_FilterOptionId",
                table: "FilterConditions",
                columns: new[] { "LeagueId", "FilterOptionId" });

            migrationBuilder.CreateIndex(
                name: "IX_FilterOptions_LeagueId_PointFilterResultConfigId",
                table: "FilterOptions",
                columns: new[] { "LeagueId", "PointFilterResultConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_FilterOptions_LeagueId_ResultFilterResultConfigId",
                table: "FilterOptions",
                columns: new[] { "LeagueId", "ResultFilterResultConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReviews_LeagueId_SessionId",
                table: "IncidentReviews",
                columns: new[] { "LeagueId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReviews_SessionId",
                table: "IncidentReviews",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReviewsInvolvedMembers_InvolvedReviewsLeagueId_Invol~",
                table: "IncidentReviewsInvolvedMembers",
                columns: new[] { "InvolvedReviewsLeagueId", "InvolvedReviewsReviewId" });

            migrationBuilder.CreateIndex(
                name: "IX_IRSimSessionDetails_LeagueId_EventId",
                table: "IRSimSessionDetails",
                columns: new[] { "LeagueId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueMembers_LeagueId_TeamId",
                table: "LeagueMembers",
                columns: new[] { "LeagueId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueMembers_MemberId",
                table: "LeagueMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueStatisticSetsStatisticSets_LeagueStatisticSetsLeagueId~",
                table: "LeagueStatisticSetsStatisticSets",
                columns: new[] { "LeagueStatisticSetsLeagueId", "LeagueStatisticSetsId" });

            migrationBuilder.CreateIndex(
                name: "IX_ResultConfigurations_LeagueId_SourceResultConfigId",
                table: "ResultConfigurations",
                columns: new[] { "LeagueId", "SourceResultConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_ResultRows_LeagueId_MemberId",
                table: "ResultRows",
                columns: new[] { "LeagueId", "MemberId" });

            migrationBuilder.CreateIndex(
                name: "IX_ResultRows_LeagueId_SubSessionId",
                table: "ResultRows",
                columns: new[] { "LeagueId", "SubSessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_ResultRows_LeagueId_TeamId",
                table: "ResultRows",
                columns: new[] { "LeagueId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_ResultRows_MemberId",
                table: "ResultRows",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewComments_LeagueId_ReplyToCommentId",
                table: "ReviewComments",
                columns: new[] { "LeagueId", "ReplyToCommentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewComments_LeagueId_ReviewId",
                table: "ReviewComments",
                columns: new[] { "LeagueId", "ReviewId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewComments_ReplyToCommentId",
                table: "ReviewComments",
                column: "ReplyToCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewComments_ReviewId",
                table: "ReviewComments",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCommentVotes_CommentId",
                table: "ReviewCommentVotes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCommentVotes_LeagueId_CommentId",
                table: "ReviewCommentVotes",
                columns: new[] { "LeagueId", "CommentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCommentVotes_LeagueId_VoteCategoryId",
                table: "ReviewCommentVotes",
                columns: new[] { "LeagueId", "VoteCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCommentVotes_MemberAtFaultId",
                table: "ReviewCommentVotes",
                column: "MemberAtFaultId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCommentVotes_VoteCategoryId",
                table: "ReviewCommentVotes",
                column: "VoteCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPenaltys_LeagueId_ResultRowId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ResultRowId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPenaltys_LeagueId_ReviewId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ReviewId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPenaltys_LeagueId_ReviewVoteId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ReviewVoteId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPenaltys_ReviewId",
                table: "ReviewPenaltys",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewPenaltys_ReviewVoteId",
                table: "ReviewPenaltys",
                column: "ReviewVoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_LeagueId_SeasonId",
                table: "Schedules",
                columns: new[] { "LeagueId", "SeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredEventResults_EventResultEntityLeagueId_EventResultEnti~",
                table: "ScoredEventResults",
                columns: new[] { "EventResultEntityLeagueId", "EventResultEntityEventId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredEventResults_LeagueId_EventId",
                table: "ScoredEventResults",
                columns: new[] { "LeagueId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredEventResults_LeagueId_ResultConfigId",
                table: "ScoredEventResults",
                columns: new[] { "LeagueId", "ResultConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredResultRows_LeagueId_SessionResultId",
                table: "ScoredResultRows",
                columns: new[] { "LeagueId", "SessionResultId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredResultRows_LeagueId_TeamId",
                table: "ScoredResultRows",
                columns: new[] { "LeagueId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredResultRows_MemberId",
                table: "ScoredResultRows",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoredResultRows_TeamId",
                table: "ScoredResultRows",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoredResultsCleanestDrivers_CleanestDriverResultsLeagueId_C~",
                table: "ScoredResultsCleanestDrivers",
                columns: new[] { "CleanestDriverResultsLeagueId", "CleanestDriverResultsSessionResultId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredResultsHardChargers_HardChargerResultsLeagueId_HardCha~",
                table: "ScoredResultsHardChargers",
                columns: new[] { "HardChargerResultsLeagueId", "HardChargerResultsSessionResultId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredSessionResults_FastestAvgLapDriverMemberId",
                table: "ScoredSessionResults",
                column: "FastestAvgLapDriverMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoredSessionResults_FastestLapDriverMemberId",
                table: "ScoredSessionResults",
                column: "FastestLapDriverMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoredSessionResults_FastestQualyLapDriverMemberId",
                table: "ScoredSessionResults",
                column: "FastestQualyLapDriverMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoredSessionResults_LeagueId_ResultId",
                table: "ScoredSessionResults",
                columns: new[] { "LeagueId", "ResultId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredSessionResults_LeagueId_ScoringId",
                table: "ScoredSessionResults",
                columns: new[] { "LeagueId", "ScoringId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredTeamResultRowsResultRows_LeagueId_TeamParentRowRefId",
                table: "ScoredTeamResultRowsResultRows",
                columns: new[] { "LeagueId", "TeamParentRowRefId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoredTeamResultRowsResultRows_LeagueId_TeamResultRowRefId",
                table: "ScoredTeamResultRowsResultRows",
                columns: new[] { "LeagueId", "TeamResultRowRefId" });

            migrationBuilder.CreateIndex(
                name: "IX_Scorings_ExtScoringSourceId",
                table: "Scorings",
                column: "ExtScoringSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Scorings_LeagueId_ExtScoringSourceId",
                table: "Scorings",
                columns: new[] { "LeagueId", "ExtScoringSourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Scorings_LeagueId_PointsRuleId",
                table: "Scorings",
                columns: new[] { "LeagueId", "PointsRuleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Scorings_LeagueId_ResultConfigId",
                table: "Scorings",
                columns: new[] { "LeagueId", "ResultConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_Scorings_ScheduleEntityLeagueId_ScheduleEntityScheduleId",
                table: "Scorings",
                columns: new[] { "ScheduleEntityLeagueId", "ScheduleEntityScheduleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_MainScoringLeagueId_MainScoringScoringId",
                table: "Seasons",
                columns: new[] { "MainScoringLeagueId", "MainScoringScoringId" });

            migrationBuilder.CreateIndex(
                name: "IX_SessionResults_LeagueId_EventId",
                table: "SessionResults",
                columns: new[] { "LeagueId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_SessionResults_LeagueId_IRSimSessionDetailsId",
                table: "SessionResults",
                columns: new[] { "LeagueId", "IRSimSessionDetailsId" });

            migrationBuilder.CreateIndex(
                name: "IX_SessionResults_SessionId",
                table: "SessionResults",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_EventId_SessionId",
                table: "Sessions",
                columns: new[] { "EventId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_LeagueId_EventId",
                table: "Sessions",
                columns: new[] { "LeagueId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_StandingConfigs_ResultConfigs_LeagueId_ResultConfigId",
                table: "StandingConfigs_ResultConfigs",
                columns: new[] { "LeagueId", "ResultConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_StandingConfigs_ResultConfigs_LeagueId_StandingConfigId",
                table: "StandingConfigs_ResultConfigs",
                columns: new[] { "LeagueId", "StandingConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_StandingRows_LeagueId_StandingId",
                table: "StandingRows",
                columns: new[] { "LeagueId", "StandingId" });

            migrationBuilder.CreateIndex(
                name: "IX_StandingRows_LeagueId_TeamId",
                table: "StandingRows",
                columns: new[] { "LeagueId", "TeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_StandingRows_MemberId",
                table: "StandingRows",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_StandingRows_ScoredResultRows_LeagueId_ScoredResultRowRefId",
                table: "StandingRows_ScoredResultRows",
                columns: new[] { "LeagueId", "ScoredResultRowRefId" });

            migrationBuilder.CreateIndex(
                name: "IX_StandingRows_ScoredResultRows_LeagueId_StandingRowRefId",
                table: "StandingRows_ScoredResultRows",
                columns: new[] { "LeagueId", "StandingRowRefId" });

            migrationBuilder.CreateIndex(
                name: "IX_Standings_LeagueId_EventId",
                table: "Standings",
                columns: new[] { "LeagueId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_Standings_LeagueId_SeasonId",
                table: "Standings",
                columns: new[] { "LeagueId", "SeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_Standings_LeagueId_StandingConfigId",
                table: "Standings",
                columns: new[] { "LeagueId", "StandingConfigId" });

            migrationBuilder.CreateIndex(
                name: "IX_StatisticSets_CurrentChampId",
                table: "StatisticSets",
                column: "CurrentChampId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticSets_LeagueId_SeasonId",
                table: "StatisticSets",
                columns: new[] { "LeagueId", "SeasonId" });

            migrationBuilder.CreateIndex(
                name: "IX_StatisticSets_LeagueId_StandingId",
                table: "StatisticSets",
                columns: new[] { "LeagueId", "StandingId" });

            migrationBuilder.CreateIndex(
                name: "IX_TrackConfigs_TrackGroupId",
                table: "TrackConfigs",
                column: "TrackGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcceptedReviewVotes_IncidentReviews_LeagueId_ReviewId",
                table: "AcceptedReviewVotes",
                columns: new[] { "LeagueId", "ReviewId" },
                principalTable: "IncidentReviews",
                principalColumns: new[] { "LeagueId", "ReviewId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddPenaltys_ScoredResultRows_LeagueId_ScoredResultRowId",
                table: "AddPenaltys",
                columns: new[] { "LeagueId", "ScoredResultRowId" },
                principalTable: "ScoredResultRows",
                principalColumns: new[] { "LeagueId", "ScoredResultRowId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DriverStatisticRows_ScoredResultRows_LeagueId_FirstResultRow~",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "FirstResultRowId" },
                principalTable: "ScoredResultRows",
                principalColumns: new[] { "LeagueId", "ScoredResultRowId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DriverStatisticRows_ScoredResultRows_LeagueId_LastResultRowId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "LastResultRowId" },
                principalTable: "ScoredResultRows",
                principalColumns: new[] { "LeagueId", "ScoredResultRowId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DriverStatisticRows_Sessions_LeagueId_FirstRaceId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "FirstRaceId" },
                principalTable: "Sessions",
                principalColumns: new[] { "LeagueId", "SessionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DriverStatisticRows_Sessions_LeagueId_FirstSessionId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "FirstSessionId" },
                principalTable: "Sessions",
                principalColumns: new[] { "LeagueId", "SessionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DriverStatisticRows_Sessions_LeagueId_LastRaceId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "LastRaceId" },
                principalTable: "Sessions",
                principalColumns: new[] { "LeagueId", "SessionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DriverStatisticRows_Sessions_LeagueId_LastSessionId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "LastSessionId" },
                principalTable: "Sessions",
                principalColumns: new[] { "LeagueId", "SessionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DriverStatisticRows_StatisticSets_LeagueId_StatisticSetId",
                table: "DriverStatisticRows",
                columns: new[] { "LeagueId", "StatisticSetId" },
                principalTable: "StatisticSets",
                principalColumns: new[] { "LeagueId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventResultConfigs_Events_LeagueId_EventRefId",
                table: "EventResultConfigs",
                columns: new[] { "LeagueId", "EventRefId" },
                principalTable: "Events",
                principalColumns: new[] { "LeagueId", "EventId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventResults_Events_LeagueId_EventId",
                table: "EventResults",
                columns: new[] { "LeagueId", "EventId" },
                principalTable: "Events",
                principalColumns: new[] { "LeagueId", "EventId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Schedules_LeagueId_ScheduleId",
                table: "Events",
                columns: new[] { "LeagueId", "ScheduleId" },
                principalTable: "Schedules",
                principalColumns: new[] { "LeagueId", "ScheduleId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueStatisticSetsStatisticSets_StatisticSets_DependendStat~",
                table: "LeagueStatisticSetsStatisticSets",
                columns: new[] { "DependendStatisticSetsLeagueId", "DependendStatisticSetsId" },
                principalTable: "StatisticSets",
                principalColumns: new[] { "LeagueId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueStatisticSetsStatisticSets_StatisticSets_LeagueStatist~",
                table: "LeagueStatisticSetsStatisticSets",
                columns: new[] { "LeagueStatisticSetsLeagueId", "LeagueStatisticSetsId" },
                principalTable: "StatisticSets",
                principalColumns: new[] { "LeagueId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewPenaltys_ScoredResultRows_LeagueId_ResultRowId",
                table: "ReviewPenaltys",
                columns: new[] { "LeagueId", "ResultRowId" },
                principalTable: "ScoredResultRows",
                principalColumns: new[] { "LeagueId", "ScoredResultRowId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Seasons_LeagueId_SeasonId",
                table: "Schedules",
                columns: new[] { "LeagueId", "SeasonId" },
                principalTable: "Seasons",
                principalColumns: new[] { "LeagueId", "SeasonId" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointRules_Leagues_LeagueId",
                table: "PointRules");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultConfigurations_Leagues_LeagueId",
                table: "ResultConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_Scorings_Leagues_LeagueId",
                table: "Scorings");

            migrationBuilder.DropForeignKey(
                name: "FK_Seasons_Leagues_LeagueId",
                table: "Seasons");

            migrationBuilder.DropForeignKey(
                name: "FK_Scorings_ResultConfigurations_LeagueId_ResultConfigId",
                table: "Scorings");

            migrationBuilder.DropForeignKey(
                name: "FK_Scorings_Schedules_ScheduleEntityLeagueId_ScheduleEntitySche~",
                table: "Scorings");

            migrationBuilder.DropTable(
                name: "AddPenaltys");

            migrationBuilder.DropTable(
                name: "CustomIncidents");

            migrationBuilder.DropTable(
                name: "DriverStatisticRows");

            migrationBuilder.DropTable(
                name: "EventResultConfigs");

            migrationBuilder.DropTable(
                name: "FilterConditions");

            migrationBuilder.DropTable(
                name: "IncidentReviewsInvolvedMembers");

            migrationBuilder.DropTable(
                name: "LeagueStatisticSetsStatisticSets");

            migrationBuilder.DropTable(
                name: "ResultRows");

            migrationBuilder.DropTable(
                name: "ReviewCommentVotes");

            migrationBuilder.DropTable(
                name: "ReviewPenaltys");

            migrationBuilder.DropTable(
                name: "ScoredResultsCleanestDrivers");

            migrationBuilder.DropTable(
                name: "ScoredResultsHardChargers");

            migrationBuilder.DropTable(
                name: "ScoredTeamResultRowsResultRows");

            migrationBuilder.DropTable(
                name: "StandingConfigs_ResultConfigs");

            migrationBuilder.DropTable(
                name: "StandingRows_ScoredResultRows");

            migrationBuilder.DropTable(
                name: "FilterOptions");

            migrationBuilder.DropTable(
                name: "StatisticSets");

            migrationBuilder.DropTable(
                name: "LeagueMembers");

            migrationBuilder.DropTable(
                name: "SessionResults");

            migrationBuilder.DropTable(
                name: "ReviewComments");

            migrationBuilder.DropTable(
                name: "AcceptedReviewVotes");

            migrationBuilder.DropTable(
                name: "ScoredResultRows");

            migrationBuilder.DropTable(
                name: "StandingRows");

            migrationBuilder.DropTable(
                name: "IRSimSessionDetails");

            migrationBuilder.DropTable(
                name: "IncidentReviews");

            migrationBuilder.DropTable(
                name: "VoteCategories");

            migrationBuilder.DropTable(
                name: "ScoredSessionResults");

            migrationBuilder.DropTable(
                name: "Standings");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "ScoredEventResults");

            migrationBuilder.DropTable(
                name: "StandingConfigurations");

            migrationBuilder.DropTable(
                name: "EventResults");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "TrackConfigs");

            migrationBuilder.DropTable(
                name: "TrackGroups");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropTable(
                name: "ResultConfigurations");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Scorings");

            migrationBuilder.DropTable(
                name: "PointRules");
        }
    }
}
