namespace iRLeagueDatabaseCore.Models;

public class TrackGroupEntity
{
    public TrackGroupEntity()
    {
        TrackConfigs = new HashSet<TrackConfigEntity>();
    }

    public long TrackGroupId { get; set; }
    public string TrackName { get; set; }
    public string Location { get; set; }

    public virtual ICollection<TrackConfigEntity> TrackConfigs { get; set; }
}

public class TrackGroupEntityConfiguration : IEntityTypeConfiguration<TrackGroupEntity>
{
    public void Configure(EntityTypeBuilder<TrackGroupEntity> entity)
    {
        entity.HasKey(e => e.TrackGroupId);

        entity.Property(e => e.TrackGroupId)
            .ValueGeneratedOnAdd();

        entity.HasMany(d => d.TrackConfigs)
            .WithOne(p => p.TrackGroup)
            .HasForeignKey(d => d.TrackGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
