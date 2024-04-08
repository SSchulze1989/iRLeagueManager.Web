using iRLeagueApiCore.Common.Responses;
using System.Net;
#if NETCOREAPP
using System.Net.Http.Json;
using System.Text.Json;
#endif

namespace iRLeagueApiCore.Client.Results;

public static class HttpExtensions
{
    public static async Task<ClientActionResult<T>> ToClientActionResultAsync<T>(this HttpResponseMessage httpResponse, JsonSerializerOptions? jsonOptions, CancellationToken cancellationToken = default)
    {
        string requestUrl = httpResponse.RequestMessage?.RequestUri?.AbsoluteUri ?? string.Empty;
        try
        {
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = httpResponse.StatusCode != HttpStatusCode.NoContent ?
                    await httpResponse.Content.ReadFromJsonAsync<T>(options: jsonOptions, cancellationToken: cancellationToken) : default;
                return new ClientActionResult<T>(content, httpResponse.StatusCode, requestUrl);
            }

            string status = "";
            string message = "";
            IEnumerable<object> errors;
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    {
                        var response = await httpResponse.Content.ReadFromJsonAsync<BadRequestResponse>(options: jsonOptions, cancellationToken: cancellationToken);
                        status = response.Status;
                        errors = response.Errors.Cast<object>();
                        break;
                    }
                case HttpStatusCode.Unauthorized:
                    {
                        var response = await httpResponse.Content.ReadFromJsonAsync<UnauthorizedResponse>(options: jsonOptions, cancellationToken: cancellationToken);
                        status = "Unauthorized";
                        message = response.Status ?? string.Empty;
                        errors = response.Errors ?? Array.Empty<object>();
                        break;
                    }
                case HttpStatusCode.Forbidden:
                    {
                        status = "Forbidden";
                        errors = Array.Empty<object>();
                        break;
                    }
                case HttpStatusCode.NotFound:
                    {
                        status = "Not Found";
                        errors = Array.Empty<object>();
                        break;
                    }
                case HttpStatusCode.MethodNotAllowed:
                    {
                        status = "Method Not Allowed";
                        message = $"Method {httpResponse.RequestMessage?.Method.Method} not allowed on {httpResponse.RequestMessage?.RequestUri}";
                        errors = Array.Empty<object>();
                        break;
                    }
                case HttpStatusCode.InternalServerError:
                    {
                        var response = await httpResponse.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
                        status = "Internal server Error";
                        message = response;
                        errors = new object[] { "Internal server Error " };
                        break;
                    }
                default:
                    status = "Unknown Response";
                    message = "";
                    errors = Array.Empty<object>();
                    break;
            }
            return new ClientActionResult<T>(false, status, message, default, httpResponse.StatusCode, requestUrl, errors);
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is JsonException)
        {
            var errors = new object[] { ex };
            return new ClientActionResult<T>(false, "Error", ex.Message, default, 0, requestUrl, errors);
        }
    }

    public static async Task<ClientActionResult<T>> AsClientActionResultAsync<T>(this Task<HttpResponseMessage> request, JsonSerializerOptions? jsonOptions, CancellationToken cancellationToken = default)
    {
        string requestUrl = "";
        try
        {
            var result = await request;
            requestUrl = result.RequestMessage?.RequestUri?.AbsoluteUri ?? string.Empty;
            return await result.ToClientActionResultAsync<T>(jsonOptions, cancellationToken);
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is HttpRequestException)
        {
            var errors = new object[] { ex };
            return new ClientActionResult<T>(false, "Error", "Exception: " + ex, default, 0, requestUrl, errors);
        }
    }

    public static async Task<ClientActionResult<T>> GetAsClientActionResult<T>(this HttpClient httpClient, string query, JsonSerializerOptions jsonOptions, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetAsync(query, cancellationToken).AsClientActionResultAsync<T>(jsonOptions, cancellationToken);
    }

    public static async Task<ClientActionResult<TResponse>> PostAsClientActionResult<TResponse, TPost>(this HttpClient httpClient, string query, TPost body, JsonSerializerOptions jsonOptions, CancellationToken cancellationToken = default)
    {
        return await httpClient.PostAsJsonAsync(query, body, cancellationToken).AsClientActionResultAsync<TResponse>(jsonOptions, cancellationToken);
    }

    public static async Task<ClientActionResult<TResponse>> PutAsClientActionResult<TResponse, TPut>(this HttpClient httpClient, string query, TPut body, JsonSerializerOptions jsonOptions, CancellationToken cancellationToken = default)
    {
        return await httpClient.PutAsJsonAsync(query, body, cancellationToken).AsClientActionResultAsync<TResponse>(jsonOptions, cancellationToken);
    }

    public static async Task<ClientActionResult<NoContent>> DeleteAsClientActionResult(this HttpClient httpClient, string query, JsonSerializerOptions jsonOptions, CancellationToken cancellationToken = default)
    {
        return await httpClient.DeleteAsync(query, cancellationToken).AsClientActionResultAsync<NoContent>(jsonOptions, cancellationToken);
    }
}
