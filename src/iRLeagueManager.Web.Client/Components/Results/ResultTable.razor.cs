using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Exceptions;
using Microsoft.AspNetCore.Components;

namespace iRLeagueManager.Web.Components;

public partial class ResultTable<TRow> : ComponentBase, IDisposable
{
    [Parameter(CaptureUnmatchedValues = true)] 
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter, EditorRequired]
    public IEnumerable<TRow> Data { get; set; } = default!;
    [Parameter, EditorRequired]
    public RenderFragment HeaderTemplate { get; set; } = default!;
    [Parameter]
    public RenderFragment<TRow> RowTemplate { get; set; } = default!;

    private SortState<TRow> SortState { get; init; } = new SortState<TRow>();

    protected override void OnInitialized()
    {
        SortState.PropertyChanged += SortState_PropertyChanged;
    }

    private void SortState_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        SortState.PropertyChanged -= SortState_PropertyChanged;
    }

    protected override void OnParametersSet()
    {
        BlazorParameterNullException.ThrowIfNull(this, Data);
        BlazorParameterNullException.ThrowIfNull(this, HeaderTemplate);
        BlazorParameterNullException.ThrowIfNull(this, RowTemplate);
    }
}
