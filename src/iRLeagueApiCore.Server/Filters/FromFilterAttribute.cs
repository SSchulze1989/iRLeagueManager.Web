namespace iRLeagueApiCore.Server.Filters;

/// <summary>
/// Indicates, that an Action parameter will be filled by an applied Filter an should be ignored by OpenAPI doc generator
/// </summary>
public sealed class FromFilterAttribute : ParameterIgnoreAttribute
{
}
