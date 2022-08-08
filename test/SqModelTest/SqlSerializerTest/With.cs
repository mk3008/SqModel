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
        var text = @"with q as query() select";
        var result = WithParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());
        Assert.Equal(2, actual.Count);
        Assert.Equal(@"q as query() ", actual[0]);
        Assert.Equal("select", actual[1]);
    }

    [Fact]
    public void CrLf()
    {
        var text = @"with q as query() 
select";
        var result = WithParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());
        Assert.Equal(2, actual.Count);
        Assert.Equal(@"q as query() 
", actual[0]);
        Assert.Equal("select", actual[1]);
    }

    [Fact]
    public void Prefix()
    {
        var text = @"    with q as query() select";
        var result = WithParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());
        Assert.Equal(3, actual.Count);
        Assert.Equal(@"    ", actual[0]);
        Assert.Equal(@"q as query() ", actual[1]);
        Assert.Equal("select", actual[2]);
    }

    [Fact]
    public void NotUse()
    {
        var text = @"select";
        var result = WithParser.Parse(text);
        var actual = result.GetValues().ToList();

        Assert.Equal(text, result.FullText());
        Assert.Single(actual);
        Assert.Equal(text, actual[0]);
    }
}
