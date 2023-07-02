namespace iRLeagueManager.Web.Data;

public class BonusPointOption
{
    public char Id { get; set; }
    public string Description { get; set; }
    public bool HasPosition { get; set; }

    public BonusPointOption(char id, string description, bool hasPosition)
    {
        Id = id;
        Description = description;
        HasPosition = hasPosition;
    }

    public static BonusPointOption[] Available = new BonusPointOption[]
    {
        new('p', "Position", true),
        new('q', "Qualy Position", true),
        new('f', "Fastest Lap", false),
        new('a', "Fastest average lap", false),
        new('c', "Cleanest Driver", false),
        new('n', "No incidents", false),
        new('g', "Gained most Pos.", false),
        new('d', "Lost most Pos.", false),
        new('l', "Lead one lap", false),
        new('m', "Lead most laps", false),
    };
}
