using iRLeagueApiCore.Client.ResultsParsing;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.IO;
using System.Text.Json;

namespace iRLeagueManager.Web.Components;

public partial class UploadResultDialog : PromptDialog<IBrowserFile>
{
    [CascadingParameter]
    public MudDialogInstance DialogInstance { get; set; } = default!;

    [Parameter]
    public EventModel Event { get; set; } = default!;

    private bool FileLoading { get; set; } = false;
    private bool ParsingFailed { get; set; } = false;

    private ParseSimSessionResult? ParsedResult;
    private string? ValidationMessage { get; set; }

    protected override void OnParametersSet()
    {
        _ = Event ?? throw new InvalidOperationException($"Parameter {nameof(Event)} must have a value");
    }

    private async Task<StatusResult> PostResultAsync(ParseSimSessionResult parsedResult)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }
        var result = await ApiService.CurrentLeague.Events()
                .WithId(Event.Id)
                .Results()
                .Upload()
                .Post(parsedResult, CancellationToken).ConfigureAwait(false);
        return result.ToStatusResult();
    }

    protected override async Task Submit()
    {
        if (ParsedResult is null)
        {
            return;
        }

        try
        {
            Loading = true;
            var result = await PostResultAsync(ParsedResult);

            if (result.IsSuccess == false)
            {
                var statusResult = result;
                ValidationMessage = statusResult.Message;
                return;
            }

            DialogInstance?.Close();
        }
        finally
        {
            Loading = false;
        }
    }

#pragma warning disable IDE1006 // Benennungsstile
    private class ParseSimEventResult
    {
        public string? type { get; set; }
        public ParseSimSessionResult? data
        {
            get; set;
        }
#pragma warning restore IDE1006 // Benennungsstile
    }

    private async Task OnFileChanged(IBrowserFile file)
    {

        // try parse the json file
        try
        {
            FileLoading = true;
            ParsedResult = null;
            var browserFile = file;
            string jsonData;
            using (var reader = new StreamReader(browserFile.OpenReadStream(maxAllowedSize: 10240000)))
            {
                jsonData = await reader.ReadToEndAsync();
            }

            var eventResult = JsonSerializer.Deserialize<ParseSimEventResult>(jsonData);
            // Default iracing schema as of 02/2025
            // result with "type" and "data" wrapper
            if (eventResult?.type == "event_result" && eventResult.data is not null)
            {
                ParsedResult = eventResult.data;
            }
            // Fallback for old json schema without "type" / "data
            else
            {
                ParsedResult = JsonSerializer.Deserialize<ParseSimSessionResult>(jsonData);
            }

            // check if type data is set but type is unknown
            if (!string.IsNullOrEmpty(eventResult?.type) && eventResult.type != "event_result")
            {
                ValidationMessage = "Unknown result type. Upload is only supported for \"event_result\" type";
                ParsingFailed = true;
                return;
            }
            // Check if parsing failed generally
            if (ParsedResult is null || ParsedResult.subsession_id == 0)
            {
                ValidationMessage = "Parsing json from iracing result file failed. Please make sure to upload a valid json file.";
                ParsingFailed = true;
                return;
            }

            ParsingFailed = false;
        }
        catch (Exception ex) when (ex is IOException or JsonException or ArgumentNullException or InvalidOperationException
            or ObjectDisposedException)
        {
            ParsingFailed = true;
        }
        finally
        {
            FileLoading = false;
        }
    }
}
