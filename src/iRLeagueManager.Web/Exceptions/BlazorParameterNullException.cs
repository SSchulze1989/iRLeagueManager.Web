namespace iRLeagueManager.Web.Exceptions;

public static class BlazorParameterNullException
{
    public static BlazorParameterNullException<TComponent, TParameter> New<TComponent, TParameter>(TComponent component, TParameter parameter)
    {
        return new BlazorParameterNullException<TComponent, TParameter>();
    }

    public static BlazorParameterNullException<TComponent, TParameter> New<TComponent, TParameter>(TComponent component, TParameter parameter, string parameterName)
    {
        return new BlazorParameterNullException<TComponent, TParameter>(parameterName);
    }
}

public sealed class BlazorParameterNullException<TComponent, TParameter> : InvalidOperationException
{
    public BlazorParameterNullException() :
        base($"Usage of {typeof(TComponent)} requires a Parameter of type {typeof(TParameter)} but value was 'null'")
    {
    }

    public BlazorParameterNullException(string parameterName) :
        base($"Usage of {typeof(TComponent)} requires a Parameter of type {typeof(TParameter)} with name {parameterName} but value was 'null'")
    {
    }
}
