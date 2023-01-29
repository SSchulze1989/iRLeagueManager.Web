using Blazored.Modal;
using Blazored.Modal.Services;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Exceptions;
using iRLeagueManager.Web.Shared;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MvvmBlazor.Components;

namespace iRLeagueManager.Web.Components;

public class EditModalBase<TViewModel, TModel> : MvvmComponentBase where TViewModel : LeagueViewModelBase<TViewModel, TModel>
{
    [Inject]
    protected TViewModel Vm { get; set; } = default!;
    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    [CascadingParameter]
    public BlazoredModalInstance ModalInstance { get; set; } = default!;
    [CascadingParameter]
    public IModalService ModalService { get; set; } = default!;

    private TModel model = default!;
    [Parameter, EditorRequired]
    public TModel Model
    {
        get => model;
        set
        {
            if (EqualityComparer<TModel>.Default.Equals(model, value) == false)
            {
                model = value;
                Vm.SetModel(model);
            }
        }
    }

    [Parameter]
    public Func<TViewModel, CancellationToken, Task<StatusResult>>? OnSubmit { get; set; }
    [Parameter]
    public Func<TViewModel, CancellationToken, Task>? OnCancel { get; set; }

    protected CancellationTokenSource Cts { get; } = new();
    protected StatusResultValidator? ResultValidator { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender == false)
        {
            return;
        }
        await JSRuntime.InvokeVoidAsync("enableTooltips", "right");
    }

    protected override void OnParametersSet()
    {
        _ = ModalInstance ?? throw BlazorParameterNullException.New(this, ModalInstance);
        _ = ModalService ?? throw BlazorParameterNullException.New(this, ModalService);
        _ = Model ?? throw BlazorParameterNullException.New(this, Model);
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
            var result = ModalResult.Ok(Vm.GetModel());
            await ModalInstance.CloseAsync(result);
        }
    }

    protected virtual async Task Cancel()
    {
        if (OnCancel is not null)
        {
            await OnCancel(Vm, Cts.Token);
        }
        await ModalInstance.CancelAsync();
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
