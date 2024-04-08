namespace iRLeagueApiCore.Common.Responses;

public struct BadRequestResponse
{
    public string Status { get; set; }
    public IEnumerable<ValidationError> Errors { get; set; }
}
