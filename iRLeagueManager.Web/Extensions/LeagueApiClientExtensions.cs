using iRLeagueApiCore.Client.Results;
using System;
using System.Runtime.Serialization;

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
            throw new ActionResultException<T>(clientActionResult, "Action result did not indicate success");
        }
    }

    public class ActionResultException<T> : InvalidOperationException
    {
        public ClientActionResult<T> ActionResult;

        public ActionResultException(ClientActionResult<T> actionResult) : this(actionResult, default)
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
