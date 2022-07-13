using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ReadUntilCommandTest
{
    public ReadUntilCommandTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        using var p = new Parser("select * from table");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.True(res.IsSuccess);
        Assert.Equal("select", res.Command);
        Assert.Equal("", res.Value);

        res = p.ReadUntilCommand();

        Assert.True(res.IsSuccess);
        Assert.Equal("from", res.Command);
        Assert.Equal("*", res.Value);

        res = p.ReadUntilCommand();

        Assert.False(res.IsSuccess);
        Assert.Equal("", res.Command);
        Assert.Equal("table", res.Value);
    }

    [Fact]
    public void NotFound()
    {
        using var p = new Parser("selection");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.False(res.IsSuccess);
        Assert.Equal(String.Empty, res.Command);
        Assert.Equal("selection", res.Value);
    }

    [Fact]
    public void IgnoreCase()
    {
        using var p = new Parser("SELECT");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.True(res.IsSuccess);
        Assert.Equal("select", res.Command);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void Symbol()
    {
        using var p = new Parser("--select");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.True(res.IsSuccess);
        Assert.Equal("--", res.Command);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void SkipSpace()
    {
        using var p = new Parser(" select");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.True(res.IsSuccess);
        Assert.Equal("select", res.Command);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void NotWord()
    {
        using var p = new Parser("testselect");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.False(res.IsSuccess);
        Assert.Equal("", res.Command);
        Assert.Equal("testselect", res.Value);
    }

    [Fact]
    public void NotWordButSymbol()
    {
        using var p = new Parser("test--");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.True(res.IsSuccess);
        Assert.Equal("--", res.Command);
        Assert.Equal("test", res.Value);
    }

    [Fact]
    public void CustomCommand()
    {
        using var p = new Parser("test");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand(new[] { new CommandString("test") });

        Assert.True(res.IsSuccess);
        Assert.Equal("test", res.Command);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void CustomCommand_NotFound()
    {
        using var p = new Parser("tets");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand(new[] { new CommandString("test") });

        Assert.False(res.IsSuccess);
        Assert.Equal("", res.Command);
        Assert.Equal("tets", res.Value);
    }
}
