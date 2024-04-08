namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class PutScheduleModel
{
    [DataMember]
    public string Name { get; set; } = string.Empty;
}
