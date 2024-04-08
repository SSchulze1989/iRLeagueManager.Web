namespace iRLeagueDatabaseCore.Models;

public class TrackConfigEntity
{
    public long TrackId { get; set; }
    public long TrackGroupId { get; set; }
    public virtual TrackGroupEntity TrackGroup { get; set; }
    public string ConfigName { get; set; }
    public double LengthKm { get; set; }
    public int Turns { get; set; }
    public ConfigType ConfigType { get; set; }
    public bool HasNightLighting { get; set; }
    public string LegacyTrackId { get; set; }
}

public class TrackConfigEntityConfiguration : IEntityTypeConfiguration<TrackConfigEntity>
{
    public void Configure(EntityTypeBuilder<TrackConfigEntity> entity)
    {
        entity.HasKey(e => e.TrackId);

        entity.Property(e => e.ConfigType)
            .HasConversion<string>();
    }
}
