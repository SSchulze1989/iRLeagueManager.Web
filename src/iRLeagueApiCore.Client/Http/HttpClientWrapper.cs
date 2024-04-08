using iRLeagueApiCore.Client.Results;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace iRLeagueApiCore.Client.Http;

public sealed class HttpClientWrapper
{
    private readonly HttpClient httpClient;
    private readonly ILogger<HttpClientWrapper> logger;
    private readonly IAsyncTokenProvider tokenProvider;
    private readonly ILeagueApiClient? apiClient;
    private readonly JsonSerializerOptions? jsonOptions;

    public Uri? BaseAddress => httpClient.BaseAddress;

    public HttpClientWrapper(HttpClient httpClient, ILogger<HttpClientWrapper> logger, IAsyncTokenProvider tokenProvider, ILeagueApiClient? apiClient = default, JsonSerializerOptions? jsonOptions = default)
    {
        this.httpClient = httpClient;
        this.tokenProvider = tokenProvider;
        this.apiClient = apiClient;
        this.jsonOptions = jsonOptions;
        this.logger = logger;
    }

    public async Task<HttpResponseMessage> Get(string uri, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri(uri, UriKind.RelativeOrAbsolute));
        return await SendRequest(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> Post(string uri, object? data, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, new Uri(uri, UriKind.RelativeOrAbsolute));
        request.Content = new StringContent(JsonSerializer.Serialize(data, jsonOptions), Encoding.UTF8, "application/json");
        return await SendRequest(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> Put(string uri, object? data, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, new Uri(uri, UriKind.RelativeOrAbsolute));
        request.Content = new StringContent(JsonSerializer.Serialize(data, jsonOptions), Encoding.UTF8, "application/json");
        return await SendRequest(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> Delete(string uri, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(uri, UriKind.RelativeOrAbsolute));
        return await SendRequest(request, cancellationToken);
    }

    public async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Options.TryGetValue(new HttpRequestOptionsKey<bool>("SkipAuth"), out bool skipAuth);
        if (skipAuth == false)
        {
            if (apiClient is not null)
            {
                await apiClient.CheckLogin(cancellationToken);
            }
            await AddJWTTokenAsync(request);
        }

        try
        {
#if DEBUG
            logger.LogDebug("Send request: [{Method}] {RequestUrl}", request.Method, request.RequestUri);
#endif
            var result = await httpClient.SendAsync(request, cancellationToken);
#if DEBUG
        logger.LogDebug("Returned: [StatusCode {StatusCode}]", result.StatusCode);
#endif

            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized && apiClient is not null)
            {
                logger.LogDebug("Logout on unauthorized response");
                // logout on unauthorized answer -> means something is wrong with our tokens
                await apiClient.LogOut();
            }
            return result;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiServiceUnavailableException(ex);
        }
    }

    public HttpRequestMessage CreateRequest(HttpMethod method, string uri, object? content, HttpRequestOptions? options = default)
    {
        var request = new HttpRequestMessage(method, new Uri(uri, UriKind.RelativeOrAbsolute));
        if (options is not null)
        {
            foreach(var option in options)
            {
                request.Options.TryAdd(option.Key, option.Value);
            }
        }
        if (content is not null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(content, jsonOptions), Encoding.UTF8, "application/json");
        }
        return request;
    }

    public async Task<ClientActionResult<T>> ConvertToClientActionResult<T>(HttpResponseMessage message, CancellationToken cancellationToken)
    {
        return await message.ToClientActionResultAsync<T>(jsonOptions, cancellationToken);
    }

    private async Task AddJWTTokenAsync(HttpRequestMessage request)
    {
        var token = await tokenProvider.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(token) == false)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
        }
    }

    public async Task<ClientActionResult<T>> GetAsClientActionResult<T>(string query, CancellationToken cancellationToken = default)
    {
        return await Get(query, cancellationToken).AsClientActionResultAsync<T>(jsonOptions, cancellationToken);
    }

    public async Task<ClientActionResult<T>> PostAsClientActionResult<T>(string query, object? body, CancellationToken cancellationToken = default)
    {
        return await Post(query, body, cancellationToken).AsClientActionResultAsync<T>(jsonOptions, cancellationToken);
    }

    public async Task<ClientActionResult<T>> PutAsClientActionResult<T>(string query, object? body, CancellationToken cancellationToken = default)
    {
        return await Put(query, body, cancellationToken).AsClientActionResultAsync<T>(jsonOptions, cancellationToken);
    }

    public async Task<ClientActionResult<NoContent>> DeleteAsClientActionResult(string query, CancellationToken cancellationToken = default)
    {
        return await Delete(query, cancellationToken).AsClientActionResultAsync<NoContent>(jsonOptions, cancellationToken);
    }
}
