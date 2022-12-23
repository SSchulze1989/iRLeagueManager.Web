using iRLeagueManager.Web.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace iRLeagueManager.Web.Shared;

public class StatusResultValidator : ComponentBase
{
    private ValidationMessageStore messageStore = default!;
    [CascadingParameter]
    private EditContext CurrentEditContext { get; set; } = default!;

    public string ErrorMessage { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        _ = CurrentEditContext ?? throw new InvalidOperationException($"Usage of {nameof(StatusResultValidator)} requires an {nameof(EditContext)} as cascading parameter");
        messageStore = new ValidationMessageStore(CurrentEditContext);
        CurrentEditContext.OnValidationRequested += (sender, args) => ClearErrors();
    }

    private void ClearErrors()
    {
        messageStore.Clear();
        ErrorMessage = string.Empty;
    }

    public void ValidateResult(StatusResult result)
    {
        if (result.IsSuccess)
        {
            messageStore.Clear();
            ErrorMessage = string.Empty;
            return;
        }


        switch (result.Status)
        {
            case StatusResult.Unauthorized:
                AddUnauthorizedValidationMessages();
                break;
            case StatusResult.BadRequest:
                AddBadRequestValidationMessages(result);
                break;
            case StatusResult.ServerError:
                DisplayErrorMessage(result);
                break;
            default:
                DisplayErrorMessage(result);
                break;
        }
    }

    private void DisplayErrorMessage(StatusResult result)
    {
        foreach (var error in result.Errors)
        {
            if (error is Exception)
            {
                ErrorMessage += $"\nError: {error.GetType()}";
                continue;
            }
            ErrorMessage += $"\nError: {error}";
        }
    }

    private void AddUnauthorizedValidationMessages()
    {
        var usernameField = CurrentEditContext.Field("Username");
        var passwordField = CurrentEditContext.Field("Password");
        messageStore.Add(usernameField, "");
        messageStore.Add(passwordField, "Incorrect Username or Password");
    }

    private void AddBadRequestValidationMessages(StatusResult result)
    {
        var validationErrors = result.ValidationErrors;
        foreach (var validationError in validationErrors)
        {
            var fieldName = GetModelFieldName(validationError.Property);
            var identifier = CurrentEditContext.Field(fieldName);
            messageStore.Add(identifier, validationError.Error);
        }
    }

    private static string GetModelFieldName(string requestFieldName)
    {
        if (requestFieldName.StartsWith("Model."))
        {
            return requestFieldName.Substring("Model.".Length);
        }
        return requestFieldName;
    }
}
