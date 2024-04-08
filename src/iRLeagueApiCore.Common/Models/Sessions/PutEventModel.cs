namespace iRLeagueApiCore.Common.Models;

public class PutEventModel : PostEventModel
{
    /// <summary>
    /// Ids of ResultConfigurations to connect with this event
    /// </summary>
    ICollection<long> ResultConfigIds { get; set; } = new List<long>();
}