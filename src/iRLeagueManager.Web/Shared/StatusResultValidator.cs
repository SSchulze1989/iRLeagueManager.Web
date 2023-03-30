using iRLeagueManager.Web.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.RegularExpressions;

namespace iRLeagueManager.Web.Shared;

public sealed class StatusResultValidator : ComponentBase
{
    private ValidationMessageStore messageStore = default!;
    [CascadingParameter]
    private EditContext CurrentEditContext { get; set; } = default!;

    /// <summary>
    /// Regex string to trim the prefix of field identifiers from validation errors
    /// -> Default = "Model."
    /// </summary>
    [Parameter]
    public string TrimPrefix { get; set; } = "Model.";

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
                AddUnauthorizedValidationMessages(result);
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

    private void AddUnauthorizedValidationMessages(StatusResult result)
    {
        if (result.Message == "MailConfirm")
        {
            ErrorMessage = "Email confirmation is missing";
            return;
        }

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

    private string GetModelFieldName(string requestFieldName)
    {
        // Trim prefix from field identifier
        requestFieldName = Regex.Replace(requestFieldName, TrimPrefix, "");
        return requestFieldName;
    }
}
