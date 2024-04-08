using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueApiCore.Client.Http;
public sealed class ApiServiceUnavailableException : Exception
{
    public ApiServiceUnavailableException() : this(null)
    {
    }

    public ApiServiceUnavailableException(Exception? innerException) : this("iRLeagueApi service is not available or did not respond to the request", innerException)
    {
    }

    public ApiServiceUnavailableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
