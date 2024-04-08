using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class SessionCalculationServiceProvider :
    ICalculationServiceProvider<SessionCalculationConfiguration, SessionCalculationData, SessionCalculationResult>
{
    public ICalculationService<SessionCalculationData, SessionCalculationResult> GetCalculationService(SessionCalculationConfiguration config)
    {
        return config.ResultKind switch
        {
            ResultKind.Member => new MemberSessionCalculationService(config),
            ResultKind.Team => new TeamSessionCalculationService(config),
            _ => throw new InvalidOperationException($"Unknown Scoring Kind: {config.ResultKind}"),
        };
    }
}
