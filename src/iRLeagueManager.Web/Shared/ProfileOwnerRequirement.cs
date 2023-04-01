using Microsoft.AspNetCore.Authorization;

namespace iRLeagueManager.Web.Shared;

public class ProfileOwnerRequirement : IAuthorizationRequirement
{
    public const string Policy = "UserIsOwner";
    public ProfileOwnerRequirement() { }

}