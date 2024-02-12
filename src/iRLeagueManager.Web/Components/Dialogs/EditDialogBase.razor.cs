using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Exceptions;
using iRLeagueManager.Web.Shared;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MvvmBlazor.Components;

namespace iRLeagueManager.Web.Components.Dialogs;

public class EditDialogBase<TViewModel, TModel> : MvvmComponentBase where TViewModel : LeagueViewModelBase<TViewModel, TModel> where TModel : class
{
    [Inject]
    protected TViewModel Vm { get; set; } = default!;

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter, EditorRequired]
    public TModel Model { get; set; } = default!;

    [Parameter]
    public Func<TViewModel, CancellationToken, Task<StatusResult>>? OnSubmit { get; set; }
    [Parameter]
    public Func<TViewModel, CancellationToken, Task>? OnCancel { get; set; }

    protected CancellationTokenSource Cts { get; } = new();
    protected StatusResultValidator? ResultValidator { get; set; }
    protected bool Loading => Vm.Loading;
    protected bool HasChanged => Vm.HasChanges;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnParametersSet()
    {
        BlazorParameterNullException.ThrowIfNull(this, MudDialog);
        BlazorParameterNullException.ThrowIfNull(this, Model);

        if (EqualityComparer<TModel>.Default.Equals(Model, Vm.GetModel()) == false)
        {
            Vm.SetModel(Model);
        }
    }

    protected virtual async Task Submit()
    {
        var success = true;
        if (OnSubmit is not null)
        {
            var status = await OnSubmit(Vm, Cts.Token);
            success &= status.IsSuccess;
            ResultValidator?.ValidateResult(status);
        }
        if (success)
        {
            MudDialog.Close(Vm.GetModel());
        }
    }

    protected virtual async Task Cancel()
    {
        if (OnCancel is not null)
        {
            await OnCancel(Vm, Cts.Token);
        }
        MudDialog.Cancel();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing == false)
        {
            Cts.Cancel();
            Cts.Dispose();
        }
        base.Dispose(disposing);
    }
}
