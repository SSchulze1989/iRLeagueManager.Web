using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.Service;
using Microsoft.Extensions.DependencyInjection;

namespace iRLeagueApiCore.UnitTests.Client.Services;
public sealed class LeagueApiClientConfigurationTests
{
    private readonly Fixture fixture = new();
    private readonly IServiceCollection services = new ServiceCollection();

    [Fact]
    public void ShouldRegisterNewTokenStore()
    {
        var testTokenStore1 = Mock.Of<ITokenStore>();
        services.AddScoped(sp => testTokenStore1);
        var sut = CreateSut();

        sut.UseTokenStore<TestTokenStore>();

        var tokenStore = services.BuildServiceProvider().GetRequiredService<ITokenStore>();
        tokenStore.Should().NotBeSameAs(testTokenStore1);
        tokenStore.Should().BeOfType<TestTokenStore>();
    }

    [Fact]
    public void ShouldRegisterNewTokenStoreInstance()
    {
        var testTokenStore1 = Mock.Of<ITokenStore>();
        var testTokenStore2 = new TestTokenStore();
        services.AddScoped(sp => testTokenStore1);
        var sut = CreateSut();

        sut.UseTokenStore(sp => testTokenStore2);

        var tokenStore = services.BuildServiceProvider().GetRequiredService<ITokenStore>();
        tokenStore.Should().NotBeSameAs(testTokenStore1);
        tokenStore.Should().BeSameAs(testTokenStore2);
    }

    [Fact]
    public void ShouldSetBaseAddress()
    {
        var testAddress = fixture.Create<Uri>();
        var sut = CreateSut();

        sut.UseBaseAddress(testAddress.ToString());

        sut.BaseAddress.Should().Be(testAddress.ToString());
    }

    [Fact]
    public void ShouldRegisterDefaultTokenStore()
    {
        services.AddScoped<TestTokenStore>();
        var sut = CreateSut();

        sut.UseDefaultTokenStore();

        var tokenProvider = services.BuildServiceProvider().GetRequiredService<ITokenStore>();
        tokenProvider.GetType().Name.Should().Be("DefaultTokenStore");
    }

    private LeagueApiClientConfiguration CreateSut()
    {
        return new(services);
    }
}
