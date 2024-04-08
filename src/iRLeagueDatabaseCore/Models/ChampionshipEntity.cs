namespace iRLeagueDatabaseCore.Models;
public partial class ChampionshipEntity : IArchivableVersionEntity
{
    public ChampionshipEntity()
    {
        ChampSeasons = new HashSet<ChampSeasonEntity>();
    }

    public long LeagueId { get; set; }
    public long ChampionshipId { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }

    public virtual LeagueEntity League { get; set; }
    public virtual ICollection<ChampSeasonEntity> ChampSeasons { get; set; }

    #region version
    public bool IsArchived { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    #endregion
}

public sealed class ChampionshipEntityConfiguration : IEntityTypeConfiguration<ChampionshipEntity>
{
    public void Configure(EntityTypeBuilder<ChampionshipEntity> entity)
    {
        entity.ToTable("Championships");

        entity.HasKey(e => new { e.LeagueId, e.ChampionshipId });

        entity.HasAlternateKey(e => e.ChampionshipId);

        entity.Property(e => e.ChampionshipId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name).HasMaxLength(80);

        entity.Property(e => e.DisplayName).HasMaxLength(255);

        entity.HasOne(d => d.League)
            .WithMany(p => p.Championships)
            .HasForeignKey(d => d.LeagueId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
