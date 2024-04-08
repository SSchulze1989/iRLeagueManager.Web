using iRLeagueApiCore.Client.QueryBuilder;

namespace iRLeagueApiCore.Client.Endpoints;
public interface IEndpoint
{
    string QueryUrl { get; }
    internal void AddEndpoint(string endpoint);
    internal void AddRouteParameter<T>(T parameter);
    internal void WithParameters(Func<IParameterBuilder, IParameterBuilder> parameterBuilder);
}

public static class IEndpointExtensions
{
    public static T AddQueryParameter<T>(this T endpoint, Func<IParameterBuilder, IParameterBuilder> parameterBuilder) where T : IEndpoint
    {
        endpoint.WithParameters(parameterBuilder);
        return endpoint;
    }

    public static T AddEndpoint<T>(this T endpointImpl, string endpoint) where T : IEndpoint
    {
        endpointImpl.AddEndpoint(endpoint);
        return endpointImpl;
    }

    public static TEndpoint AddRouteParameter<TEndpoint, TParam>(this TEndpoint endpoint, TParam parameter) where TEndpoint : IEndpoint
    {
        endpoint.AddRouteParameter(parameter);
        return endpoint;
    }
}
