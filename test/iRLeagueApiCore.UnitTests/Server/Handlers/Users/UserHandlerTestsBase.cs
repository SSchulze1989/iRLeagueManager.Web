using AutoFixture.Dsl;
using FluentValidation;
using iRLeagueApiCore.Common;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.UnitTests.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.Logging;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Users;
public abstract class UserHandlerTestsBase<THandler, TRequest> : IClassFixture<IdentityFixture>
{
    protected readonly Fixture fixture;
    protected readonly IdentityFixture identityFixture;
    protected readonly ILogger<THandler> logger;
    protected readonly IEnumerable<IValidator<TRequest>> validators;

    public UserHandlerTestsBase(IdentityFixture identityFixture)
    {
        this.identityFixture = identityFixture;
        fixture = new();
        logger = Mock.Of<ILogger<THandler>>();
        validators = Array.Empty<IValidator<TRequest>>();
        identityFixture.Setup();
    }

    protected IPostprocessComposer<ApplicationUser> UserBuilder()
    {
        return fixture.Build<ApplicationUser>()
            .With(x => x.FullName, () => GetFullName(fixture.Create<string>().Substring(0,10), fixture.Create<string>().Substring(0,10)));
    }

    protected string GetFullName(string firstname, string lastname)
    {
        return $"{firstname};{lastname}";
    }

    protected (string? firstname, string? lastname) GetFirstnameLastname(string? fullName)
    {
        var parts = fullName?.Split(';');
        return (parts?.ElementAtOrDefault(0), parts?.ElementAtOrDefault(1));
    }

    protected ApplicationUser CreateTestUser()
    {
        var user = UserBuilder().Create();
        identityFixture.Users.Add(user);
        return user;
    }

    protected IdentityRole CreateTestRole(string leagueName)
    {
        var roleName = LeagueRoles.GetLeagueRoleName(leagueName, LeagueRoles.Admin);
        var role = new IdentityRole(roleName);
        role.NormalizedName = MockHelpers.MockLookupNormalizer().NormalizeName(roleName);
        identityFixture.Roles.Add(role.Id, role);
        return role;
    }
}
