using FluentValidation;
using iRLeagueApiCore.Common;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueApiCore.Server.Models;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Leagues;

public sealed class PostLeagueDbTestFixture : HandlersTestsBase<PostLeagueHandler, PostLeagueRequest, LeagueModel>
{
    private const string postLeagueName = "PostLeague";

    private List<ApplicationUser> Users { get; init; }
    private Dictionary<string, IdentityRole> Roles { get; init; }
    private Dictionary<ApplicationUser, List<IdentityRole>> UserRoles { get; init; }
    private UserManager<ApplicationUser> TestUserManager { get; init; }
    private RoleManager<IdentityRole> TestRoleManager { get; init; }

    public PostLeagueDbTestFixture() : base()
    {
        Users = new() { new ApplicationUser() { UserName = testUserName, Id = testUserId } };
        Roles = new IdentityRole[]
        {
            new("Admin"),
            new(LeagueRoles.GetLeagueRoleName(testLeagueName, LeagueRoles.Admin)),
            new(LeagueRoles.GetLeagueRoleName(testLeagueName, LeagueRoles.Owner))
        }.ToDictionary(k => k.Id, v => v);
        UserRoles = new();
        TestUserManager = GetTestUserManager();
        TestRoleManager = GetTestRoleManager();
    }

    protected override PostLeagueHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PostLeagueRequest> validator)
    {
        return new PostLeagueHandler(logger, dbContext, new IValidator<PostLeagueRequest>[] { validator }, TestUserManager, TestRoleManager );
    }

    protected override PostLeagueRequest DefaultRequest()
    {
        var model = new PostLeagueModel()
        {
            Name = postLeagueName,
            NameFull = "Full test league name"
        };
        return CreateRequest(DefaultUser(), model);
    }

    protected override void DefaultAssertions(PostLeagueRequest request, LeagueModel result, LeagueDbContext dbContext)
    {
        var expected = request.Model;
        result.Id.Should().NotBe(0);
        result.Name.Should().Be(expected.Name);
        result.NameFull.Should().Be(expected.NameFull);
        AssertCreated(request.User, DateTime.UtcNow, result);
        base.DefaultAssertions(request, result, dbContext);
    }

    private PostLeagueRequest CreateRequest(LeagueUser user, PostLeagueModel model)
    {
        return new PostLeagueRequest(user, model);
    }

    [Fact]
    public override async Task<LeagueModel> ShouldHandleDefault()
    {
        var result = await base.ShouldHandleDefault();
        return result;
    }

    [Fact]
    public override async Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }

    [Fact]
    public async Task ShouldCreateOwnerRole()
    {
        await base.ShouldHandleDefault();
        var ownerRole = TestRoleManager.FindByNameAsync(LeagueRoles.GetLeagueRoleName(postLeagueName, LeagueRoles.Owner));
        ownerRole.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldAddUserToOwnerRole()
    {
        await base.ShouldHandleDefault();
        var user = await TestUserManager.FindByIdAsync(testUserId);
        var isInRole = await TestUserManager.IsInRoleAsync(user, LeagueRoles.GetLeagueRoleName(postLeagueName, LeagueRoles.Owner));
        isInRole.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldNotCreateLeague_WhenUserRoleActionFails()
    {
        var nonExistingUser = fixture.Create<LeagueUser>();
        var request = CreateRequest(nonExistingUser, DefaultRequest().Model);
        var handler = CreateTestHandler(dbContext, MockHelpers.TestValidator<PostLeagueRequest>());
        var handle = () => handler.Handle(request);
        await handle.Should().ThrowAsync<Exception>();

        // make sure league does not exist
        var league = dbContext.Leagues.FirstOrDefault(x => x.Name == postLeagueName);
        league.Should().BeNull();
    }

    private IRoleStore<IdentityRole> GetTestRoleStore()
    {
        return MockHelpers.TestRoleStore(Roles);
    }

    private IUserRoleStore<ApplicationUser> GetTestUserRoleStore()
    {
        var userStore = MockHelpers.TestUserStore(Users);
        var roleStore = GetTestRoleStore();
        return MockHelpers.TestUserRoleStore(userStore, roleStore, UserRoles);
    }

    private UserManager<ApplicationUser> GetTestUserManager()
    {
        var userStore = GetTestUserRoleStore();
        return MockHelpers.TestUserManager(userStore);
    }

    private RoleManager<IdentityRole> GetTestRoleManager()
    {
        var roleStore = GetTestRoleStore();
        return MockHelpers.TestRoleManager(roleStore);
    }
}
