using iRLeagueApiCore.Client.QueryBuilder;

namespace iRLeagueApiCore.UnitTests.Client.QueryBuilder;

public sealed class ParameterBuilderTests
{
    [Fact]
    public void ShouldBuildSingleParameter()
    {
        const string parameterName = "test";
        const string parameterValue = "value";

        var test = new ParameterBuilder()
            .Add(parameterName, parameterValue)
            .Build();

        Assert.Equal("test=value", test);
    }

    [Fact]
    public void ShouldBuildMultipleParameters()
    {
        const string parameterName1 = "test";
        const string parameterValue1 = "value";
        const string parameterName2 = "test2";
        const string parameterValue2 = "value2";
        const string parameterName3 = "test3";
        const string parameterValue3 = "value3";

        var test = new ParameterBuilder()
            .Add(parameterName1, parameterValue1)
            .Add(parameterName2, parameterValue2)
            .Add(parameterName3, parameterValue3)
            .Build();

        Assert.Equal("test=value&test2=value2&test3=value3", test);
    }

    [Fact]
    public void ShouldBuildArrayParameters()
    {
        const string parameterName = "test";
        var values = new string[]
        {
                "value1",
                "value2",
                "value3"
        };

        var test = new ParameterBuilder()
            .AddArray(parameterName, values)
            .Build();

        Assert.Equal("test=value1&test=value2&test=value3", test);
    }
}
