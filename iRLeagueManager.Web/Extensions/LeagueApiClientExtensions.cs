using iRLeagueApiCore.Client.Results;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web;

namespace iRleagueManager.Web.Extensions
{
    public static class LeagueApiClientExtensions
    {
        public static T EnsureSuccess<T>(this ClientActionResult<T> clientActionResult)
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
    }

    public static class ExtensionMethods
    {
        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        public static string QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }
    }

    public class ActionResultException<T> : InvalidOperationException
    {
        public ClientActionResult<T> ActionResult;

        public ActionResultException(ClientActionResult<T> actionResult) : this(actionResult, $"Action result did not indicate success: {actionResult.Status} -> {actionResult.Message} -- from Request: {actionResult.RequestUrl}")
        {
        }

        public ActionResultException(ClientActionResult<T> actionResult, string message) : this(actionResult, message, default)
        {
        }

        public ActionResultException(ClientActionResult<T> actionResult, string message, Exception innerException) : base(message, innerException)
        {
            ActionResult = actionResult;
        }

        protected ActionResultException(ClientActionResult<T> actionResult, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ActionResult = actionResult;
        }
    }
}
