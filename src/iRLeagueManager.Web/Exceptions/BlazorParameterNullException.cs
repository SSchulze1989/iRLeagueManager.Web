namespace iRLeagueManager.Web.Exceptions;

public static class BlazorParameterNullException
{
    public static BlazorParameterNullException<TComponent, TParameter> New<TComponent, TParameter>(TComponent component, TParameter parameter, bool cascading = false)
    {
        return new BlazorParameterNullException<TComponent, TParameter>(cascading: cascading);
    }

    public static BlazorParameterNullException<TComponent, TParameter> New<TComponent, TParameter>(TComponent component, TParameter parameter, string parameterName, bool cascading = false)
    {
        return new BlazorParameterNullException<TComponent, TParameter>(parameterName, cascading: cascading);
    }

    public static void ThrowIfNull<TComponent, TParameter>(TComponent component, TParameter parameter, bool cascading = false)
    {
        if (parameter is null)
        {
            throw New(component, parameter, cascading: cascading);
        }
    }

    public static void ThrowIfNull<TComponent, TParameter>(TComponent component, TParameter parameter, string parameterName, bool cascading = false)
    {
        if (parameter is null)
        {
            throw New(component, parameter, parameterName, cascading: cascading);
        }
    }
}

public sealed class BlazorParameterNullException<TComponent, TParameter> : InvalidOperationException
{
    public BlazorParameterNullException(bool cascading = false) :
        base($"Usage of {typeof(TComponent)} requires a {(cascading ? "Cascading" : "")}Parameter of type {typeof(TParameter)} but value was 'null'")
    {
    }

    public BlazorParameterNullException(string parameterName, bool cascading = false) :
        base($"Usage of {typeof(TComponent)} requires a {(cascading ? "Cascading" : "")}Parameter of type {typeof(TParameter)} with name {parameterName} but value was 'null'")
    {
    }
}
