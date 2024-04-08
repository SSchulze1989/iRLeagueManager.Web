using System.Net;

namespace iRLeagueApiCore.Client.Results;

public struct ClientActionResult<T>
{
    public ClientActionResult(T? content, HttpStatusCode httpStatusCode, string requestUrl = "") : this(true, "Success", "", content, httpStatusCode, requestUrl)
    { }

    public ClientActionResult(bool success, string status, string message, T? content, HttpStatusCode httpStatusCode, string requestUrl = "", IEnumerable<object>? errors = null)
    {
        Success = success;
        Status = status;
        Message = message;
        Content = content;
        HttpStatusCode = httpStatusCode;
        RequestUrl = requestUrl;
        Errors = errors ?? Array.Empty<object>();
    }

    public bool Success { get; }
    public string Status { get; }
    public string Message { get; }
    public T? Content { get; }
    public HttpStatusCode HttpStatusCode { get; }
    public string RequestUrl { get; }
    public IEnumerable<object> Errors { get; }
}

public struct NoContent
{
    public static NoContent Value => new();
}
