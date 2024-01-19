using iRLeagueManager.Web.Exceptions;
using iRLeagueManager.Web.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace iRLeagueManager.Web.Components.Dialogs;

public class PromptDialog<T> : ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public T? Value { get; set; } = default;
    [Parameter] public Func<T?, CancellationToken, Task<bool>>? OnSubmit { get; set; }
    [Parameter] public Func<T?, CancellationToken, Task>? OnCancel { get; set; }
    [Parameter] public string OkText { get; set; } = "Ok";
    [Parameter] public string CancelText { get; set; } = "Cancel";
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? HelperText { get; set; }
    [Parameter] public Variant Variant { get; set; } = Variant.Outlined;

    protected CancellationTokenSource Cts { get; } = new();
    protected StatusResultValidator? ResultValidator { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        BlazorParameterNullException.ThrowIfNull(this, MudDialog);
    }

    protected virtual async Task Submit()
    {
        if (OnSubmit is null || await OnSubmit.Invoke(Value, Cts.Token))
        {
            MudDialog.Close(Value);
        }
    }

    protected virtual void Cancel()
    {
        MudDialog.Cancel();
    }
}