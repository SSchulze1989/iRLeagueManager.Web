using iRLeagueApiCore.Client.QueryBuilder;

namespace iRLeagueApiCore.UnitTests.Client.QueryBuilder;

public sealed class RouteBuilderTests
{
    [Fact]
    public void ShouldBuildRouteWithNameAndValue()
    {
        var name = "test";
        var value = "value";

        var test = new RouteBuilder()
            .AddEndpoint(name)
            .AddParameter(value)
            .Build();

        Assert.Equal("test/value", test);
    }

    [Fact]
    public void ShouldBuildRouteWithMulitpleNamesAndValues()
    {
        var name1 = "test1";
        var value1 = "value1";
        var name2 = "test2";
        var name3 = "test3";
        var value3 = "value3";

        var test = new RouteBuilder()
            .AddEndpoint(name1)
            .AddParameter(value1)
            .AddEndpoint(name2)
            .AddEndpoint(name3)
            .AddParameter(value3)
            .Build();

        Assert.Equal("test1/value1/test2/test3/value3", test);
    }

    [Fact]
    public void ShouldBuildRouteWithParameters()
    {
        var name = "test";
        var value = "value";

        var parameter1 = "param1";
        var paramValue1 = "paramValue1";
        var parameter2 = "param2";
        var paramValue2 = "paramValue2";

        var test = new RouteBuilder()
            .AddEndpoint(name)
            .AddParameter(value)
            .WithParameters(param => param
                .Add(parameter1, paramValue1)
                .Add(parameter2, paramValue2)
                )
            .Build();

        Assert.Equal("test/value?param1=paramValue1&param2=paramValue2", test);
    }
}
