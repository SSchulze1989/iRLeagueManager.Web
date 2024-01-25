﻿using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class PointRuleViewModel : LeagueViewModelBase<PointRuleViewModel, PointRuleModel>
{
    public PointRuleViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    {
    }

    public PointRuleViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, PointRuleModel model) :
        base(loggerFactory, apiService, model)
    {
    }

    public long LeagueId => model.LeagueId;
    public long PointRuleId => model.PointRuleId;

    private PointRuleType ruleType;
    public PointRuleType RuleType
    {
        get => ruleType;
        set
        {
            if (Set(ref ruleType, value))
            {
                RuleTypeChanged(value);
            }
        }
    }

    public int MaxPoints { get => model.MaxPoints; set => SetP(model.MaxPoints, value => model.MaxPoints = value, value); }
    public int PointDropOff { get => model.PointDropOff; set => SetP(model.PointDropOff, value => model.PointDropOff = value, value); }
    public IList<int> PointsPerPlace { get => model.PointsPerPlace; set => SetP(model.PointsPerPlace, value => model.PointsPerPlace = value, value); }
    public ICollection<BonusPointModel> BonusPoints
    {
        get => model.BonusPoints;
        set
        {
            if (SetP(model.BonusPoints, value => model.BonusPoints = value, value))
            {
                OnPropertyChanged(nameof(BonusPointConfigs));
            }
        }
    }
    public ICollection<SortOptions> PointsSortOptions { get => model.PointsSortOptions; set => SetP(model.PointsSortOptions, value => model.PointsSortOptions = value, value); }
    public ICollection<SortOptions> FinalSortOptions { get => model.FinalSortOptions; set => SetP(model.FinalSortOptions, value => model.FinalSortOptions = value, value); }
    public IEnumerable<BonusPointConfig> BonusPointConfigs
    {
        get => BonusPoints.Select(x => new BonusPointConfig(x.Type, (int)x.Value, (int)x.Points, x.Conditions));
        set => BonusPoints = value.Select(x => new BonusPointModel() { Type = x.OptionId, Value = x.Position, Points = x.Points, Conditions = x.Conditions }).ToList();
    }

    public ICollection<AutoPenaltyConfiguration> AutoPenalties { get => model.AutoPenalties; set => SetP(model.AutoPenalties, value => model.AutoPenalties = value, value); }

    public enum PointRuleType
    {
        MaxPoints = 0,
        PointList = 1,
        Formula = 2,
    }

    protected override void SetModel(PointRuleModel model)
    {
        base.SetModel(model);
        RuleType = InferRuleType(model);
    }

    private void RuleTypeChanged(PointRuleType ruleType)
    {
        switch (ruleType)
        {
            case PointRuleType.MaxPoints:
                break;
            default:
                break;
        }
    }

    private static PointRuleType InferRuleType(PointRuleModel model)
    {
        if (model.MaxPoints != 0)
        {
            return PointRuleType.MaxPoints;
        }
        return PointRuleType.PointList;
    }
}
