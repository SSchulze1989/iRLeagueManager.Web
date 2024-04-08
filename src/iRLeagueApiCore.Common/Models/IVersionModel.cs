namespace iRLeagueApiCore.Common.Models;

public interface IVersionModel
{
    /// <summary>
    /// Date of creation
    /// </summary>
    [DataMember]
    DateTime? CreatedOn { get; set; }
    /// <summary>
    /// Date of last modification
    /// </summary>
    [DataMember]
    DateTime? LastModifiedOn { get; set; }
    /// <summary>
    /// User id that created the entry
    /// </summary>
    [DataMember]
    string? CreatedByUserId { get; set; }
    /// <summary>
    /// User id that last modified the entry
    /// </summary>
    [DataMember]
    string? LastModifiedByUserId { get; set; }
    /// <summary>
    /// User name that created the entry
    /// </summary>
    [DataMember]
    string? CreatedByUserName { get; set; }
    /// <summary>
    /// User name that last modified the entry
    /// </summary>
    [DataMember]
    string? LastModifiedByUserName { get; set; }
}
