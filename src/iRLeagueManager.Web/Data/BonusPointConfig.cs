namespace iRLeagueManager.Web.Data;

public class BonusPointConfig
{
    public BonusPointOption Option { get; set; }
    public char OptionId
    {
        get => Option.Id;
        set => Option = BonusPointOption.Available.FirstOrDefault(x => x.Id == value) ?? BonusPointOption.Available.First();
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

    public BonusPointConfig(string bonusKey, int points)
    {
        if (string.IsNullOrEmpty(bonusKey))
        {
            throw new ArgumentException(nameof(bonusKey));
        }
        var bonusKeyId = bonusKey[0];
        int bonusKeyValue = 0;
        if (bonusKey.Length > 1 && int.TryParse(bonusKey[1..], out bonusKeyValue) == false)
        {
            throw new ArgumentException(nameof(bonusKey));
        }
        var bonusOption = BonusPointOption.Available.FirstOrDefault(x => x.Id == bonusKeyId)
            ?? throw new ArgumentException(nameof(bonusKey));
        Option = bonusOption;
        Position = bonusKeyValue;
        Points = points;
    }

    public override string ToString()
    {
        return $"{Option.Description} {(Position != 0 ? Position : "")} => {Points}";
    }
}
