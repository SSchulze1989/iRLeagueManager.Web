using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Admin;
using iRLeagueApiCore.UnitTests.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Admin;

public sealed class GiveRoleRequestTests : IClassFixture<IdentityFixture>
{
    private readonly IdentityFixture fixture;
    private readonly ILogger<GiveRoleHandler> logger;

    public GiveRoleRequestTests(IdentityFixture fixture)
    {
        this.fixture = fixture;
        logger = Mock.Of<ILogger<GiveRoleHandler>>();
    }

    [Fact]
    public async Task GiveRoleValid()
    {
        var validator = fixture.GetDefaultValidator<GiveRoleRequest>().Object;
        var roleManager = fixture.GetDefaultMockRoleManager().Object;
        var roleAdded = false;
        var mockUserManager = fixture.GetDefaultMockUserManager();
        mockUserManager.Setup(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x == fixture.validUser), It.Is<string>(x => x == fixture.testLeagueRoleName)))
            .ReturnsAsync(() => { roleAdded = true; return IdentityResult.Success; });
        var userManager = mockUserManager.Object;

        var request = new GiveRoleRequest(fixture.testLeague,
            new UserRoleModel() { UserName = fixture.validUser.UserName, RoleName = fixture.testRole });
        request.User = fixture.validUser;
        var handler = new GiveRoleHandler(logger, validator.ToEnumerable(), userManager, roleManager);
        var result = await handler.Handle(request);

        Assert.True(roleAdded);
    }

    [Fact]
    public async Task GiveRoleValidationFailed()
    {
        var mockValidator = fixture.GetDefaultValidator<GiveRoleRequest>();
        mockValidator.Setup(x => x.ValidateAsync(It.IsAny<GiveRoleRequest>(), default))
            .ReturnsAsync(fixture.GetFailedResult());
        var validator = mockValidator.Object;
        var roleManager = fixture.GetDefaultMockRoleManager().Object;
        var userManager = fixture.GetDefaultMockUserManager().Object;

        var request = new GiveRoleRequest(fixture.testLeague, new UserRoleModel());
        var handler = new GiveRoleHandler(logger, validator.ToEnumerable(), userManager, roleManager);
        await Assert.ThrowsAsync<ValidationException>(async () => await handler.Handle(request));
    }

    [Fact]
    public async Task CreateRole()
    {
        var roleCreated = false;
        var roleAdded = false;

        var validator = fixture.GetDefaultValidator<GiveRoleRequest>().Object;
        var mockRoleManager = fixture.GetDefaultMockRoleManager();
        mockRoleManager.Setup(x => x.CreateAsync(It.Is<IdentityRole>(x => x.Name == fixture.testLeagueRoleName)))
            .ReturnsAsync(() => { roleCreated = true; return IdentityResult.Success; });
        mockRoleManager.Setup(x => x.RoleExistsAsync(It.Is<string>(x => x == fixture.testLeagueRoleName)))
            .ReturnsAsync(false);
        var roleManager = mockRoleManager.Object;
        var mockUserManager = fixture.GetDefaultMockUserManager();
        mockUserManager.Setup(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x == fixture.validUser), It.Is<string>(x => x == fixture.testLeagueRoleName)))
            .ReturnsAsync(() => { roleAdded = true; return IdentityResult.Success; });
        var userManager = mockUserManager.Object;

        var request = new GiveRoleRequest(fixture.testLeague,
            new UserRoleModel() { UserName = fixture.validUser.UserName, RoleName = fixture.testRole });
        request.User = fixture.validUser;
        var handler = new GiveRoleHandler(logger, validator.ToEnumerable(), userManager, roleManager);
        var result = await handler.Handle(request);

        Assert.True(roleAdded);
        Assert.True(roleCreated);
    }

    [Fact]
    public async Task CreateRoleFailed()
    {
        var roleCreated = false;
        var roleAdded = false;

        var validator = fixture.GetDefaultValidator<GiveRoleRequest>().Object;
        var mockRoleManager = fixture.GetDefaultMockRoleManager();
        mockRoleManager.Setup(x => x.CreateAsync(It.Is<IdentityRole>(x => x.Name == fixture.testLeagueRoleName)))
            .ReturnsAsync(() => { roleCreated = false; return IdentityResult.Failed(); });
        mockRoleManager.Setup(x => x.RoleExistsAsync(It.Is<string>(x => x == fixture.testLeagueRoleName)))
            .ReturnsAsync(false);
        var roleManager = mockRoleManager.Object;
        var mockUserManager = fixture.GetDefaultMockUserManager();
        mockUserManager.Setup(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x == fixture.validUser), It.Is<string>(x => x == fixture.testLeagueRoleName)))
            .ReturnsAsync(() => { roleAdded = true; return IdentityResult.Success; });
        var userManager = mockUserManager.Object;

        var request = new GiveRoleRequest(fixture.testLeague,
            new UserRoleModel() { UserName = fixture.validUser.UserName, RoleName = fixture.testRole });
        request.User = fixture.validUser;
        var handler = new GiveRoleHandler(logger, validator.ToEnumerable(), userManager, roleManager);
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.Handle(request));

        Assert.False(roleAdded);
        Assert.False(roleCreated);
    }

    [Fact]
    public async Task AddToRoleFailed()
    {
        var roleAdded = false;

        var validator = fixture.GetDefaultValidator<GiveRoleRequest>().Object;
        var roleManager = fixture.GetDefaultMockRoleManager().Object;
        var mockUserManager = fixture.GetDefaultMockUserManager();
        mockUserManager.Setup(x => x.AddToRoleAsync(It.Is<ApplicationUser>(x => x == fixture.validUser), It.Is<string>(x => x == fixture.testLeagueRoleName)))
            .ReturnsAsync(() => { roleAdded = false; return IdentityResult.Failed(); });
        var userManager = mockUserManager.Object;

        var request = new GiveRoleRequest(fixture.testLeague,
            new UserRoleModel() { UserName = fixture.validUser.UserName, RoleName = fixture.testRole });
        request.User = fixture.validUser;
        var handler = new GiveRoleHandler(logger, validator.ToEnumerable(), userManager, roleManager);
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.Handle(request));

        Assert.False(roleAdded);
    }
}
