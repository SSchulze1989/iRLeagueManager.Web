#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class ScoredResultRowEntity : ResultRowBase
{
    public ScoredResultRowEntity()
    {
        ReviewPenalties = new HashSet<ReviewPenaltyEntity>();
        TeamParentRows = new HashSet<ScoredResultRowEntity>();
        TeamResultRows = new HashSet<ScoredResultRowEntity>();
        StandingRows = new HashSet<StandingRows_ScoredResultRows>();
    }

    public ScoredResultRowEntity(ResultRowEntity resultRow)
        : this()
    {
        Member = resultRow.Member;
        Team = resultRow.Team;
        StartPosition = resultRow.StartPosition;
        FinishPosition = resultRow.FinishPosition;
        CarNumber = resultRow.CarNumber;
        ClassId = resultRow.ClassId;
        Car = resultRow.Car;
        CarClass = resultRow.CarClass;
        CompletedLaps = resultRow.CompletedLaps;
        LeadLaps = resultRow.LeadLaps;
        FastLapNr = resultRow.FastLapNr;
        Incidents = resultRow.Incidents;
        Status = resultRow.Status;
        QualifyingTime = resultRow.QualifyingTime;
        Interval = resultRow.Interval;
        AvgLapTime = resultRow.AvgLapTime;
        FastestLapTime = resultRow.FastestLapTime;
        PositionChange = resultRow.PositionChange;
        IRacingId = resultRow.IRacingId;
        SimSessionType = resultRow.SimSessionType;
        OldIRating = resultRow.OldIRating;
        NewIRating = resultRow.NewIRating;
        SeasonStartIRating = resultRow.SeasonStartIRating;
        License = resultRow.License;
        OldSafetyRating = resultRow.OldSafetyRating;
        NewSafetyRating = resultRow.NewSafetyRating;
        OldCpi = resultRow.OldCpi;
        NewCpi = resultRow.NewCpi;
        ClubId = resultRow.ClubId;
        ClubName = resultRow.ClubName;
        CarId = resultRow.CarId;
        CompletedPct = resultRow.CompletedPct;
        QualifyingTimeAt = resultRow.QualifyingTimeAt;
        Division = resultRow.Division;
        OldLicenseLevel = resultRow.OldLicenseLevel;
        NewLicenseLevel = resultRow.NewLicenseLevel;
        NumPitStops = resultRow.NumPitStops;
        PittedLaps = resultRow.PittedLaps;
        NumOfftrackLaps = resultRow.NumOfftrackLaps;
        OfftrackLaps = resultRow.OfftrackLaps;
        NumContactLaps = resultRow.NumContactLaps;
        ContactLaps = resultRow.ContactLaps;
    }

    public long LeagueId { get; set; }
    public long ScoredResultRowId { get; set; }
    public long SessionResultId { get; set; }
    public long? MemberId { get; set; }
    public long? TeamId { get; set; }
    /// <summary>
    /// Imported Id from old database
    /// Will be deleted after imports have finished
    /// </summary>
    public long? ImportId { get; set; }

    public double BonusPoints { get; set; }
    public double PenaltyPoints { get; set; }
    public int FinalPosition { get; set; }
    public double FinalPositionChange { get; set; }
    public double TotalPoints { get; set; }
    public bool PointsEligible { get; set; }

    public virtual MemberEntity Member { get; set; }
    public virtual TeamEntity Team { get; set; }
    public virtual ScoredSessionResultEntity ScoredSessionResult { get; set; }
    public virtual ICollection<AddPenaltyEntity> AddPenalties { get; set; }
    public virtual IEnumerable<ScoredResultRowEntity> TeamParentRows { get; set; }
    public virtual ICollection<ReviewPenaltyEntity> ReviewPenalties { get; set; }
    public virtual ICollection<ScoredResultRowEntity> TeamResultRows { get; set; }
    public virtual IEnumerable<StandingRows_ScoredResultRows> StandingRows { get; set; }
}

public class ScoredResultRowEntityConfiguration : IEntityTypeConfiguration<ScoredResultRowEntity>
{
    public void Configure(EntityTypeBuilder<ScoredResultRowEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.ScoredResultRowId });

        entity.HasAlternateKey(e => e.ScoredResultRowId);

        entity.Property(e => e.ScoredResultRowId)
            .ValueGeneratedOnAdd();

        entity.HasIndex(e => e.MemberId);

        entity.HasIndex(e => e.TeamId);

        entity.Property(e => e.CarNumber).HasMaxLength(8);

        entity.Property(e => e.QualifyingTimeAt).HasColumnType("datetime");

        entity.Property(e => e.AvgLapTime).HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.FastestLapTime).HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.QualifyingTime).HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.Interval).HasConversion<TimeSpanToTicksConverter>();

        entity.HasOne(d => d.Team)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.TeamId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        entity.HasOne(d => d.ScoredSessionResult)
            .WithMany(p => p.ScoredResultRows)
            .HasForeignKey(d => new { d.LeagueId, d.SessionResultId });

        entity.HasOne(d => d.Member)
            .WithMany()
            .HasForeignKey(d => d.MemberId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        entity.HasMany(d => d.TeamResultRows)
            .WithMany(p => p.TeamParentRows)
            .UsingEntity<ScoredTeamResultRowsResultRows>(
                left => left.HasOne(e => e.TeamResultRow)
                    .WithMany()
                    .HasForeignKey(e => new { e.LeagueId, e.TeamResultRowRefId }),
                right => right.HasOne(e => e.TeamParentRow)
                    .WithMany()
                    .HasForeignKey(e => new { e.LeagueId, e.TeamParentRowRefId }));

    }
}
