using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class SingleQuote
{
    [Fact]
    public void Default()
    {
        var text = @"'text'";
        var result = SingleQuoteParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());
        Assert.Single(actual);
        Assert.Equal("text", actual[0]);
    }

    [Fact]
    public void PrefixSufix()
    {
        var text = @"start'text'end";
        var result = SingleQuoteParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());
        Assert.Equal(3, actual.Count);
        Assert.Equal("start", actual[0]);
        Assert.Equal("text", actual[1]);
        Assert.Equal("end", actual[2]);
    }

    [Fact]
    public void Multiple()
    {
        var text = @"start'text1','text2'end";
        var result = SingleQuoteParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());

        Assert.Equal(text, result.FullText());
        Assert.Equal(5, actual.Count);
        Assert.Equal("start", actual[0]);
        Assert.Equal("text1", actual[1]);
        Assert.Equal(",", actual[2]);
        Assert.Equal("text2", actual[3]);
        Assert.Equal("end", actual[4]);
    }

    [Fact]
    public void ResultLock()
    {
        var text = @"start'text1(text2)'end";
        var result = SingleQuoteParser.Parse(text).Segmentation(ParentheseParser.Parse);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());

        Assert.Equal(text, result.FullText());
        Assert.Equal(3, actual.Count);
        Assert.Equal("start", actual[0]);
        Assert.Equal("text1(text2)", actual[1]);
        Assert.Equal("end", actual[2]);
    }
}
