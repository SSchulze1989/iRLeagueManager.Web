namespace iRLeagueManager.Web.Data;

public class BonusPointOption
{
    public BonusPointType Type { get; set; }
    public string Description { get; set; }
    public bool HasPosition { get; set; }

    public BonusPointOption(BonusPointType type, string description, bool hasPosition)
    {
        Type = type;
        Description = description;
        HasPosition = hasPosition;
    }

    public static BonusPointOption[] Available = new BonusPointOption[]
    {
        new(BonusPointType.Position, "Position", true),
        new(BonusPointType.QualyPosition, "Qualy Position", true),
        new(BonusPointType.FastestLap, "Fastest Lap", false),
        new(BonusPointType.FastestAverageLap, "Fastest average lap", false),
        new(BonusPointType.CleanestDriver, "Cleanest Driver", false),
        new(BonusPointType.NoIncidents, "No incidents", false),
        new(BonusPointType.MostPositionsGained, "Gained most Pos.", false),
        new(BonusPointType.MostPositionsLost, "Lost most Pos.", false),
        new(BonusPointType.LeadOneLap, "Lead one lap", false),
        new(BonusPointType.LeadMostLaps, "Lead most laps", false),
        new(BonusPointType.Custom, "Custom", false),
    };
}
