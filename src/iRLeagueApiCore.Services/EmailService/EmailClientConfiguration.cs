using System.Net;

namespace iRLeagueApiCore.Services.EmailService;

public sealed class EmailClientConfiguration
{
    public EmailClientConfiguration(string host, int port, NetworkCredential credentials, string sender, bool enableSsl = true)
    {
        Host = host;
        Port = port;
        Credentials = credentials;
        Sender = sender;
        EnableSsl = enableSsl;
    }

    public string Host { get; }
    public int Port { get; }
    public NetworkCredential Credentials { get; }
    public string Sender { get; }
    public bool EnableSsl { get; }

    public static EmailClientConfiguration Default { get; } = new(string.Empty, 0, new(), string.Empty, false);
}
