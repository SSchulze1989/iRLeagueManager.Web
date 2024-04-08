using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class EventCalculationServiceProvider :
    ICalculationServiceProvider<EventCalculationConfiguration, EventCalculationData, EventCalculationResult>
{
    private readonly ICalculationServiceProvider<SessionCalculationConfiguration, SessionCalculationData, SessionCalculationResult>
        sessionCalculationServiceProvider;

    public EventCalculationServiceProvider(
        ICalculationServiceProvider<SessionCalculationConfiguration, SessionCalculationData, SessionCalculationResult> sessionCalculationServiceProvider)
    {
        this.sessionCalculationServiceProvider = sessionCalculationServiceProvider;
    }

    public ICalculationService<EventCalculationData, EventCalculationResult> GetCalculationService(EventCalculationConfiguration config)
    {
        return new EventCalculationService(config, sessionCalculationServiceProvider);
    }
}
