using iRLeagueDatabaseCore.Converters;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTests.Converters;

public class TestDictionaryToStringConverter
{
    private const char pairDelimiter = ';';
    private const char valueDelimiter = ':';
    private static Dictionary<string, int> testDict = new Dictionary<string, int>() { { "val1", 1 }, { "val2", 2 } };
    private static string testString = "val1:1;val2:2";

    [Fact]
    public void ShouldConvertIntToString()
    {
        AssertConvertToString(testDict, testString);
    }

    private static void AssertConvertToString<TKey, TValue>(IDictionary<TKey, TValue> dict, string expected)
    {
        var converter = new DictionaryToStringConverter<TKey, TValue>();
        var result = (string)converter.ConvertToProvider(dict);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ShouldConvertToDictionary()
    {
        AssertConvertToDict(testString, testDict);
    }

    private static void AssertConvertToDict<TKey, TValue>(string str, IDictionary<TKey, TValue> expected)
    {
        var converter = new DictionaryToStringConverter<TKey, TValue>();
        var result = (IDictionary<TKey, TValue>)converter.ConvertFromProvider(str);
        Assert.Equal(expected.Count, result.Count);
        for (int i = 0; i < result.Count; i++)
        {
            Assert.Equal(expected.ElementAt(i).Key, result.ElementAt(i).Key);
            Assert.Equal(expected.ElementAt(i).Value, result.ElementAt(i).Value);
        }
    }
}
