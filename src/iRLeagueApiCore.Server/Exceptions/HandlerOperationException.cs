using System.Runtime.Serialization;

namespace iRLeagueApiCore.Server.Exceptions;

public sealed class HandlerOperationException : InvalidOperationException
{
    public HandlerOperationException()
    {
    }

    public HandlerOperationException(string message) : base(message)
    {
    }

    public HandlerOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    private HandlerOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
