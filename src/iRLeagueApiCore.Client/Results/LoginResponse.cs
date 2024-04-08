namespace iRLeagueApiCore.Client.Results;

public struct LoginResponse
{
    public string IdToken { get; set; }
    public string AccessToken { get; set; }
    public DateTime Expires { get; set; }
}
