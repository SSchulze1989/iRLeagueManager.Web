using iRLeagueDatabaseCore.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace UnitTests.Converters;

public class TestCollectionToStringConverter
{
    private const char delimiter = ';';

    [Theory]
    [InlineData(new int[] { 1, 3, 5, 8, 7 }, "1;3;5;8;7")]
    [InlineData(new int[] { 0, 4, 42, 3 }, "0,4,42,3", ',')]
    [InlineData(new int[] { 1, 0, 52345 }, "1;0;52345")]
    public void ShouldConvertIntToString(IEnumerable<int> intCollection, string arrayStr, char delimiter = delimiter)
    {
        var converter = new CollectionToStringConverter<int>(delimiter);
        var testStr = (string)converter.ConvertToProvider(intCollection);
        Assert.Equal(arrayStr, testStr);
    }

    [Theory]
    [InlineData(new double[] { 1.2, 2.4 }, "1.2;2.4")]
    public void ShouldConvertToDoubleString(IEnumerable<double> doubleCollection, string arrayStr)
    {
        var converter = new CollectionToStringConverter<double>();
        var testStr = (string)converter.ConvertToProvider(doubleCollection);
        Assert.Equal(arrayStr, testStr);
    }

    [Theory]
    [InlineData(new double[] { 1.2, 2.4 }, "1,2;2,4")]
    public void ShouldConvertToDoubleStringWithDifferentCulture(IEnumerable<double> doubleCollection, string arrayStr)
    {
        var converter = new CollectionToStringConverter<double>(CultureInfo.GetCultureInfo("de-DE"));
        var testStr = (string)converter.ConvertToProvider(doubleCollection);
        Assert.Equal(arrayStr, testStr);
    }

    [Theory]
    [InlineData("1;3;5;8;7", new int[] { 1, 3, 5, 8, 7 })]
    [InlineData("0,4,42,3", new int[] { 0, 4, 42, 3 }, ',')]
    [InlineData("0.1.2.3", new int[] { 0, 1, 2, 3 }, '.')]
    [InlineData("1;;52345;", new int[] { 1, 52345 })]
    public void ShouldConvertToIntCollection(string arrayStr, IEnumerable<int> intCollection, char delimiter = delimiter)
    {
        var converter = new CollectionToStringConverter<int>(delimiter);
        var testCollection = (ICollection<int>)converter.ConvertFromProvider(arrayStr);
        Assert.Equal(intCollection, testCollection);
    }

    [Theory]
    [InlineData("1.2;2.4", new double[] { 1.2, 2.4 })]
    public void ShouldConvertToDoubleCollection(string arrayStr, IEnumerable<double> doubleCollection)
    {
        var converter = new CollectionToStringConverter<double>();
        var testCollection = (ICollection<double>)converter.ConvertFromProvider(arrayStr);
        Assert.Equal(doubleCollection, testCollection);
    }

    [Theory]
    [InlineData("1;2;3", ',')]
    [InlineData("1,2.3", ',')]
    [InlineData("1,2.3", '.')]
    [InlineData("1;2;a")]
    public void ShouldThrowOnInvalidFormat(string arrayStr, char delimiter = delimiter)
    {
        var converter = new CollectionToStringConverter<int>(delimiter);
        Assert.Throws<FormatException>(() => (ICollection<int>)converter.ConvertFromProvider(arrayStr));
    }
}
