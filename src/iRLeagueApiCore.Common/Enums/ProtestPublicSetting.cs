namespace iRLeagueApiCore.Common.Enums;

public enum ProtestPublicSetting
{
    /// <summary>
    /// Hide protests so that the can only be seen by stewards
    /// </summary>
    Hidden = 0,
    /// <summary>
    /// Show protests in public but hide the name of the protester
    /// </summary>
    WithoutProtester = 1,
    /// <summary>
    /// Show both protest and name of protester in public
    /// </summary>
    WithProtester = 2,
}
