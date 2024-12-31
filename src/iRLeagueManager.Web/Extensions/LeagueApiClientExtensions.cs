using iRLeagueApiCore.Client.Results;
using iRLeagueManager.Web.Data;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web;

namespace iRLeagueManager.Web.Extensions;

internal static class LeagueApiClientExtensions
{
    public static T? EnsureSuccess<T>(this ClientActionResult<T> clientActionResult)
    {
        if (clientActionResult.Success == true)
        {
            return clientActionResult.Content;
        }
        if (clientActionResult.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default(T);
        }
        throw new ActionResultException<T>(clientActionResult);
    }

    public static StatusResult ToStatusResult<T>(this ClientActionResult<T> clientActionResult)
    {
        if (clientActionResult.Success)
        {
            return StatusResult.SuccessResult(clientActionResult.Message);
        }
        return StatusResult.FailedResult(clientActionResult.Status, clientActionResult.Message, clientActionResult.Errors);
    }

    public static StatusResult<T> ToContentStatusResult<T>(this ClientActionResult<T> clientActionResult)
    {
        if (clientActionResult.Success)
        {
            return StatusResult<T>.SuccessResult(clientActionResult.Content, clientActionResult.Message);
        }
        return StatusResult<T>.FailedResult(clientActionResult.Status, clientActionResult.Content, clientActionResult.Message, clientActionResult.Errors);
    }

    public static StatusResult<T> ToContentStatusResult<T>(this ClientActionResult<T> clientActionResult, T content)
    {
        if (clientActionResult.Success)
        {
            return StatusResult<T>.SuccessResult(clientActionResult.Content, clientActionResult.Message);
        }
        return StatusResult<T>.FailedResult(clientActionResult.Status, content, clientActionResult.Message, clientActionResult.Errors);
    }
}

public static class ExtensionMethods
{
    public static NameValueCollection QueryString(this NavigationManager navigationManager)
    {
        return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
    }

    public static string QueryString(this NavigationManager navigationManager, string key)
    {
        return navigationManager.QueryString()[key] ?? string.Empty;
    }

    public static T? QueryParameter<T>(this NavigationManager navigationManager, string key)
    {
        var paramString = navigationManager.QueryString()[key];
        if (string.IsNullOrEmpty(paramString))
        {
            return default(T);
        }
        return (T)Convert.ChangeType(paramString, typeof(T));
    }
}

public sealed class ActionResultException<T> : InvalidOperationException
{
    public ClientActionResult<T> ActionResult;

    public ActionResultException(ClientActionResult<T> actionResult) : this(actionResult, $"Action result did not indicate success: {actionResult.Status} -> {actionResult.Message} -- from Request: {actionResult.RequestUrl}")
    {
    }

    public ActionResultException(ClientActionResult<T> actionResult, string message) : this(actionResult, message, default)
    {
    }

    public ActionResultException(ClientActionResult<T> actionResult, string message, Exception? innerException) : base(message, innerException)
    {
        ActionResult = actionResult;
    }
}
