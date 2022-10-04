namespace iRLeagueManager.Web.Shared
{
    public interface IModalBody
    {
        public Task<bool> CanSubmit();
    }
}
