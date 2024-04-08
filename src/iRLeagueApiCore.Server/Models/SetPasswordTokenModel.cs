namespace iRLeagueApiCore.Server.Models;

public sealed class SetPasswordTokenModel
{
    public string PasswordToken { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
