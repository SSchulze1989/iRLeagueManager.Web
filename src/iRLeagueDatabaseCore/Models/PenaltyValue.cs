using System.Text.Json.Serialization;

namespace iRLeagueDatabaseCore.Models;

public sealed class PenaltyValue
{
    public PenaltyType Type { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double Points { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TimeSpan Time { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Positions { get; set; }
}
