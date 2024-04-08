using iRLeagueApiCore.Client;
using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.Service;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;
public static class IRLeagueApiClientServiceExtensions
{
    public static IServiceCollection AddLeagueApiClient(this IServiceCollection services, Action<LeagueApiClientConfiguration>? configure = null)
    {
        services.AddHttpClient();
        services.TryAddScoped<HttpClientWrapperFactory>();
        services.TryAddScoped<ITokenStore, DefaultTokenStore>();
        services.TryAddScoped<IAsyncTokenProvider>(sp => sp.GetRequiredService<ITokenStore>());

        // run configuration
        var configuration = new LeagueApiClientConfiguration(services);
        configure?.Invoke(configuration);

        services.TryAddScoped<LeagueApiClientFactory>(sp => new(
                configuration.BaseAddress,
                sp.GetRequiredService<ILoggerFactory>(),
                sp.GetRequiredService<IHttpClientFactory>(),
                sp.GetRequiredService<ITokenStore>(),
                sp.GetRequiredService<HttpClientWrapperFactory>()
            ));
        services.TryAddScoped(sp => sp.GetRequiredService<LeagueApiClientFactory>().CreateClient());

        return services;
    }
}
