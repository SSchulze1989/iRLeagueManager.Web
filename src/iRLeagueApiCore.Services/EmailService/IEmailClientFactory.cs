namespace iRLeagueApiCore.Services.EmailService;

public interface IEmailClientFactory
{
    public IEmailClient CreateEmailClient();
}
