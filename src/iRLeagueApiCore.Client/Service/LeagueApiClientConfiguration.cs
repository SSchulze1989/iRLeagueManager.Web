using iRLeagueApiCore.Client.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace iRLeagueApiCore.Client.Service;
public sealed class LeagueApiClientConfiguration
{
    const string defaultBaseAddress = "https://irleaguemanager.net/api/";
    private readonly IServiceCollection services;

    public string BaseAddress { get; set; } = defaultBaseAddress;

    public LeagueApiClientConfiguration(IServiceCollection services)
    {
        this.services = services;
    }

    public LeagueApiClientConfiguration UseBaseAddress(string baseAddress)
    {
        BaseAddress = baseAddress;
        return this;
    }

    public LeagueApiClientConfiguration UseDefaultTokenStore()
    {
        services.Replace(ServiceDescriptor.Scoped<ITokenStore, DefaultTokenStore>());
        return this;
    }

    public LeagueApiClientConfiguration UseTokenStore<T>() where T : class, ITokenStore
    {
        services.Replace(ServiceDescriptor.Scoped<ITokenStore,  T>());
        return this;
    }

    public LeagueApiClientConfiguration UseTokenStore<T>(Func<IServiceProvider, T> factory) where T : class, ITokenStore
    {
        services.Replace(ServiceDescriptor.Scoped<ITokenStore>(factory));
        return this;
    }
}
