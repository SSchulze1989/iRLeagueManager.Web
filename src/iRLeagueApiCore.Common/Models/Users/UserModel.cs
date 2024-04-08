namespace iRLeagueApiCore.Common.Models.Users;

/// <summary>
/// User model containing basic user infos that can be viewed by everyone
/// </summary>
[DataContract]
public class UserModel
{
    [DataMember]
    public string UserId { get; set; } = string.Empty;
    [DataMember]
    public string UserName { get; set; } = string.Empty;
    [DataMember]
    public string Firstname { get; set; } = string.Empty;
    [DataMember]
    public string Lastname { get; set; } = string.Empty;
}
