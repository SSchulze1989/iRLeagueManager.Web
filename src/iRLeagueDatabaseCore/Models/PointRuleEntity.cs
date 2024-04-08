using iRLeagueApiCore.Common.Models;
using System.Text.Json;

namespace iRLeagueDatabaseCore.Models;

public class PointRuleEntity : IVersionEntity
{
    public PointRuleEntity()
    {
        Scorings = new HashSet<ScoringEntity>();
        AutoPenalties = new HashSet<AutoPenaltyConfigEntity>();
        BonusPoints = new List<BonusPointModel>();
    }

    public long LeagueId { get; set; }
    public long PointRuleId { get; set; }

    public string Name { get; set; }
    public PointRuleType RuleType { get; set; }
    public ICollection<int> PointsPerPlace { get; set; }
    public ICollection<BonusPointModel> BonusPoints { get; set; }
    public string Formula { get; set; }
    public int MaxPoints { get; set; }
    public int PointDropOff { get; set; }
    public ICollection<SortOptions> PointsSortOptions { get; set; }
    public ICollection<SortOptions> FinalSortOptions { get; set; }

    public virtual LeagueEntity League { get; set; }
    public virtual IEnumerable<ScoringEntity> Scorings { get; set; }
    public virtual ICollection<AutoPenaltyConfigEntity> AutoPenalties { get; set; }

    #region version
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    #endregion
}

public class PointsRuleEntityConfiguration : IEntityTypeConfiguration<PointRuleEntity>
{
    public void Configure(EntityTypeBuilder<PointRuleEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.PointRuleId });

        entity.HasAlternateKey(e => e.PointRuleId);

        entity.Property(e => e.PointRuleId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.CreatedOn)
            .HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn)
            .HasColumnType("datetime");

        entity.Property(e => e.PointsPerPlace)
            .HasConversion(new CollectionToStringConverter<int>(), new ValueComparer<ICollection<int>>(true));

        entity.Property(e => e.BonusPoints)
            .HasColumnType("json")
            .HasConversion(
                v => JsonSerializer.Serialize(v, default(JsonSerializerOptions)),
                v => JsonSerializer.Deserialize<ICollection<BonusPointModel>>(v, default(JsonSerializerOptions)),
                new ValueComparer<ICollection<BonusPointModel>>(false))
            .IsRequired(true);

        entity.Property(e => e.PointsSortOptions)
            .HasConversion(new CollectionToStringConverter<SortOptions>(), new ValueComparer<ICollection<SortOptions>>(true));

        entity.Property(e => e.FinalSortOptions)
            .HasConversion(new CollectionToStringConverter<SortOptions>(), new ValueComparer<ICollection<SortOptions>>(true));

        entity.HasOne(d => d.League)
            .WithMany(p => p.PointRules)
            .HasForeignKey(d => d.LeagueId);
    }
}
