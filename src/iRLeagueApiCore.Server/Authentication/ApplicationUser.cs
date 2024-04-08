using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Authentication;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public bool HideFullName { get; set; }
}
