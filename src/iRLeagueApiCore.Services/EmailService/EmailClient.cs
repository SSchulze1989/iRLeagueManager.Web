using System.Net.Mail;

namespace iRLeagueApiCore.Services.EmailService;

public sealed class EmailClient : IEmailClient
{
    private readonly ILogger<EmailClient> logger;
    private readonly EmailClientConfiguration clientConfiguration;

    public EmailClient(EmailClientConfiguration clientConfiguration, ILogger<EmailClient> logger)
    {
        this.clientConfiguration = clientConfiguration;
        this.logger = logger;
    }

    public async Task SendNoReplyMailAsync(string email, string subject, string body)
    {
        using var smtpClient = CreateSmtpClient(clientConfiguration);
        if (smtpClient == null)
        {
            logger.LogInformation("No Email SMTP host configured - Emails will not be send. To configure a client specify \"Host\", \"Port\" and \"Sender\" in appsettings \"Mail\" section");
            return;
        }

        using var message = new MailMessage(new MailAddress(clientConfiguration.Sender), new MailAddress(email))
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        logger.LogInformation("Sending Email To:\"{Recipient}\" through Host: \"{Host}\"", email, smtpClient.Host);
        await smtpClient.SendMailAsync(message);
    }

    private static SmtpClient? CreateSmtpClient(EmailClientConfiguration configuration)
    {
        if (configuration.Equals(EmailClientConfiguration.Default))
        {
            return null;
        }
        return new()
        {
            Host = configuration.Host,
            Port = configuration.Port,
            Credentials = configuration.Credentials,
            EnableSsl = configuration.EnableSsl,
        };
    }
}
