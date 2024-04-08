namespace iRLeagueDatabaseCore.Models;

public class StandingRowEntity
{
    public StandingRowEntity()
    {
        ResultRows = new HashSet<StandingRows_ScoredResultRows>();
    }

    public long LeagueId { get; set; }
    public long StandingRowId { get; set; }
    public long StandingId { get; set; }
    public long? MemberId { get; set; }
    public long? TeamId { get; set; }

    public int Position { get; set; }
    public int LastPosition { get; set; }
    public int ClassId { get; set; }
    public string CarClass { get; set; }
    public int RacePoints { get; set; }
    public int RacePointsChange { get; set; }
    public int PenaltyPoints { get; set; }
    public int PenaltyPointsChange { get; set; }
    public int TotalPoints { get; set; }
    public int TotalPointsChange { get; set; }
    public int Races { get; set; }
    public int RacesCounted { get; set; }
    public int RacesScored { get; set; }
    public int RacesInPoints { get; set; }
    public int DroppedResultCount { get; set; }
    public int CompletedLaps { get; set; }
    public int CompletedLapsChange { get; set; }
    public int LeadLaps { get; set; }
    public int LeadLapsChange { get; set; }
    public int FastestLaps { get; set; }
    public int FastestLapsChange { get; set; }
    public int PolePositions { get; set; }
    public int PolePositionsChange { get; set; }
    public int Wins { get; set; }
    public int WinsChange { get; set; }
    public int Top3 { get; set; }
    public int Top5 { get; set; }
    public int Top10 { get; set; }
    public int Incidents { get; set; }
    public int IncidentsChange { get; set; }
    public int PositionChange { get; set; }
    public int StartIrating { get; set; }
    public int LastIrating { get; set; }

    public virtual StandingEntity SeasonStanding { get; set; }
    public virtual MemberEntity Member { get; set; }
    public virtual TeamEntity Team { get; set; }
    public virtual ICollection<StandingRows_ScoredResultRows> ResultRows { get; set; }
}

public class StandingRowEntityConfiguration : IEntityTypeConfiguration<StandingRowEntity>
{
    public void Configure(EntityTypeBuilder<StandingRowEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.StandingRowId });

        entity.ToTable("StandingRows");

        entity.HasAlternateKey(e => e.StandingRowId);

        entity.Property(e => e.StandingRowId)
            .ValueGeneratedOnAdd();

        entity.HasOne(p => p.SeasonStanding)
            .WithMany(d => d.StandingRows)
            .HasForeignKey(p => new { p.LeagueId, p.StandingId });

        entity.HasOne(p => p.Member)
            .WithMany()
            .HasForeignKey(p => p.MemberId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .IsRequired(false);

        entity.HasOne(p => p.Team)
            .WithMany()
            .HasForeignKey(p => new { p.LeagueId, p.TeamId })
            .OnDelete(DeleteBehavior.ClientSetNull)
            .IsRequired(false);

        entity.HasMany(p => p.ResultRows)
            .WithOne(d => d.StandingRow)
            .HasForeignKey(d => new { d.LeagueId, d.StandingRowRefId });
    }
}
