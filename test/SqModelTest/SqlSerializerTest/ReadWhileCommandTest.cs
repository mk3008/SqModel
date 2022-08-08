using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ReadWhileCommandTest
{
    public ReadWhileCommandTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Find()
    {
        using var p = new Parser("select * from table");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadWhileCommand();
        Assert.Equal("", res.Value);
        Assert.Equal("select", res.Command.CommandText);
        Assert.Equal("", res.SufixSymbol.CommandText);
    }

    [Fact]
    public void NotFind()
    {
        using var p = new Parser("aselect * from table");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadWhileCommand();
        Assert.Equal("as", res.Value);
        Assert.Equal("", res.Command.CommandText);
        Assert.Equal("", res.SufixSymbol.CommandText);
    }

    [Fact]
    public void FindSufixSymbol()
    {
        using var p = new Parser("as--elect * from table");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadWhileCommand();
        Assert.Equal("", res.Value);
        Assert.Equal("as", res.Command.CommandText);
        Assert.Equal("--", res.SufixSymbol.CommandText);
    }

    [Fact]
    public void FindLast()
    {
        using var p = new Parser("select");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadWhileCommand();
        Assert.Equal("", res.Value);
        Assert.Equal("select", res.Command.CommandText);
        Assert.Equal("", res.SufixSymbol.CommandText);
    }

    [Fact]
    public void NotFindLast()
    {
        using var p = new Parser("se");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadWhileCommand();
        Assert.Equal("se", res.Value);
        Assert.Equal("", res.Command.CommandText);
        Assert.Equal("", res.SufixSymbol.CommandText);
    }

    [Fact]
    public void IgnoreCase()
    {
        using var p = new Parser("SELECT");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadWhileCommand();
        Assert.Equal("", res.Value);
        Assert.Equal("select", res.Command.CommandText);
        Assert.Equal("", res.SufixSymbol.CommandText);
    }
}
