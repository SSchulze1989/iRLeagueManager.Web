using System.Text;

namespace iRLeagueApiCore.Client.QueryBuilder;

public sealed class RouteBuilder : IRouteBuilder
{
    private readonly List<string> parts;

    private IParameterBuilder? ParameterBuilder { get; set; }

    public RouteBuilder()
    {
        parts = new List<string>();
    }

    public RouteBuilder(List<string> parts)
    {
        this.parts = parts;
    }

    public IRouteBuilder AddEndpoint(string name)
    {
        parts.Add(name);
        return this;
    }

    public IRouteBuilder AddParameter<T>(T value)
    {
        parts.Add(value?.ToString() ?? string.Empty);
        return this;
    }

    public string Build()
    {
        var builder = new StringBuilder();
        var joinedString = string.Join("/", parts);
        builder.Append(joinedString);
        if (ParameterBuilder != null)
        {
            var parameterString = ParameterBuilder.Build();
            if (string.IsNullOrEmpty(parameterString) == false)
            {
                builder.AppendFormat("?{0}", parameterString);
            }
        }
        return builder.ToString();
    }

    public IRouteBuilder WithParameters(Func<IParameterBuilder, IParameterBuilder> parameterBuilder)
    {
        ParameterBuilder = parameterBuilder.Invoke(new ParameterBuilder());
        return this;
    }

    public RouteBuilder Copy()
    {
        return new RouteBuilder(new List<string>(parts));
    }

    public IRouteBuilder RemoveLast()
    {
        if (parts.Count > 0)
            parts.RemoveAt(parts.Count - 1);
        return this;
    }
}
