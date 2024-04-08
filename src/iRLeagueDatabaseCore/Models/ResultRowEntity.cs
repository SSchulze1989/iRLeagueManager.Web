#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class ResultRowEntity : ResultRowBase
{
    public ResultRowEntity()
    {
    }

    public long LeagueId { get; set; }
    public long ResultRowId { get; set; }
    public long SubSessionId { get; set; }
    public long MemberId { get; set; }
    public long? TeamId { get; set; }
    public bool PointsEligible { get; set; }


    public virtual MemberEntity Member { get; set; }
    public virtual LeagueMemberEntity LeagueMember { get; set; }
    public virtual SessionResultEntity SubResult { get; set; }
    public virtual TeamEntity Team { get; set; }
}

public class ResultRowEntityConfiguration : IEntityTypeConfiguration<ResultRowEntity>
{
    public void Configure(EntityTypeBuilder<ResultRowEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.ResultRowId });
        entity.HasAlternateKey(e => e.ResultRowId);

        entity.Property(e => e.ResultRowId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.CarNumber).HasMaxLength(8);

        entity.Property(e => e.QualifyingTimeAt).HasColumnType("datetime");

        entity.Property(e => e.AvgLapTime).HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.FastestLapTime).HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.QualifyingTime).HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.Interval).HasConversion<TimeSpanToTicksConverter>();

        entity.HasOne(d => d.Member)
            .WithMany(p => p.ResultRows)
            .HasForeignKey(d => d.MemberId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        entity.HasOne(d => d.LeagueMember)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.MemberId });

        entity.HasOne(d => d.SubResult)
            .WithMany(p => p.ResultRows)
            .HasForeignKey(d => new { d.LeagueId, d.SubSessionId })
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.Team)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.TeamId })
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
