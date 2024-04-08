namespace iRLeagueApiCore.Services.ResultService.Calculation;

public interface ICalculationServiceProvider<TConfig, TIn, TOut>
{
    public ICalculationService<TIn, TOut> GetCalculationService(TConfig config);
}
