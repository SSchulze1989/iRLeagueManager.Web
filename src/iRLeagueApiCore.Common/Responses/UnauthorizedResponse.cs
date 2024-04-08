namespace iRLeagueApiCore.Common.Responses;
public struct UnauthorizedResponse
{
    public string Status { get; set; }
    public IEnumerable<object> Errors { get; set; }
}
