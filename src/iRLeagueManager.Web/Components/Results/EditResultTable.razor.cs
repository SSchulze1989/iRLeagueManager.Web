using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Components.Results;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Exceptions;
using iRLeagueManager.Web.Extensions;
using iRLeagueManager.Web.Shared;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Linq.Expressions;
using System.Reflection;

namespace iRLeagueManager.Web.Components;

public partial class EditResultTable : UtilityComponentBase
{
    public abstract class EditColumn<T> where T : class
    {
        public string Title { get; }
        public bool Editable { get; }
        public bool Hidden { get; set; }
        public bool Visible { get => !Hidden; set => Hidden = !value; }

        protected EditColumn(string title, bool editable = true)
        {
            Title = title;
            Editable = editable;
        }
    }

    public class EditColumn<T, TProperty> : EditColumn<T> where T : class
    {
        public Expression<Func<T, TProperty>> Property { get; }

        private readonly Func<T, TProperty> getFunc;
        private readonly Action<T, TProperty> setFunc;

        public EditColumn(string title, Expression<Func<T, TProperty>> property, bool editable = true) : base(title, editable)
        {
            Property = property;

            MemberExpression memberExpression = property.Body switch
            {
                MemberExpression member => member,
                UnaryExpression unary => unary.Operand is MemberExpression member ? member : throw new ArgumentException("Argument must be a member expression", nameof(property)),
                _ => throw new ArgumentException("Argument must be a member expression", nameof(property)),
            };

            if (memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("Member expression must target a property", nameof(property));
            }

            getFunc = row => (TProperty)propertyInfo.GetValue(row)!;
            setFunc = (row, value) => propertyInfo.SetValue(row, value);
        }

        public TProperty GetValue(T row)
        {
            return getFunc(row);
        }

        public void SetValue(T row, TProperty value)
        {
            setFunc(row, value);
        }
    }

    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    [Parameter, EditorRequired] public RawSessionResultViewModel SessionResult { get; set; } = default!;
    [Parameter, EditorRequired] public IEnumerable<MemberInfoModel> Members { get; set; } = default!;
    [Parameter, EditorRequired] public IEnumerable<TeamInfoModel> Teams { get; set; } = default!;

    private ObservableCollection<RawResultRowViewModel> ResultRows { get; set; } = default!;
    private RawResultRowViewModel? LastEditedItem { get; set; }
    private bool ShowAllColumns { get; set; } = true;

    private readonly List<EditColumn<RawResultRowViewModel>> columns = [
        CreateColumn("Member", x => x.MemberId),
        CreateColumn("iRacing Id", x => x.IRacingId, editable: false),
        CreateColumn("Team", x => x.TeamId),
        CreateColumn("Start Position", x => x.StartPosition),
        CreateColumn("Car Nr.", x => x.CarNumber),
        CreateColumn("Points", x => x.RacePoints),
        CreateColumn("Compl. Laps", x => x.CompletedLaps),
        CreateColumn("Lead Laps", x => x.LeadLaps),
        CreateColumn("Interval", x => x.Interval),
        CreateColumn("Status", x => x.Status),
        CreateColumn("Class Id", x => x.ClassId, hidden: true),
        CreateColumn("Car", x => x.Car, hidden: true),
        CreateColumn("Car Class", x => x.CarClass, hidden: true),
        CreateColumn("Completed Laps", x => x.CompletedLaps, hidden: true),
        CreateColumn("Incidents", x => x.Incidents, hidden: true),
        CreateColumn("Qualifying Time", x => x.QualifyingTime, hidden: true),
        CreateColumn("Interval", x => x.Interval, hidden: true),
        CreateColumn("Avg Lap Time", x => x.AvgLapTime, hidden: true),
        CreateColumn("Fastest Lap Time", x => x.FastestLapTime, hidden: true),
        CreateColumn("Position Change", x => x.PositionChange, hidden: true),
        CreateColumn("iRating", x => x.OldIRating, hidden: true),
        //CreateColumn("New iRating", x => x.NewIRating, hidden: true),
        CreateColumn("Season Start iRating", x => x.SeasonStartIRating, hidden: true),
        CreateColumn("License", x => x.License, hidden: true),
        CreateColumn("Safety Rating", x => x.OldSafetyRating, hidden: true),
        //CreateColumn("New Safety Rating", x => x.NewSafetyRating, hidden: true),
        CreateColumn("CPI", x => x.OldCpi, hidden: true),
        //CreateColumn("New CPI", x => x.NewCpi, hidden: true),
        CreateColumn("Club Id", x => x.ClubId, hidden: true),
        CreateColumn("Club Name", x => x.ClubName, hidden: true),
        CreateColumn("Car Id", x => x.CarId, hidden: true),
        CreateColumn("Completed Pct", x => x.CompletedPct, hidden: true),
        CreateColumn("Qualifying Time At", x => x.QualifyingTimeAt, hidden: true),
        CreateColumn("Division", x => x.Division, hidden: true),
        CreateColumn("License Level", x => x.OldLicenseLevel, hidden: true),
        //CreateColumn("New License Level", x => x.NewLicenseLevel, hidden: true),
        CreateColumn("Country Code", x => x.CountryCode, hidden: true),
    ];

    protected override void OnParametersSet()
    {
        BlazorParameterNullException.ThrowIfNull(this, SessionResult);
        BlazorParameterNullException.ThrowIfNull(this, Members);
        ResultRows = SessionResult.ResultRows;
        base.OnParametersSet();
    }

    private async Task<IEnumerable<long>> SearchMembers(string search, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(search))
        {
            return await Task.FromResult(GetMembersNotInResult().Select(x => x.MemberId));
        }

        var terms = search.ToLower().Split(',', ' ', ';')
        .Where(x => string.IsNullOrWhiteSpace(x) == false)
            .ToArray();
        return await Task.FromResult(GetMembersNotInResult()
            .Where(x => MatchMemberSearchTerms(x, terms))
            .Select(x => x.MemberId));
    }

    private string GetMemberNameFromId(long memberId)
    {
        var member = Members.FirstOrDefault(x => x.MemberId == memberId);
        if (member is null)
        {
            return "Not found";
        }

        return $"{member.FirstName} {member.LastName}";
    }

    private static bool MatchMemberSearchTerms(MemberInfoModel member, params string[] terms)
    {
        var searchName = member.FirstName + member.LastName;
        return terms.Any(x => searchName.Contains(x, StringComparison.OrdinalIgnoreCase));
    }

    private IEnumerable<MemberInfoModel> GetMembersNotInResult() =>
        Members.Where(x => SessionResult.ResultRows.None(row => x.MemberId == row.MemberId));

    private async Task<IEnumerable<long?>> SearchTeams(string search, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(search))
        {
            return await Task.FromResult(Teams.Select(x => x.TeamId).Cast<long?>());
        }

        var terms = search.ToLower().Split(',', ' ', ';')
        .Where(x => string.IsNullOrWhiteSpace(x) == false)
            .ToArray();
        return await Task.FromResult(Teams
            .Where(x => MatchTeamSearchTerms(x, terms))
            .Select(x => x.TeamId)
            .Cast<long?>());
    }

    private string GetTeamNameFromId(long? teamId)
    {
        var team = Teams.FirstOrDefault(x => x.TeamId == teamId.GetValueOrDefault());
        if (team is null)
        {
            return "";
        }

        return team.Name;
    }

    private static bool MatchTeamSearchTerms(TeamInfoModel team, params string[] terms)
    {
        return terms.Any(x => team.Name.Contains(x, StringComparison.OrdinalIgnoreCase));
    }

    private void MoveItemUp(RawResultRowViewModel row)
    {
        var rowIndex = ResultRows.IndexOf(row);
        var swapIndex = rowIndex - 1;
        if (swapIndex < 0)
        {
            return;
        }
        var swapRow = ResultRows.ElementAt(swapIndex);
        (row.FinishPosition, swapRow.FinishPosition) = (swapRow.FinishPosition, row.FinishPosition);
        (ResultRows[rowIndex], ResultRows[swapIndex]) = (ResultRows[swapIndex], ResultRows[rowIndex]);
        LastEditedItem = row;
    }

    private void MoveItemDown(RawResultRowViewModel row)
    {
        var rowIndex = ResultRows.IndexOf(row);
        var swapIndex = rowIndex + 1;
        if (swapIndex >= ResultRows.Count)
        {
            return;
        }
        var swapRow = ResultRows.ElementAt(swapIndex);
        (row.FinishPosition, swapRow.FinishPosition) = (swapRow.FinishPosition, row.FinishPosition);
        (ResultRows[rowIndex], ResultRows[swapIndex]) = (ResultRows[swapIndex], ResultRows[rowIndex]);
        LastEditedItem = row;
    }

    private static TimeSpan SetInterval(int laps, int minutes, int seconds, int milliseconds)
    {
        return new TimeSpan(laps, 0, minutes, seconds, milliseconds);
    }

    private static EditColumn<RawResultRowViewModel, T> CreateColumn<T>(string title, Expression<Func<RawResultRowViewModel, T>> property, bool editable = true, bool hidden = false)
        => new(title, property, editable)
        {
            Hidden = hidden,
        };

    private void ApplyRowChanges(RawResultRowViewModel row)
    {
        if (row.HasChanges)
        {
            SessionResult.Changed();
        }
        row.ApplyChanges();
    }

    void StartedEditingItem(RawResultRowViewModel item)
    {
    }

    void CanceledEditingItem(RawResultRowViewModel item)
    {
        item.Reset();
    }

    void CommittedItemChanges(RawResultRowViewModel item)
    {
        if (item.HasChanges)
        {
            SessionResult.Changed();
        }
        item.ApplyChanges();
        LastEditedItem = item;
    }

    private async Task AddRowClick()
    {
        var copyRow = SessionResult.ResultRows.LastOrDefault()?.CopyModel() ?? new();
        var newRow = new RawResultRowModel() {
            FinishPosition = SessionResult.ResultRows.Count + 1,
            StartPosition = SessionResult.ResultRows.Count + 1,
            ClassId = copyRow.ClassId,
            CarClass = copyRow.CarClass,
            Car = copyRow.Car,
            CarId = copyRow.CarId,
            Interval = copyRow.Interval + TimeSpan.FromMilliseconds(1),
            SimSessionType = copyRow.SimSessionType,
            PointsEligible = true,
            Status = (int)RaceStatus.Running,
        };

        var parameters = new DialogParameters<AddResultRowDialog>()
        {
            { x => x.Model, newRow },
            { x => x.Columns, columns },
            { x => x.SessionResult, SessionResult },
            { x => x.Members, Members },
            { x => x.Teams, Teams },
        };
        var result = await DialogService.Show<AddResultRowDialog>("Add row", parameters).Result;
        if (result?.Canceled != false || result.Data is not RawResultRowModel row)
        {
            return;
        }

        await SessionResult.AddRow(row);
    }

    private async Task RemoveRowClick(RawResultRowViewModel row)
    {
        var parameters = new DialogParameters<ConfirmDialog>()
        {
            { x => x.Text, $"Are you sure you want to remove {GetMemberNameFromId(row.MemberId)} from the result?" },
            { x => x.ButtonTypes, ButtonTypes.YesNo  },
        };
        var result = await DialogService.Show<ConfirmDialog>("Confirm remove", parameters).Result;
        if (result?.Canceled != false)
        {
            return;
        }
        SessionResult.RemoveRow(row);
    }
}
