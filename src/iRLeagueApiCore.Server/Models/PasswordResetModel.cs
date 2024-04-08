namespace iRLeagueApiCore.Server.Models;

public sealed class PasswordResetModel
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string LinkUriTemplate { get; set; } = string.Empty;
}
