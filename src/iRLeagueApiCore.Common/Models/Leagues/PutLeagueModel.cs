namespace iRLeagueApiCore.Common.Models;

/// <summary>
/// Schema for posting a new league
/// </summary>
[DataContract]
public class PutLeagueModel
{
    /// <summary>
    /// Full name of the league can contain any UTF-8 characters
    /// </summary>
    [DataMember]
    public string NameFull { get; set; } = string.Empty;
    /// <summary>
    /// Longtext description of the league. Will be displayed on the league welcome page in iRLeagueManager.Web application.
    /// Supports markdown
    /// </summary>
    [DataMember]
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Plain text version of league description
    /// </summary>
    [DataMember]
    public string DescriptionPlain { get; set; } = string.Empty;
    [DataMember]
    public bool EnableProtests { get; set; }
    /// <summary>
    /// Time span after a race has finished (according to event duration) after which a protest can be filed
    /// </summary>
    [DataMember]
    public TimeSpan ProtestCoolDownPeriod { get; set; }
    /// <summary>
    /// Time span after a race has finished (according to event duration) until a protest can be filed
    /// </summary>
    [DataMember]
    public TimeSpan ProtestsClosedAfter { get; set; }
    /// <summary>
    /// Set public visibility of protests
    /// </summary>
    [DataMember]
    public ProtestPublicSetting ProtestsPublic { get; set; }
    /// <summary>
    /// Set who can access the protest form and file protests
    /// </summary>
    [DataMember]
    public ProtestFormAccess ProtestFormAccess { get; set; }
    [DataMember]
    public LeaguePublicSetting LeaguePublic { get; set; }
    /// <summary>
    /// Enable access to protest form and reviews while race is running and before results have been uploaded
    /// </summary>
    [DataMember]
    public bool EnableLiveReviews { get; set; }
}
