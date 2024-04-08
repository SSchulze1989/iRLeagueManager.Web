namespace iRLeagueDatabaseCore.Models;

public abstract class Revision
{
    public DateTime? CreatedOn { get; set; } = null;
    public DateTime? LastModifiedOn { get; set; } = null;

    public int Version { get; set; }

    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
}
