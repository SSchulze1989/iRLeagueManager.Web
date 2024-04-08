namespace iRLeagueApiCore.Common.Models;

/// <summary>
/// Schema for fetching an existing league
/// </summary>
[DataContract]
public class LeagueModel : PostLeagueModel, IVersionModel
{
    /// <summary>
    /// Unique league id
    /// </summary>
    [DataMember]
    public long Id { get; set; }
    [DataMember]
    public bool IsInitialized { get; set; }
    /// <summary>
    /// Ids of seasons in this league
    /// </summary>
    [DataMember]
    public IEnumerable<long> SeasonIds { get; set; } = Array.Empty<long>();
    [DataMember(EmitDefaultValue = false)]
    public SubscriptionStatus? SubscriptionStatus { get; set; }
    [DataMember(EmitDefaultValue = false)]
    public DateTime? SubscriptionExpires { get; set; }

    #region version
    /// <summary>
    /// Date of creation
    /// </summary>
    [DataMember]
    public DateTime? CreatedOn { get; set; }
    /// <summary>
    /// Date of last modification
    /// </summary>
    [DataMember]
    public DateTime? LastModifiedOn { get; set; }
    /// <summary>
    /// User id that created the entry
    /// </summary>
    [DataMember]
    public string? CreatedByUserId { get; set; }
    /// <summary>
    /// User id that last modified the entry
    /// </summary>
    [DataMember]
    public string? LastModifiedByUserId { get; set; }
    /// <summary>
    /// User name that created the entry
    /// </summary>
    [DataMember]
    public string? CreatedByUserName { get; set; }
    /// <summary>
    /// User name that last modified the entry
    /// </summary>
    [DataMember]
    public string? LastModifiedByUserName { get; set; }
    #endregion
}
