namespace iRLeagueApiCore.Common.Models.Reviews;

[DataContract]
public class ReviewCommentModel : PutReviewCommentModel, IVersionModel
{
    [DataMember]
    public long LeagueId { get; set; }
    [DataMember]
    public long CommentId { get; set; }
    [DataMember]
    public long ReviewId { get; set; }
    [DataMember]
    public DateTime? Date { get; set; }
    [DataMember]
    public string AuthorUserId { get; set; } = string.Empty;
    [DataMember]
    public string AuthorName { get; set; } = string.Empty;

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
