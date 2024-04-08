namespace iRLeagueApiCore.Client.QueryBuilder;

public sealed class ParameterBuilder : IParameterBuilder
{
    private List<string> _parameters;

    public ParameterBuilder()
    {
        _parameters = new List<string>();
    }

    public IParameterBuilder Add<T>(string name, T value)
    {
        _parameters.Add(ParameterString(name, value));
        return this;
    }

    public IParameterBuilder AddArray<T>(string name, IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            _parameters.Add(ParameterString(name, value));
        }
        return this;
    }

    public string Build()
    {
        return string.Join("&", _parameters);
    }

    private string ParameterString<T>(string name, T value)
    {
        return string.Format("{0}={1}", name, value);
    }
}
