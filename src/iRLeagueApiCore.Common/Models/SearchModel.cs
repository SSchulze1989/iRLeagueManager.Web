namespace iRLeagueApiCore.Common.Models;

/// <summary>
/// Model providing data for search functionalty (e.g.: search user by name(s))
/// </summary>
[DataContract]
public class SearchModel
{
    /// <summary>
    /// Array of search keys to search a match for
    /// </summary>
    [DataMember]
    public IEnumerable<string> SearchKeys { get; set; } = Array.Empty<string>();
    /// <summary>
    /// If true -> only return sets that match all of the provided search keys
    /// <para>Default: <see langword="false"/></para>
    /// </summary>
    [DataMember]
    public bool MatchAll { get; set; } = false;
}
