namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class PostPointRuleModel
{
    [DataMember]
    public string Name { get; set; } = string.Empty;
    [DataMember]
    public PointRuleType RuleType { get; set; }
    [DataMember]
    public IList<int> PointsPerPlace { get; set; } = new List<int>();
    [DataMember]
    public ICollection<BonusPointModel> BonusPoints { get; set; } = new List<BonusPointModel>();
    [DataMember]
    public string Formula { get; set; } = string.Empty;
    [DataMember]
    public int MaxPoints { get; set; }
    public int PointDropOff { get; set; }
    public ICollection<SortOptions> PointsSortOptions { get; set; } = new List<SortOptions>();
    public ICollection<SortOptions> FinalSortOptions { get; set; } = new List<SortOptions>();
    /// <summary>
    /// List of configurations for automated penalties
    /// </summary>
    [DataMember]
    public ICollection<AutoPenaltyConfiguration> AutoPenalties { get; set; } = new List<AutoPenaltyConfiguration>();
}
