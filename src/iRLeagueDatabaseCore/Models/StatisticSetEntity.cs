#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class StatisticSetEntity : IVersionEntity
{
    public StatisticSetEntity()
    {
        DriverStatisticRows = new HashSet<DriverStatisticRowEntity>();
        LeagueStatisticSets = new HashSet<StatisticSetEntity>();
        DependendStatisticSets = new HashSet<StatisticSetEntity>();
    }

    public long LeagueId { get; set; }
    public long Id { get; set; }
    public string Name { get; set; }
    public long UpdateInterval { get; set; }
    public DateTime? UpdateTime { get; set; }
    public bool RequiresRecalculation { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    public long? CurrentChampId { get; set; }
    public long? SeasonId { get; set; }
    public long? StandingId { get; set; }
    public int? FinishedRaces { get; set; }
    public bool? IsSeasonFinished { get; set; }
    public string ImportSource { get; set; }
    public string Description { get; set; }
    public DateTime? FirstDate { get; set; }
    public DateTime? LastDate { get; set; }

    public virtual MemberEntity CurrentChamp { get; set; }
    public virtual SeasonEntity Season { get; set; }
    public virtual ICollection<DriverStatisticRowEntity> DriverStatisticRows { get; set; }
    public virtual ICollection<StatisticSetEntity> LeagueStatisticSets { get; set; }
    public virtual ICollection<StatisticSetEntity> DependendStatisticSets { get; set; }
}

public class StatisticSetEntityConfiguration : IEntityTypeConfiguration<StatisticSetEntity>
{
    public void Configure(EntityTypeBuilder<StatisticSetEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.Id });

        entity.HasAlternateKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.HasIndex(e => e.CurrentChampId);

        entity.HasIndex(e => new { e.LeagueId, e.StandingId });

        entity.HasIndex(e => new { e.LeagueId, e.SeasonId });

        entity.Property(e => e.CreatedOn).HasColumnType("datetime");

        entity.Property(e => e.FirstDate).HasColumnType("datetime");

        entity.Property(e => e.LastDate).HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

        entity.Property(e => e.UpdateTime).HasColumnType("datetime");

        entity.HasOne(d => d.CurrentChamp)
            .WithMany(p => p.StatisticSets)
            .HasForeignKey(d => d.CurrentChampId);

        entity.HasOne(d => d.Season)
            .WithMany(p => p.StatisticSets)
            .HasForeignKey(d => new { d.LeagueId, d.SeasonId })
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(d => d.DependendStatisticSets)
            .WithMany(p => p.LeagueStatisticSets)
            .UsingEntity(e => e.ToTable("LeagueStatisticSetsStatisticSets"));
    }
}
