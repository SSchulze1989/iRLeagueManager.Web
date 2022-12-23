using iRLeagueApiCore.Common.Responses;

namespace iRLeagueManager.Web.Data;

public class StatusResult
{
    public StatusResult(bool success, string status)
    {
        IsSuccess = success;
        Status = status;
    }

    public StatusResult(bool success, string status, string message) :
        this(success, status)
    {
        Message = message;
    }

    public StatusResult(bool success, string status, string message, IEnumerable<object> errors) :
        this(success, status, message)
    {
        Message = message;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public string Status { get; } = string.Empty;
    public string Message { get; } = string.Empty;
    public IEnumerable<object> Errors { get; } = Array.Empty<object>();
    public IEnumerable<ValidationError> ValidationErrors => Errors.OfType<ValidationError>();

    public const string Success = "Success";
    public const string BadRequest = "Bad request";
    public const string Unauthorized = "Unauthorized";
    public const string ServerError = "Internal server Error";

    public static StatusResult SuccessResult()
    {
        return new StatusResult(true, "Success");
    }

    public static StatusResult SuccessResult(string message)
    {
        return new StatusResult(true, "Success", message);
    }

    public static StatusResult FailedResult(string status, string message, IEnumerable<object> errors)
    {
        return new StatusResult(false, status, message, errors);
    }
}
