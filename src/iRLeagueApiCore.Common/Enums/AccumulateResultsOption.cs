namespace iRLeagueApiCore.Common.Enums;

/// <summary>
/// Select the method to accumulate scorings
/// </summary>
public enum AccumulateResultsOption
{
    None = 0,
    Sum,
    Best,
    Worst,
    Average,
    WeightedAverage
}
