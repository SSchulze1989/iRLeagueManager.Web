namespace iRLeagueApiCore.Client.QueryBuilder;

public interface IParameterBuilder
{
    IParameterBuilder Add<T>(string name, T value);
    IParameterBuilder AddArray<T>(string name, IEnumerable<T> values);
    string Build();
}
