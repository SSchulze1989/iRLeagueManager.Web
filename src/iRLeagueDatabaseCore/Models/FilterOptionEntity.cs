using iRLeagueApiCore.Common.Models;
using System.Text.Json;

namespace iRLeagueDatabaseCore.Models;

public partial class FilterOptionEntity : IVersionEntity
{
    public FilterOptionEntity()
    {
        Conditions = new HashSet<FilterConditionModel>();
    }

    public long LeagueId { get; set; }
    public long FilterOptionId { get; set; }
    public long? PointFilterResultConfigId { get; set; }
    public long? ResultFilterResultConfigId { get; set; }
    public long? ChampSeasonId { get; set; }

    #region version
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    #endregion
    public virtual ResultConfigurationEntity PointFilterResultConfig { get; set; }
    public virtual ResultConfigurationEntity ResultFilterResultConfig { get; set; }
    public virtual ChampSeasonEntity ChampSeason { get; set; }
    public virtual ICollection<FilterConditionModel> Conditions { get; set; }
}

public sealed class FilterOptionEntityConfiguration : IEntityTypeConfiguration<FilterOptionEntity>
{
    public void Configure(EntityTypeBuilder<FilterOptionEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.FilterOptionId });

        entity.HasAlternateKey(e => e.FilterOptionId);

        entity.Property(e => e.FilterOptionId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.CreatedOn).HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

        entity.Property(e => e.Conditions)
            .HasColumnType("json")
            .HasConversion(
                v => JsonSerializer.Serialize(v, default(JsonSerializerOptions)),
                v => JsonSerializer.Deserialize<ICollection<FilterConditionModel>>(v, default(JsonSerializerOptions)),
                new ValueComparer<ICollection<FilterConditionModel>>(false))
            .IsRequired(true);

        entity.HasOne(d => d.PointFilterResultConfig)
            .WithMany(p => p.PointFilters)
            .HasForeignKey(d => new { d.LeagueId, d.PointFilterResultConfigId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientCascade);

        entity.HasOne(d => d.ResultFilterResultConfig)
            .WithMany(p => p.ResultFilters)
            .HasForeignKey(d => new { d.LeagueId, d.ResultFilterResultConfigId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientCascade);

        entity.HasOne(d => d.ChampSeason)
            .WithMany(p => p.Filters)
            .HasForeignKey(d => new { d.LeagueId, d.ChampSeasonId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
