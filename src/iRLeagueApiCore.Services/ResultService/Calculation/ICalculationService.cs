namespace iRLeagueApiCore.Services.ResultService.Calculation;

public interface ICalculationService<TIn, TOut>
{
    public Task<TOut> Calculate(TIn data);
}
