namespace iRLeagueApiCore.Common.Enums;

public enum ComparatorType
{
    IsSmaller,
    IsSmallerOrEqual,
    IsEqual,
    IsBiggerOrEqual,
    IsBigger,
    NotEqual,
    InList,
    ForEach,    // special comparator that multiplies the configured bonus/penalty for each multiple of the provided value
}
