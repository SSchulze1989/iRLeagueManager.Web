namespace iRLeagueDatabaseCore.Models;

public class EventResultConfigs
{
    public long LeagueId { get; set; }
    public long EventRefId { get; set; }
    public long ResultConfigRefId { get; set; }

    public virtual EventEntity Event { get; set; }
    public virtual ResultConfigurationEntity ResultConfig { get; set; }
}
