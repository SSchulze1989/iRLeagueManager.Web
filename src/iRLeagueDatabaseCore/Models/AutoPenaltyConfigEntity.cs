using iRLeagueApiCore.Common.Models;
using System.Text.Json;

namespace iRLeagueDatabaseCore.Models;
public partial class AutoPenaltyConfigEntity
{
    public AutoPenaltyConfigEntity()
    {
        Conditions = new HashSet<FilterConditionModel>();
    }

    public long LeagueId { get; set; }
    public long PenaltyConfigId { get; set; }
    public long PointRuleId { get; set; }

    public string Description { get; set; }
    public PenaltyType Type { get; set; }
    public int Points { get; set; }
    public TimeSpan Time { get; set; }
    public int Positions { get; set; }

    public virtual PointRuleEntity PointRule { get; set; }
    public virtual ICollection<FilterConditionModel> Conditions { get; set; }
}

public sealed class AutoPenaltyConfigEntityConfiguration : IEntityTypeConfiguration<AutoPenaltyConfigEntity>
{
    public void Configure(EntityTypeBuilder<AutoPenaltyConfigEntity> entity)
    {
        entity.ToTable("AutoPenaltyConfigs");

        entity.HasKey(e => new { e.LeagueId, e.PenaltyConfigId });

        entity.HasAlternateKey(e => e.PenaltyConfigId);

        entity.Property(e => e.PenaltyConfigId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Conditions)
            .HasColumnType("json")
            .HasConversion(
                v => JsonSerializer.Serialize(v, default(JsonSerializerOptions)),
                v => JsonSerializer.Deserialize<ICollection<FilterConditionModel>>(v, default(JsonSerializerOptions)),
                new ValueComparer<ICollection<FilterConditionModel>>(false))
            .IsRequired(true);

        entity.HasOne(d => d.PointRule)
            .WithMany(p => p.AutoPenalties)
            .HasForeignKey(d => new { d.LeagueId, d.PointRuleId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
