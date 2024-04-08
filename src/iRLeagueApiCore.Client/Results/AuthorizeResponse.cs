namespace iRLeagueApiCore.Client.Results;
public struct AuthorizeResponse
{
    public string AccessToken { get; set; }
    public DateTime Expires { get; set; }
}
