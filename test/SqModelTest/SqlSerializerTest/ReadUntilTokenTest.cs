using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ReadUntilTokenTest
{
    public ReadUntilTokenTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        using var p = new Parser("select * from table_a");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken();

        Assert.Equal("select", res.NextToken);
        Assert.Equal("", res.Value);

        p.ReadWhileSpace();
        res = p.ReadUntilToken();

        Assert.Equal("*", res.NextToken);
        Assert.Equal("", res.Value);

        p.ReadWhileSpace();
        res = p.ReadUntilToken();

        Assert.Equal("from", res.NextToken);
        Assert.Equal("", res.Value);

        p.ReadWhileSpace();
        res = p.ReadUntilToken();

        Assert.Equal("", res.NextToken);
        Assert.Equal("table_a", res.Value);
    }

    [Fact]
    public void NotFound()
    {
        using var p = new Parser("selection select");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken().Trim();

        Assert.Equal("select", res.NextToken);
        Assert.Equal("selection", res.Value);
    }

    [Fact]
    public void NotFound_Many()
    {
        using var p = new Parser("selection a select");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken().Trim();

        Assert.Equal("select", res.NextToken);
        Assert.Equal("selection a", res.Value);
    }

    [Fact]
    public void IgnoreCase()
    {
        using var p = new Parser("SELECT");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken();

        Assert.Equal("select", res.NextToken);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void SymbolToken_Minus()
    {
        using var p = new Parser("-select");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken();

        Assert.Equal("-", res.NextToken);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void LineCommentToken()
    {
        using var p = new Parser("--select");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken();

        Assert.Equal("--", res.NextToken);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void SymbolToken_Slash()
    {
        using var p = new Parser("/select");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken();

        Assert.Equal("/", res.NextToken);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void BlockCommentToken()
    {
        using var p = new Parser("/*select");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken();

        Assert.Equal("/*", res.NextToken);
        Assert.Equal("", res.Value);
    }

    [Fact]
    public void NotWord()
    {
        using var p = new Parser("testselect select");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken().Trim();

        Assert.Equal("select", res.NextToken);
        Assert.Equal("testselect", res.Value);
    }

    [Fact]
    public void NotWord_Spaces()
    {
        using var p = new Parser("testselect       select");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken().Trim();

        Assert.Equal("select", res.NextToken);
        Assert.Equal("testselect", res.Value);
    }

    [Fact]
    public void NotWordButSymbol()
    {
        using var p = new Parser("test--");
        p.Logger = (x) => Output.WriteLine(x);

        p.ReadWhileSpace();
        var res = p.ReadUntilToken().Trim();

        Assert.Equal("--", res.NextToken);
        Assert.Equal("test", res.Value);
    }

    //[Fact]
    //public void SkipSpace()
    //{
    //    using var p = new Parser(" select");
    //    p.Logger = (x) => Output.WriteLine(x);

    //    var res = p.ReadUntilCommand();

    //    Assert.Equal("select", res.Command.CommandText);
    //    Assert.Equal("", res.Value);
    //}





    //[Fact]
    //public void CustomCommand()
    //{
    //    using var p = new Parser("test");
    //    p.Logger = (x) => Output.WriteLine(x);

    //    var res = p.ReadUntilCommand(new[] { new Token("test") });

    //    Assert.Equal("test", res.Command.CommandText);
    //    Assert.Equal("", res.Value);
    //}

    //[Fact]
    //public void CustomCommand_NotFound()
    //{
    //    using var p = new Parser("tets");
    //    p.Logger = (x) => Output.WriteLine(x);

    //    var res = p.ReadUntilCommand(new[] { new Token("test") });

    //    Assert.Equal("", res.Command.CommandText);
    //    Assert.Equal("tets", res.Value);
    //}
}
