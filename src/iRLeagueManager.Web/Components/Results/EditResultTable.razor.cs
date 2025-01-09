using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Exceptions;
using iRLeagueManager.Web.Extensions;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;
using System.Reflection;

namespace iRLeagueManager.Web.Components;

public partial class EditResultTable
{
    private class BindingHelper(RawResultRowModel Row, EditColumn Column)
    {
        public string? Value
        {
            get => Column.GetValue(Row);
            set => Column.SetValue(Row, value);
        }
    }

    private class EditColumn
    {
        private readonly Func<RawResultRowModel, string?> getFunc;
        private readonly Action<RawResultRowModel, string?> setFunc;

        public string Description { get; }
        public string Name { get; }
        public Type Type { get; }

        public EditColumn(string name, Func<RawResultRowModel, string?> getFunc, Action<RawResultRowModel, string?> setFunc, Type type)
        {
            Description = Name = name;
            Type = type;
            this.getFunc = getFunc;
            this.setFunc = setFunc;
        }

        public EditColumn(Expression<Func<RawResultRowModel, object>> property, string? description = null)
        {
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

            getFunc = row => (string?)System.Convert.ChangeType(propertyInfo.GetValue(row), typeof(string));
            setFunc = (row, value) => propertyInfo.SetValue(row, System.Convert.ChangeType(value, propertyInfo.PropertyType));
            Name = propertyInfo.Name;
            Type = propertyInfo.PropertyType;
            Description = description ?? Name;
        }

        public string? GetValue(RawResultRowModel row) => getFunc(row);

        public void SetValue(RawResultRowModel row, string? value) => setFunc(row, value);
    }

    [Parameter, EditorRequired] public RawSessionResultViewModel SessionResult { get; set; } = default!;
    [Parameter, EditorRequired] public IEnumerable<MemberInfoModel> Members { get; set; } = default!;

    private List<RawResultRowViewModel> ResultRows { get; set; } = default!;

    protected override void OnParametersSet()
    {
        BlazorParameterNullException.ThrowIfNull(this, SessionResult);
        BlazorParameterNullException.ThrowIfNull(this, Members);
        ResultRows = SessionResult.ResultRows.ToList();
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

    private readonly List<EditColumn> columns = [
        new (x => x.FinishPosition),
        new (x => x.MemberId),
        new (x => x.CompletedLaps),
        new (x => x.RacePoints, "Points"),
        new (x => x.CompletedLaps),
        new ("Interval", x => x.Interval.LapTimeString() , (row, value) => row.Interval = value != null ? TimeSpan.Parse(value) : TimeSpan.Zero, typeof(TimeSpan))
    ];

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
        Members.Where(x => SessionResult.ResultRows.None(row => x.MemberId == row?.MemberId));

    private void MoveRowUp(RawResultRowViewModel row)
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
    }

    private void MoveRowDown(RawResultRowViewModel row)
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
    }

    private static TimeSpan SetInterval(int laps, int minutes, int seconds, int milliseconds)
    {
        return new TimeSpan(laps, 0, minutes, seconds, milliseconds);
    }

    private void ApplyRowChanges(RawResultRowViewModel row)
    {
        if (row.HasChanges)
        {
            SessionResult.Changed();
        }
        row.ApplyChanges();
    }
}
