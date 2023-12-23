using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace iRLeagueManager.Web.Components.Settings;

public class SettingBase : MudComponentBase
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string Text { get; set; } = string.Empty;
    [Parameter] public string HelperText { get; set; } = string.Empty;
    [Parameter] public Color Color { get; set; } = Color.Inherit;
    [Parameter] public string Icon { get; set; } = string.Empty;
    [Parameter] public Size IconSize { get; set; } = Size.Medium;
    [Parameter] public Color IconColor { get; set; } = Color.Inherit;
    [Parameter] public bool Disabled { get; set; } = false;
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected bool displayLabel => string.IsNullOrEmpty(Label) == false;
    protected bool displayHelperText => string.IsNullOrEmpty(HelperText) == false;
}