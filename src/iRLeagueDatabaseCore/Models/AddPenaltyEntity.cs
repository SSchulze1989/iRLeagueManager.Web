using iRLeagueApiCore.Common.Converters;
using System.Text.Json;

namespace iRLeagueDatabaseCore.Models;

public partial class AddPenaltyEntity
{
    public long LeagueId { get; set; }
    public long AddPenaltyId { get; set; }
    public long ScoredResultRowId { get; set; }
    public string Lap { get; set; }
    public string Corner { get; set; }
    public string Reason { get; set; }
    public PenaltyValue Value { get; set; }

    public virtual ScoredResultRowEntity ScoredResultRow { get; set; }
}

public class AddPenaltyEntityConfiguration : IEntityTypeConfiguration<AddPenaltyEntity>
{
    public void Configure(EntityTypeBuilder<AddPenaltyEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.AddPenaltyId });

        entity.HasAlternateKey(e => e.AddPenaltyId);

        entity.Property(e => e.AddPenaltyId)
            .ValueGeneratedOnAdd();

        entity.HasIndex(e => new { e.LeagueId, e.ScoredResultRowId });

        entity.Property(e => e.Reason)
            .HasMaxLength(2048);

        entity.Property(e => e.Lap)
            .HasMaxLength(255);

        entity.Property(e => e.Corner)
            .HasMaxLength(255);

        entity.Property(e => e.Value)
            .HasColumnType("json")
            .HasConversion(
                v => JsonSerializer.Serialize(v, default(JsonSerializerOptions)),
                v => JsonSerializer.Deserialize<PenaltyValue>(v, default(JsonSerializerOptions)));

        entity.HasOne(d => d.ScoredResultRow)
            .WithMany(p => p.AddPenalties)
            .HasForeignKey(d => new { d.LeagueId, d.ScoredResultRowId })
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
