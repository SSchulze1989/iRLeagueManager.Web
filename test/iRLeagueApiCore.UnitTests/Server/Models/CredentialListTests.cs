using iRLeagueApiCore.Server.Models;
using System.Net;

namespace iRLeagueApiCore.UnitTests.Server.Models;
public sealed class CredentialListTests
{
    private readonly Fixture fixture;
    private readonly Uri uri;
    private readonly string authenticationType;
    private readonly string username;
    private readonly string password;

    public CredentialListTests()
    {
        fixture = new();
        uri = new Uri("https://example.com");
        authenticationType = "BasicAuth";
        username = fixture.Create<string>();
        password = fixture.Create<string>();
    }

    [Fact]
    public void Add_ShouldAddAndReturnNewCredential()
    {
        var credential = new NetworkCredential(username, password);
        var sut = new CredentialList();

        sut.Add(uri, authenticationType, credential);

        var test = sut.GetCredential(uri, authenticationType);
        test.Should().NotBeNull();
        test!.UserName.Should().Be(username);
        test.Password.Should().Be(password);
    }

    [Fact]
    public void Add_ShouldUpdateExistingCredential()
    {
        var original = new NetworkCredential(username, password);
        var newPassword = fixture.Create<string>();
        var updated = new NetworkCredential(username, newPassword);
        var sut = new CredentialList();
        
        sut.Add(uri, authenticationType, original);
        sut.Add(uri, authenticationType , updated);

        var test = sut.GetCredential(uri, authenticationType);
        test.Should().NotBeNull();
        test!.UserName.Should().Be(username);
        test.Password.Should().Be(newPassword);
    }

    [Fact]
    public void Add_ShouldStoreDifferentCredentials_WhenUriIsDifferent()
    {
        var uri1 = fixture.Create<Uri>();
        var uri2 = fixture.Create<Uri>();
        var password1 = fixture.Create<string>();
        var password2 = fixture.Create<string>();
        var credential1 = new NetworkCredential(username, password1);
        var credential2 = new NetworkCredential(username, password2);
        var sut = new CredentialList();

        sut.Add(uri1, authenticationType, credential1);
        sut.Add(uri2, authenticationType , credential2);

        var test1 = sut.GetCredential(uri1, authenticationType);
        var test2 = sut.GetCredential(uri2, authenticationType);
        test1.Should().NotBeNull();
        test1!.Password.Should().Be(password1);
        test2.Should().NotBeNull();
        test2!.Password.Should().Be(password2);
    }

    [Fact]
    public void Remove_ShouldRemoveExistingCredential()
    {
        var credential = new NetworkCredential(username, password);
        var sut = new CredentialList();
        sut.Add(uri, authenticationType, credential);

        sut.Remove(uri, authenticationType);

        var test = sut.GetCredential(uri, authenticationType);
        test.Should().BeNull();
    }

    [Fact]
    public void Remove_ShouldNotThrow_WhenCredentialDoesNotExist()
    {
        var sut = new CredentialList();

        sut.Remove(uri, authenticationType);

        sut.GetCredential(uri, authenticationType);
    }
}
