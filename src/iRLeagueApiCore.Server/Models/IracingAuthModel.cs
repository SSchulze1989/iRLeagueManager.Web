using System.Runtime.Serialization;

namespace iRLeagueApiCore.Server.Models;

[DataContract]
public sealed class IracingAuthModel
{
    /// <summary>
    /// UserName (email) for use of authentication against iracing api 
    /// </summary>
    [DataMember]
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Passwor for use of authentication agains iracing api
    /// </summary>
    [DataMember]
    public string Password { get; set; } = string.Empty;
}
