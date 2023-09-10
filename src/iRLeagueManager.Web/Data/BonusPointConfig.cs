namespace iRLeagueManager.Web.Data;

public class BonusPointConfig
{
    public BonusPointOption Option { get; set; }
    public BonusPointType OptionId
    {
        get => Option.Type;
        set => Option = BonusPointOption.Available.FirstOrDefault(x => x.Type == value) ?? BonusPointOption.Available.First();
    }
    public int Position { get; set; }
    public int Points { get; set; }

    public BonusPointConfig()
    {
        Option = BonusPointOption.Available.First();
    }

    public BonusPointConfig(BonusPointOption option, int position, int points)
    {
        Option = option;
        Position = position;
        Points = points;
    }

    public BonusPointConfig(BonusPointType type, int position, int points)
    {
        var bonusOption = BonusPointOption.Available.FirstOrDefault(x => x.Type == type)
            ?? throw new ArgumentException(nameof(type));
        Option = bonusOption;
        Position = position;
        Points = points;
    }

    public override string ToString()
    {
        return $"{Option.Description} {(Position != 0 ? Position : "")} => {Points}";
    }
}
