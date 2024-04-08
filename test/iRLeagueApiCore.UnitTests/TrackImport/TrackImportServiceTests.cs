using iRLeagueApiCore.TrackImport.Service;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace iRLeagueApiCore.UnitTests.TrackImport;

public sealed class TrackImportServiceTests
{
    private readonly IConfiguration configuration;
    private const string Skip = "Skip tests that hit external api";

    private const string testUserName = "CLunky@iracing.Com";
    private const string testPassword = "MyPassWord";
    private const string testPasswordEncoded = "xGKecAR27ALXNuMLsGaG0v5Q9pSs2tZTZRKNgmHMg+Q=";

    public TrackImportServiceTests()
    {
        configuration = ((IConfigurationBuilder)(new ConfigurationBuilder()))
            .AddUserSecrets<TrackImportServiceTests>()
            .Build();
    }
    [Fact]
    public void EncodePassword_ShouldEncodeTestHash_WhenUsingTestPassword()
    {
        string userName = testUserName;
        string password = testPassword;
        string expected = testPasswordEncoded;

        var encoded = TrackImportService.EncodePassword(userName, password);

        encoded.Should().Be(expected);
    }

    [Fact(Skip = Skip)]
    public async Task Authenticate_ShouldReturnTrue_WhenUsingValidCredentials()
    {
        string username = configuration["Iracing:UserName"];
        string password = configuration["Iracing:Password"];
        var cookieContainer = new CookieContainer();
        var httpClientHander = new HttpClientHandler()
        {
            UseCookies = true,
            CookieContainer = cookieContainer
        };

        var sut = new TrackImportService(new(httpClientHander));
        var result = await sut.Authenticate(username, password);

        result.Should().BeTrue();
        cookieContainer.GetAllCookies().Should().NotBeEmpty();
    }

    [Fact(Skip = Skip)]
    public async Task Authenticate_ShouldReturnFalse_WhenUsingInvalidCredentials()
    {
        var username = testUserName;
        var password = testPassword;
        var cookieContainer = new CookieContainer();
        var httpClientHander = new HttpClientHandler()
        {
            UseCookies = true,
            CookieContainer = cookieContainer
        };

        var sut = new TrackImportService(new(httpClientHander));
        var result = await sut.Authenticate(username, password);

        result.Should().BeFalse();
    }

    [Fact(Skip = Skip)]
    public async Task GetTracksData_ShouldReturnData_WhenUsingValidCredentials()
    {
        string username = configuration["Iracing:UserName"];
        string password = configuration["Iracing:Password"];
        var cookieContainer = new CookieContainer();
        var httpClientHander = new HttpClientHandler()
        {
            UseCookies = true,
            CookieContainer = cookieContainer
        };

        var sut = new TrackImportService(new(httpClientHander));
        await sut.Authenticate(username, password);
        cookieContainer.GetCookies(new("https://members-ng.iracing.com")).Should().NotBeEmpty();
        var result = await sut.GetTracksData();

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
}
