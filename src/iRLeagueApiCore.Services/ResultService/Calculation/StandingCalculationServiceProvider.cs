using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class StandingCalculationServiceProvider :
    ICalculationServiceProvider<StandingCalculationConfiguration, StandingCalculationData, StandingCalculationResult>
{
    public ICalculationService<StandingCalculationData, StandingCalculationResult> GetCalculationService(StandingCalculationConfiguration config)
    {
        switch (config.ResultKind)
        {
            case Common.Enums.ResultKind.Member:
                return new MemberStandingCalculationService(config);
            case Common.Enums.ResultKind.Team:
                return new TeamStandingCalculationService(config);
        }
        return new MemberStandingCalculationService(config);
    }
}
