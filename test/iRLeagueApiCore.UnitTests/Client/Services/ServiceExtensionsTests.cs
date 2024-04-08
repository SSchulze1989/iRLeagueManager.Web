using iRLeagueApiCore.Client;
using iRLeagueApiCore.Client.Http;
using Microsoft.Extensions.DependencyInjection;

namespace iRLeagueApiCore.UnitTests.Client.Services;
public sealed class ServiceExtensionsTests
{
    private const string defaultBaseAddress = "https://irleaguemanager.net/api/";
    private readonly IServiceCollection services = new ServiceCollection();
    private readonly Fixture fixture = new();

    [Fact]
    public void ShouldAddServices_WithDefaultConfiguration()
    {
        services.AddLeagueApiClient();

        var provider = services.BuildServiceProvider();
        var tokenStore = provider.GetRequiredService<ITokenStore>();
        var apiClient = provider.GetRequiredService<ILeagueApiClient>();

        tokenStore.GetType().Name.Should().Be("DefaultTokenStore");
        apiClient.BaseAddress.Should().Be(defaultBaseAddress);
    }

    [Fact]
    public void ShouldAddServices_WithDifferentBaseAddress()
    {
        var baseUri = fixture.Create<Uri>();

        services.AddLeagueApiClient(config => config.BaseAddress = baseUri.ToString());
        var apiClient = services.BuildServiceProvider().GetRequiredService<ILeagueApiClient>();

        apiClient.BaseAddress.Should().Be(baseUri);
    }

    [Fact]
    public void ShouldAddServices_WithDifferentTokenStore1()
    {
        services.AddLeagueApiClient(config => config.UseTokenStore<TestTokenStore>());

        var tokenStore = services.BuildServiceProvider().GetRequiredService<ITokenStore>();

        tokenStore.Should().BeOfType<TestTokenStore>();
    }

    [Fact]
    public void ShouldAddServices_WithDifferentTokenStore2()
    {
        services.AddLeagueApiClient(config => config.UseTokenStore(sp => new TestTokenStore()));

        var tokenStore = services.BuildServiceProvider().GetRequiredService<ITokenStore>();

        tokenStore.Should().BeOfType<TestTokenStore>();
    }
}
