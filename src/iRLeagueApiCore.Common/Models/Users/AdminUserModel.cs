namespace iRLeagueApiCore.Common.Models.Users;

/// <summary>
/// User model that is only retrieved by admin functionality
/// Shows all roles the user is in
/// </summary>
[DataContract]
public class AdminUserModel : PrivateUserModel
{
    [DataMember]
    public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
}
