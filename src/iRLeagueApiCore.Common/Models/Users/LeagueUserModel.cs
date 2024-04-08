namespace iRLeagueApiCore.Common.Models.Users;

/// <summary>
/// Information about user and the current league roles
/// Can either be retreived by an admin or the user itself
/// </summary>
[DataContract]
public class LeagueUserModel : UserModel
{

    [DataMember]
    public IEnumerable<string> LeagueRoles { get; set; } = Array.Empty<string>();
}
