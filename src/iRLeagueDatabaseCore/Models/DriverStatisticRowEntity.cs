#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class DriverStatisticRowEntity
{
    public long StatisticSetId { get; set; }
    public long LeagueId { get; set; }
    public long MemberId { get; set; }
    public long? FirstResultRowId { get; set; }
    public long? LastResultRowId { get; set; }
    public int StartIRating { get; set; }
    public int EndIRating { get; set; }
    public double StartSRating { get; set; }
    public double EndSRating { get; set; }
    public long? FirstSessionId { get; set; }
    public DateTime? FirstSessionDate { get; set; }
    public long? FirstRaceId { get; set; }
    public DateTime? FirstRaceDate { get; set; }
    public long? LastSessionId { get; set; }
    public DateTime? LastSessionDate { get; set; }
    public long? LastRaceId { get; set; }
    public DateTime? LastRaceDate { get; set; }
    public double RacePoints { get; set; }
    public double TotalPoints { get; set; }
    public double BonusPoints { get; set; }
    public int Races { get; set; }
    public int Wins { get; set; }
    public int Poles { get; set; }
    public int Top3 { get; set; }
    public int Top5 { get; set; }
    public int Top10 { get; set; }
    public int Top15 { get; set; }
    public int Top20 { get; set; }
    public int Top25 { get; set; }
    public int RacesInPoints { get; set; }
    public int RacesCompleted { get; set; }
    public double Incidents { get; set; }
    public double PenaltyPoints { get; set; }
    public int FastestLaps { get; set; }
    public int IncidentsUnderInvestigation { get; set; }
    public int IncidentsWithPenalty { get; set; }
    public double LeadingLaps { get; set; }
    public double CompletedLaps { get; set; }
    public int CurrentSeasonPosition { get; set; }
    public double DrivenKm { get; set; }
    public double LeadingKm { get; set; }
    public double AvgFinishPosition { get; set; }
    public double AvgFinalPosition { get; set; }
    public double AvgStartPosition { get; set; }
    public double AvgPointsPerRace { get; set; }
    public double AvgIncidentsPerRace { get; set; }
    public double AvgIncidentsPerLap { get; set; }
    public double AvgIncidentsPerKm { get; set; }
    public double AvgPenaltyPointsPerRace { get; set; }
    public double AvgPenaltyPointsPerLap { get; set; }
    public double AvgPenaltyPointsPerKm { get; set; }
    public double AvgIRating { get; set; }
    public double AvgSRating { get; set; }
    public double BestFinishPosition { get; set; }
    public double WorstFinishPosition { get; set; }
    public double FirstRaceFinishPosition { get; set; }
    public double LastRaceFinishPosition { get; set; }
    public int BestFinalPosition { get; set; }
    public int WorstFinalPosition { get; set; }
    public int FirstRaceFinalPosition { get; set; }
    public int LastRaceFinalPosition { get; set; }
    public double BestStartPosition { get; set; }
    public double WorstStartPosition { get; set; }
    public double FirstRaceStartPosition { get; set; }
    public double LastRaceStartPosition { get; set; }
    public int Titles { get; set; }
    public int HardChargerAwards { get; set; }
    public int CleanestDriverAwards { get; set; }

    public virtual SessionEntity FirstRace { get; set; }
    public virtual ScoredResultRowEntity FirstResultRow { get; set; }
    public virtual SessionEntity FirstSession { get; set; }
    public virtual SessionEntity LastRace { get; set; }
    public virtual ScoredResultRowEntity LastResultRow { get; set; }
    public virtual SessionEntity LastSession { get; set; }
    public virtual MemberEntity Member { get; set; }
    public virtual StatisticSetEntity StatisticSet { get; set; }
}

public class DriverStatisticRowEntityConfiguration : IEntityTypeConfiguration<DriverStatisticRowEntity>
{
    public void Configure(EntityTypeBuilder<DriverStatisticRowEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.StatisticSetId, e.MemberId });

        entity.HasIndex(e => new { e.LeagueId, e.FirstRaceId });

        entity.HasIndex(e => new { e.LeagueId, e.FirstResultRowId });

        entity.HasIndex(e => new { e.LeagueId, e.FirstSessionId });

        entity.HasIndex(e => new { e.LeagueId, e.LastRaceId });

        entity.HasIndex(e => new { e.LeagueId, e.LastResultRowId });

        entity.HasIndex(e => new { e.LeagueId, e.LastSessionId });

        entity.HasIndex(e => e.MemberId);

        entity.HasIndex(e => e.StatisticSetId);

        entity.Property(e => e.FirstRaceDate)
            .HasColumnType("datetime");

        entity.Property(e => e.FirstSessionDate)
            .HasColumnType("datetime");

        entity.Property(e => e.LastRaceDate)
            .HasColumnType("datetime");

        entity.Property(e => e.LastSessionDate)
            .HasColumnType("datetime");

        entity.HasOne(d => d.FirstRace)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.FirstRaceId });

        entity.HasOne(d => d.FirstResultRow)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.FirstResultRowId });

        entity.HasOne(d => d.FirstSession)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.FirstSessionId });

        entity.HasOne(d => d.LastRace)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.LastRaceId });

        entity.HasOne(d => d.LastResultRow)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.LastResultRowId });

        entity.HasOne(d => d.LastSession)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.LastSessionId });

        entity.HasOne(d => d.Member)
            .WithMany(p => p.DriverStatisticRows)
            .HasForeignKey(d => d.MemberId);

        entity.HasOne(d => d.StatisticSet)
            .WithMany(p => p.DriverStatisticRows)
            .HasForeignKey(d => new { d.LeagueId, d.StatisticSetId });
    }
}
