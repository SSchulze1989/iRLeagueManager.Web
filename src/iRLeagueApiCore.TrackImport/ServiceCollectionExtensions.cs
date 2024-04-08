using iRLeagueApiCore.TrackImport.Service;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTrackImporter(this IServiceCollection services)
    {
        services.TryAddScoped<HttpClient>();
        services.TryAddScoped<TrackImportService>();
        return services;
    }
}
