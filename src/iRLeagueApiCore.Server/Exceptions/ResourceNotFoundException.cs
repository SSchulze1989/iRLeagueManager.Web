using System.Runtime.Serialization;

namespace iRLeagueApiCore.Server.Exceptions;

public sealed class ResourceNotFoundException : Exception
{
    public string ResourceName { get; } = string.Empty;
    public ResourceNotFoundException() : this("Requested resource was not found")
    {
    }

    public ResourceNotFoundException(string message) : base(message)
    {
    }

    public ResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    private ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
