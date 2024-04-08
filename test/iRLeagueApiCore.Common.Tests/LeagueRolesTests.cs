namespace iRLeagueApiCore.Common.Tests;

public class LeagueRolesTests
{
    [Theory]
    [InlineData(LeagueRoles.Owner, true, true, true, true, true)]
    [InlineData(LeagueRoles.Admin, false, true, true, true, true)]
    [InlineData(LeagueRoles.Organizer, false, false, true, false, true)]
    [InlineData(LeagueRoles.Steward, false, false, false, true, true)]
    [InlineData(LeagueRoles.Member, false, false, false, false, true)]
    public void ShouldHaveRoles(string roleName,
        bool owner,
        bool admin,
        bool organizer,
        bool steward,
        bool member)
    {
        // Setup
        var role = LeagueRoles.GetRoleValue(roleName);
        var roles = new[] { role };

        LeagueRoles.CheckRole(LeagueRoles.OwnerValue, roles).Should().Be(owner);
        LeagueRoles.CheckRole(LeagueRoles.AdminValue, roles).Should().Be(admin);
        LeagueRoles.CheckRole(LeagueRoles.OrganizerValue, roles).Should().Be(organizer);
        LeagueRoles.CheckRole(LeagueRoles.StewardValue, roles).Should().Be(steward);
        LeagueRoles.CheckRole(LeagueRoles.MemberValue, roles).Should().Be(member);
    }

    [Theory]
    [InlineData(LeagueRoles.Owner, new string[0])]
    [InlineData(LeagueRoles.Admin, new[] { LeagueRoles.Owner })]
    [InlineData(LeagueRoles.Organizer, new[] { LeagueRoles.Owner, LeagueRoles.Admin })]
    [InlineData(LeagueRoles.Steward, new[] { LeagueRoles.Owner, LeagueRoles.Admin })]
    [InlineData(LeagueRoles.Member, new[] { LeagueRoles.Owner, LeagueRoles.Admin, LeagueRoles.Organizer, LeagueRoles.Steward })]
    public void ShouldBeImplicitOf(string roleName, string[] implicitOfRoleNames)
    {
        // Setup
        var role = LeagueRoles.GetRoleValue(roleName);
        var implicitOfRoles = implicitOfRoleNames.Select(x => LeagueRoles.GetRoleValue(x)).ToArray();

        // Test
        var testImplicitOfRoles = LeagueRoles.ImplicitRoleOf(role);
        testImplicitOfRoles.Should().HaveSameCount(implicitOfRoles);
        if (implicitOfRoles.Length != 0)
        {
            testImplicitOfRoles.Should().Contain(implicitOfRoles);
        }
    }

    [Fact]
    public void ShouldConvertImplicitToString()
    {
        var role = LeagueRoles.GetRoleValue("Admin");
        string roleString = role;

        role.Equals(roleString).Should().BeTrue();
    }
}
