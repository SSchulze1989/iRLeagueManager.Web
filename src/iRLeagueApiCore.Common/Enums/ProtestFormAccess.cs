namespace iRLeagueApiCore.Common.Enums;

[Flags]
public enum ProtestFormAccess
{
    Public = 1,
    Participants = 2,
    LeagueMembers = 4,
    Password = 8,
}
