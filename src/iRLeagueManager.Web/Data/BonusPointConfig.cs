using iRLeagueApiCore.Common.Models;

namespace iRLeagueManager.Web.Data;

public class BonusPointConfig
{
    public BonusPointOption Option { get; set; }
    public BonusPointType OptionId
    {
        get => Option.Type;
        set => Option = BonusPointOption.Available.FirstOrDefault(x => x.Type == value) ?? BonusPointOption.Available.First();
    }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public int Points { get; set; }
    public IList<FilterConditionModel> Conditions { get; set; } = new List<FilterConditionModel>();

    public BonusPointConfig()
    {
        Option = BonusPointOption.Available.First();
    }

    public BonusPointConfig(BonusPointType type, string name, int position, int points, IEnumerable<FilterConditionModel> conditions)
    {
        var bonusOption = BonusPointOption.Available.FirstOrDefault(x => x.Type == type)
            ?? throw new ArgumentException(null, nameof(type));
        Name = name;
        Option = bonusOption;
        Position = position;
        Points = points;
        Conditions = conditions.ToList();
    }

    public override string ToString()
    {
        return $"{Option.Description} {(Position != 0 ? Position : "")} => {Points}";
    }
}
