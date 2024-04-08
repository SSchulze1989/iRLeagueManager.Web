using iRLeagueApiCore.Common.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace iRLeagueApiCore.Common.Tests;

public sealed class TimeSpanConverterTests
{
    [Fact]
    public void Read_ShouldReadTimeSpan_WithoutDays()
    {
        var json = @"{""TestTime"": ""01:02:03.45600""}";
        var test = JsonSerializer.Deserialize<TestClass>(json);
        test.Should().NotBeNull();
        test!.TestTime.Should().Be(new TimeSpan(0, 1, 2, 3, 456));
    }

    [Fact]
    public void Read_ShouldReadTimeSpan_WithDays()
    {
        var json = @"{""TestTime"": ""01.01:02:03.45600""}";
        var test = JsonSerializer.Deserialize<TestClass>(json);
        test.Should().NotBeNull();
        test!.TestTime.Should().Be(new TimeSpan(1, 1, 2, 3, 456));
    }

    [Fact]
    public void Write_ShouldWriteTimeSpan_WithoutDays()
    {
        var time = new TimeSpan(0, 1, 2, 3, 456);
        var test = JsonSerializer.Serialize(new TestClass() { TestTime = time });
        test.Should().NotBeNull();
        test.Should().Contain("\"01:02:03.45600\"");
    }

    [Fact]
    public void Write_ShouldWriteTimeSpan_WithDays()
    {
        var time = new TimeSpan(1, 1, 2, 3, 456);
        var test = JsonSerializer.Serialize(new TestClass() { TestTime = time });
        test.Should().NotBeNull();
        test.Should().Contain("\"01.01:02:03.45600\"");
    }

    sealed class TestClass
    {
        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan TestTime { get; set; }
    }
}
