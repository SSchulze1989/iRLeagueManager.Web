using FluentValidation;
using FluentValidation.Results;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Admin;
using iRLeagueApiCore.UnitTests.Fixtures;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Admin;

public sealed class ListUserDbTestFixture : IClassFixture<IdentityFixture>
{
    private readonly IdentityFixture fixture;
    private readonly ILogger<ListUsersHandler> logger;

    public ListUserDbTestFixture(IdentityFixture fixture)
    {
        this.fixture = fixture;
        logger = Mock.Of<ILogger<ListUsersHandler>>();
    }

    [Fact]
    public async Task RequestValid()
    {
        var validator = fixture.GetDefaultValidator<ListUsersRequest>().Object;
        var mockUserManager = fixture.GetDefaultMockUserManager();
        mockUserManager.Setup(x => x.GetUsersInRoleAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ApplicationUser>());
        mockUserManager.Setup(x => x.GetUsersInRoleAsync(It.Is<string>(x => x == fixture.testLeagueRoleName)))
            .ReturnsAsync(new List<ApplicationUser>() { fixture.validUser });
        var userManager = mockUserManager.Object;
        var request = new ListUsersRequest(fixture.testLeague);
        var handler = new ListUsersHandler(logger, new IValidator<ListUsersRequest>[] { validator },
            userManager);

        Debug.Assert((await userManager.GetUsersInRoleAsync(fixture.testLeagueRoleName)).Count == 1);

        var result = await handler.Handle(request, default);
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(fixture.validUser.UserName, result.First().UserName);
    }

    [Fact]
    public async Task RequestValidationFailed()
    {
        var mockValidator = fixture.GetDefaultValidator<ListUsersRequest>();
        mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ListUsersRequest>(), default))
            .ReturnsAsync(new ValidationResult(new ValidationFailure[] { new ValidationFailure(nameof(ListUsersRequest.LeagueName), "") }));
        var validator = mockValidator.Object;

        Debug.Assert((await validator.ValidateAsync(new ListUsersRequest(""))).IsValid == false);

        var handler = new ListUsersHandler(logger, new IValidator<ListUsersRequest>[] { validator }, fixture.GetDefaultMockUserManager().Object);
        await Assert.ThrowsAsync<ValidationException>(async () => await handler.Handle(new ListUsersRequest("")));
    }

    [Fact]
    public async Task RequestEmptyUserList()
    {
        var validator = fixture.GetDefaultValidator<ListUsersRequest>().Object;
        var mockUserManager = fixture.GetDefaultMockUserManager();
        mockUserManager.Setup(x => x.GetUsersInRoleAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<ApplicationUser>());
        var userManager = mockUserManager.Object;
        var request = new ListUsersRequest(fixture.testLeague);
        var handler = new ListUsersHandler(logger, new IValidator<ListUsersRequest>[] { validator },
            userManager);

        Debug.Assert((await userManager.GetUsersInRoleAsync(fixture.testLeagueRoleName)).Count == 0);

        var result = await handler.Handle(request, default);
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
