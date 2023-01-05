using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace iRLeagueManager.Web.Components;

public class InputTimeSpan : InputBase<TimeSpan>
{
    /// <summary>
    /// Gets or sets the associated <see cref="ElementReference"/>.
    /// <para>
    /// May be <see langword="null"/> if accessed before the component is rendered.
    /// </para>
    /// </summary>
    [DisallowNull] public ElementReference? Element { get; protected set; }

    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TimeSpan result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (TimeSpan.TryParse(value, out result) == false)
        {
            validationErrorMessage = "Input is not a valid time span format";
            result = TimeSpan.Zero;
            return false;
        }
        validationErrorMessage = null;
        return true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", "time");
        builder.AddAttribute(3, "class", CssClass);
        builder.AddAttribute(4, "value", BindConverter.FormatValue(CurrentValueAsString));
        builder.AddAttribute(5, "onchange", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
        builder.AddElementReferenceCapture(6, __inputReference => Element = __inputReference);
        builder.CloseElement();
    }
}
