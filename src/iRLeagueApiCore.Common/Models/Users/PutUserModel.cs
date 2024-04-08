namespace iRLeagueApiCore.Common.Models.Users;

[DataContract]
public class PutUserModel
{
    [DataMember]
    public string Firstname { get; set; } = string.Empty;
    [DataMember]
    public string Lastname { get; set; } = string.Empty;
    [DataMember]
    public string Email { get; set; } = string.Empty;
    [DataMember]
    public bool HideFirstnameLastname { get; set; }
}
