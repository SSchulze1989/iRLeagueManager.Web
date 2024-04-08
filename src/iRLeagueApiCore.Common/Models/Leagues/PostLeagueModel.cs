namespace iRLeagueApiCore.Common.Models;

/// <summary>
/// Schema for updating an existing league
/// </summary>
[DataContract]
public class PostLeagueModel : PutLeagueModel
{
    /// <summary>
    /// Short name of the league
    /// <para/>Used to identify the league in queries
    /// <para/>Cannot contain spaces and only use characters: a-z A-Z 0-1 _ -
    /// </summary>
    [DataMember]
    public string Name { get; set; } = string.Empty;
}
