namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class PostChampionshipModel
{
    [DataMember]
    public string Name { get; set; } = string.Empty;
    [DataMember]
    public string DisplayName { get; set; } = string.Empty;
}
