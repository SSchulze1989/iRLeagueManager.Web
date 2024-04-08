namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class PostResultConfigModel
{
    [DataMember]
    public string Name { get; set; } = string.Empty;
    [DataMember]
    public string DisplayName { get; set; } = string.Empty;
    [DataMember]
    public ResultConfigInfoModel? SourceResultConfig { get; set; }
    [DataMember]
    public int ResultsPerTeam { get; set; }
    [DataMember]
    public IList<ScoringModel> Scorings { get; set; } = new List<ScoringModel>();
    [DataMember]
    public ICollection<ResultFilterModel> FiltersForPoints { get; set; } = new List<ResultFilterModel>();
    [DataMember]
    public ICollection<ResultFilterModel> FiltersForResult { get; set; } = new List<ResultFilterModel>();
}
