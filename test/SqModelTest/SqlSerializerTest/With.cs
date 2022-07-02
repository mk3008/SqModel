using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class With
{
    [Fact]
    public void Default()
    {
        var text = @"with end";
        var result = WithParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());
        Assert.Single(actual);
        Assert.Equal("end", actual[0]);
    }

    [Fact]
    public void CrLfSeparator()
    {
        var text = @"with
end";
        var result = WithParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());
        Assert.Single(actual);
        Assert.Equal("end", actual[0]);
    }

    [Fact]
    public void PrefixSeparator()
    {
        var text = @"    with
end";
        var result = WithParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());
        Assert.Single(actual);
        Assert.Equal("end", actual[0]);
    }

    [Fact]
    public void NotUse()
    {
        var text = @"    end";
        var result = WithParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());
        Assert.Single(actual);
        Assert.Equal(text, actual[0]);
    }
}
