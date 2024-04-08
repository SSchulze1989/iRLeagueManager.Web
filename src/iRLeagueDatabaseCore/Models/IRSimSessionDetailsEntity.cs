#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class IRSimSessionDetailsEntity
{
    public long LeagueId { get; set; }
    public long EventId { get; set; }
    public long SessionDetailsId { get; set; }
    public long IRSubsessionId { get; set; }
    public long IRSeasonId { get; set; }
    public string IRSeasonName { get; set; }
    public int IRSeasonYear { get; set; }
    public int IRSeasonQuarter { get; set; }
    public int IRRaceWeek { get; set; }
    public long IRSessionId { get; set; }
    public int LicenseCategory { get; set; }
    public string SessionName { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int CornersPerLap { get; set; }
    public double KmDistPerLap { get; set; }
    public int MaxWeeks { get; set; }
    public int EventStrengthOfField { get; set; }
    public TimeSpan EventAverageLap { get; set; }
    public int EventLapsComplete { get; set; }
    public int NumCautions { get; set; }
    public int NumCautionLaps { get; set; }
    public int NumLeadChanges { get; set; }
    public int TimeOfDay { get; set; }
    public int DamageModel { get; set; }
    public int IRTrackId { get; set; }
    public string TrackName { get; set; }
    public string ConfigName { get; set; }
    public int TrackCategoryId { get; set; }
    public string Category { get; set; }
    public int WeatherType { get; set; }
    public int TempUnits { get; set; }
    public int TempValue { get; set; }
    public int RelHumidity { get; set; }
    public int Fog { get; set; }
    public int WindDir { get; set; }
    public int WindUnits { get; set; }
    public int Skies { get; set; }
    public int WeatherVarInitial { get; set; }
    public int WeatherVarOngoing { get; set; }
    public DateTime? SimStartUtcTime { get; set; }
    public TimeSpan SimStartUtcOffset { get; set; }
    public bool LeaveMarbles { get; set; }
    public int PracticeRubber { get; set; }
    public int QualifyRubber { get; set; }
    public int WarmupRubber { get; set; }
    public int RaceRubber { get; set; }
    public int PracticeGripCompound { get; set; }
    public int QualifyGripCompund { get; set; }
    public int WarmupGripCompound { get; set; }
    public int RaceGripCompound { get; set; }

    public virtual EventEntity Event { get; set; }
}

public class IRSimSessionDetailsEntityConfiguration : IEntityTypeConfiguration<IRSimSessionDetailsEntity>
{
    public void Configure(EntityTypeBuilder<IRSimSessionDetailsEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.SessionDetailsId });

        entity.HasAlternateKey(e => e.SessionDetailsId);

        entity.Property(e => e.SessionDetailsId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.EventAverageLap)
            .HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.EndTime)
            .HasColumnType("datetime");

        entity.Property(e => e.SimStartUtcOffset)
            .HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.SimStartUtcTime)
            .HasColumnType("datetime");

        entity.Property(e => e.StartTime)
            .HasColumnType("datetime");

        entity.HasOne(d => d.Event)
            .WithMany(p => p.SimSessionDetails)
            .HasForeignKey(d => new { d.LeagueId, d.EventId });
    }
}
