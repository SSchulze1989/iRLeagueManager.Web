using iRLeagueApiCore.Common;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Users;
using iRLeagueApiCore.UnitTests.Fixtures;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Users;
public sealed class AddLeagueRoleHandlerTests : UserHandlerTestsBase<AddLeagueRoleHandler, AddLeagueRoleRequest>
{
    public AddLeagueRoleHandlerTests(IdentityFixture identityFixture) : 
        base(identityFixture)
    {
    }

    [Fact]
    public async Task ShouldAddUserToRole()
    {
        var testUser = CreateTestUser();
        var testRole = CreateTestRole(identityFixture.testLeague);
        var request = CreateRequest(identityFixture.testLeague, testUser, testRole);
        var sut = CreateSut();

        await sut.Handle(request, default);

        identityFixture.UserRoles.Should().Contain(x => x.Value.Any(role => role.Id == testRole.Id));
    }

    [Fact]
    public async Task ShoulCreateNonExistingRole()
    {
        var testUser = CreateTestUser();
        var testRole = CreateTestRole(identityFixture.testLeague);
        identityFixture.Roles.Remove(testRole.Id);
        var request = CreateRequest(identityFixture.testLeague, testUser, testRole);
        var sut = CreateSut();

        identityFixture.Roles.Should().NotContain(x => x.Value.Name == testRole.Name);
        await sut.Handle(request, default);

        identityFixture.Roles.Should().Contain(x => x.Value.Name == testRole.Name);
    }

    [Fact]
    public async Task ShouldReturnUserWithRole()
    {
        var testUser = CreateTestUser();
        var testRole = CreateTestRole(identityFixture.testLeague);
        var request = CreateRequest(identityFixture.testLeague, testUser, testRole);
        var sut = CreateSut();

        var result = await sut.Handle(request, default);

        result.LeagueRoles.Should().Contain(LeagueRoles.GetRoleName(testRole.Name));
    }

    private AddLeagueRoleRequest CreateRequest(string leagueName, ApplicationUser user, IdentityRole role)
    {
        return new AddLeagueRoleRequest(leagueName, user.Id, LeagueRoles.GetRoleName(role.Name)!);
    }

    private AddLeagueRoleHandler CreateSut()
    {
        return new AddLeagueRoleHandler(logger, identityFixture.UserManager, identityFixture.RoleManager, validators);
    }
}
