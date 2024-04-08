#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class CustomIncidentEntity
{
    public long IncidentId { get; set; }
    public long LeagueId { get; set; }
    public virtual LeagueEntity League { get; set; }
    public string Text { get; set; }
    public int Index { get; set; }
}

public class CustomIncidentEntityConfiguration : IEntityTypeConfiguration<CustomIncidentEntity>
{
    public void Configure(EntityTypeBuilder<CustomIncidentEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.IncidentId });

        entity.HasAlternateKey(e => e.IncidentId);

        entity.Property(e => e.IncidentId)
            .ValueGeneratedOnAdd();
    }
}
