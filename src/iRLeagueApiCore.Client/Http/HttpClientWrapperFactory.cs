using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace iRLeagueApiCore.Client.Http;
public sealed class HttpClientWrapperFactory
{
    private readonly ILoggerFactory loggerFactory;
    private readonly JsonSerializerOptions? jsonOptions;

    public HttpClientWrapperFactory(ILoggerFactory loggerFactory, JsonSerializerOptions? jsonOptions = null)
    {
        this.loggerFactory = loggerFactory;
        this.jsonOptions = jsonOptions;
    }

    public HttpClientWrapper Create(HttpClient httpClient, IAsyncTokenProvider tokenProvider, LeagueApiClient? apiClient = null)
    {
        return new(httpClient, loggerFactory.CreateLogger<HttpClientWrapper>(), tokenProvider, apiClient, jsonOptions);
    }
}
