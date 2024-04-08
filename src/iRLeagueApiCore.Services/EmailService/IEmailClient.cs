namespace iRLeagueApiCore.Services.EmailService;

public interface IEmailClient
{
    public Task SendNoReplyMailAsync(string email, string subject, string body);
}
