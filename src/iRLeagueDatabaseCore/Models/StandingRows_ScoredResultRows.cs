namespace iRLeagueDatabaseCore.Models;

public class StandingRows_ScoredResultRows
{
    public long LeagueId { get; set; }
    public long StandingRowRefId { get; set; }
    public long ScoredResultRowRefId { get; set; }
    public bool IsScored { get; set; }

    public virtual StandingRowEntity StandingRow { get; set; }
    public virtual ScoredResultRowEntity ScoredResultRow { get; set; }
}

public sealed class StandingRows_ScoredResultRowsConfiguration : IEntityTypeConfiguration<StandingRows_ScoredResultRows>
{
    public void Configure(EntityTypeBuilder<StandingRows_ScoredResultRows> entity)
    {
        entity.HasKey(e => new { e.ScoredResultRowRefId, e.LeagueId, e.StandingRowRefId });

        entity.HasOne(p => p.ScoredResultRow)
            .WithMany(d => d.StandingRows)
            .HasForeignKey(p => new { p.LeagueId, p.ScoredResultRowRefId });
    }
}
