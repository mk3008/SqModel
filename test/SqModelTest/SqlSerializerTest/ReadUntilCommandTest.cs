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

        p.ReadSkipSpaces();
        var res = p.ReadUntilCommand();

        Assert.Equal("select", res.Command.CommandText);
        Assert.Equal("", res.Value);

        p.ReadSkipSpaces();
        res = p.ReadUntilCommand();

        Assert.Equal("from", res.Command.CommandText);
        Assert.Equal("*", res.Value);

        p.ReadSkipSpaces();
        res = p.ReadUntilCommand();

        Assert.Equal("", res.Command.CommandText);
        Assert.Equal("table", res.Value);
    }

    [Fact]
    public void NotFound()
    {
        using var p = new Parser("selection");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.Equal("", res.Command.CommandText);
        Assert.Equal("selection", res.Value);
    }

    [Fact]
    public void IgnoreCase()
    {
        using var p = new Parser("SELECT");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.Equal("select", res.Command.CommandText);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void Symbol()
    {
        using var p = new Parser("--select");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.Equal("--", res.Command.CommandText);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void SkipSpace()
    {
        using var p = new Parser(" select");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.Equal("select", res.Command.CommandText);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void NotWord()
    {
        using var p = new Parser("testselect");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.Equal("", res.Command.CommandText);
        Assert.Equal("testselect", res.Value);
    }

    [Fact]
    public void NotWordButSymbol()
    {
        using var p = new Parser("test--");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand();

        Assert.Equal("--", res.Command.CommandText);
        Assert.Equal("test", res.Value);
    }

    [Fact]
    public void CustomCommand()
    {
        using var p = new Parser("test");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand(new[] { new Command("test") });

        Assert.Equal("test", res.Command.CommandText);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void CustomCommand_NotFound()
    {
        using var p = new Parser("tets");
        p.Logger = (x) => Output.WriteLine(x);

        var res = p.ReadUntilCommand(new[] { new Command("test") });

        Assert.Equal("", res.Command.CommandText);
        Assert.Equal("tets", res.Value);
    }
}
