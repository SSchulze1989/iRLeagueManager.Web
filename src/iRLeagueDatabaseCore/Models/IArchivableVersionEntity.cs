namespace iRLeagueDatabaseCore.Models;
public interface IArchivableVersionEntity : IVersionEntity
{
    public bool IsArchived { get; set; }
}
