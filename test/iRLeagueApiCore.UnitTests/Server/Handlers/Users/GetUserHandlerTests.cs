using FluentValidation;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Users;
using iRLeagueApiCore.UnitTests.Fixtures;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Users;
public sealed class GetUserHandlerTests : UserHandlerTestsBase<GetUserHandler, GetUserRequest>
{
    public GetUserHandlerTests(IdentityFixture identityFixture) : base(identityFixture)
    {
    }

    [Fact]
    public async Task ShouldReturn_WhenExists()
    {
        var testUser = UserBuilder().Create();
        identityFixture.Users.Add(testUser);
        var userManager = identityFixture.UserManager;
        var request = new GetUserRequest(testUser.Id);
        var sut = new GetUserHandler(logger, userManager, validators);

        var test = await sut.Handle(request, default);

        test.Should().NotBeNull();
        test!.UserId.Should().Be(testUser.Id);
        test.UserName.Should().Be(testUser.UserName);
    }

    [Fact]
    public async Task ShouldReturn_FirstnameAndLastname_WhenPublic()
    {
        var testUser = UserBuilder().Create();
        var (firstname, lastname) = GetFirstnameLastname(testUser.FullName);
        identityFixture.Users.Add(testUser);
        var userManager = identityFixture.UserManager;
        var request = new GetUserRequest(testUser.Id);
        var sut = new GetUserHandler(logger, userManager, validators);

        testUser.HideFullName = false;
        var test = await sut.Handle(request, default);

        test.Should().NotBeNull();
        test!.UserId.Should().Be(testUser.Id);
        test.UserName.Should().Be(testUser.UserName);
        test.Firstname.Should().Be(firstname);
        test.Lastname.Should().Be(lastname);
    }

    [Fact]
    public async Task ShouldReturn_FirstnameAndLastnameEmpty_WhenNotPublic()
    {
        var testUser = UserBuilder().Create();
        identityFixture.Users.Add(testUser);
        var userManager = identityFixture.UserManager;
        var request = new GetUserRequest(testUser.Id);
        var sut = new GetUserHandler(logger, userManager, validators);

        testUser.HideFullName = true;
        var test = await sut.Handle(request, default);

        test.Should().NotBeNull();
        test!.UserId.Should().Be(testUser.Id);
        test.UserName.Should().Be(testUser.UserName);
        test.Firstname.Should().BeEmpty();
        test.Lastname.Should().BeEmpty();
    }
}
