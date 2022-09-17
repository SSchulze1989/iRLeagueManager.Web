using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace iRLeagueManager.Web.Shared
{
    public partial class CancelSubmitForm
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; } = default;
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; } = default;
        [Parameter]
        public EditContext? EditContext { get; set; } = default;
        [Parameter]
        public object? Model { get; set; } = default;
        [Parameter]
        public string SubmitText { get; set; } = "Submit";
        [Parameter]
        public string CancelText { get; set; } = "Cancel";
        [Parameter]
        public EventCallback<EditContext> OnSubmit { get; set; }
        [Parameter]
        public EventCallback<EditContext> OnValidSubmit { get; set; }
        [Parameter]
        public EventCallback<EditContext> OnInvalidSubmit { get; set; }
        [Parameter]
        public EventCallback<EditContext> OnCancel { get; set; }

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync(EditContext);
        }
    }
}
