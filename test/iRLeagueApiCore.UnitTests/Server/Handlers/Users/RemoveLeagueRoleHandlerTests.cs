
using iRLeagueApiCore.Common;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Users;
using iRLeagueApiCore.UnitTests.Fixtures;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Users;
public sealed class RemoveLeagueRoleHandlerTests : UserHandlerTestsBase<RemoveLeagueRoleHandler, RemoveLeagueRoleRequest>
{
    private string TestLeagueName => identityFixture.testLeague;

    public RemoveLeagueRoleHandlerTests(IdentityFixture identityFixture) : 
        base(identityFixture)
    {
    }

    [Fact]
    public async Task ShouldRemoveRoleFromUser()
    {
        var testUser = CreateTestUser();
        var testRole = CreateTestRole(TestLeagueName);
        identityFixture.UserRoles.Add(testUser, new() {testRole});
        var request = CreateRequest(TestLeagueName, testUser, testRole);
        var sut = CreateSut();

        identityFixture.UserRoles.Should().Contain(x => x.Key == testUser && x.Value.Any(role => role == testRole));
        await sut.Handle(request, default);

        identityFixture.UserRoles.Should().NotContain(x => x.Key == testUser && x.Value.Any(role => role == testRole));
    }

    [Fact]
    public async Task ShouldSucceed_WhenUserNotInRole()
    {
        var testUser = CreateTestUser();
        var testRole = CreateTestRole(TestLeagueName);
        var request = CreateRequest(TestLeagueName, testUser, testRole);
        var sut = CreateSut();

        await sut.Handle(request, default);
    }

    [Fact]
    public async Task ShouldReturnUserWithoutRole()
    {
        var testUser = CreateTestUser();
        var testRole = CreateTestRole(TestLeagueName);
        identityFixture.UserRoles.Add(testUser, new() { testRole });
        var request = CreateRequest(TestLeagueName, testUser, testRole);
        var sut = CreateSut();

        var result = await sut.Handle(request, default);

        result.LeagueRoles.Should().NotContain(LeagueRoles.GetRoleName(testRole.Name));
    }

    
    private RemoveLeagueRoleRequest CreateRequest(string leagueName, ApplicationUser user, IdentityRole role)
    {
        return new(leagueName, user.Id, LeagueRoles.GetRoleName(role.Name)!);
    }

    private RemoveLeagueRoleHandler CreateSut()
    {
        return new RemoveLeagueRoleHandler(logger, identityFixture.UserManager, validators);
    }
}
