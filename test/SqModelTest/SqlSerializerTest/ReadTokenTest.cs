using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ReadTokenTest
{
    public ReadTokenTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Split()
    {
        /// inner join table_b as b on ...
        var relationSql = "a b\rc\nd\r\ne\tf";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(6, lst.Count);
        Assert.Equal("a", lst[0]);
        Assert.Equal("b", lst[1]);
        Assert.Equal("c", lst[2]);
        Assert.Equal("d", lst[3]);
        Assert.Equal("e", lst[4]);
        Assert.Equal("f", lst[5]);
    }

    [Fact]
    public void Word()
    {
        /// inner join table_b as b on ...
        var relationSql = "a bc def";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("a", lst[0]);
        Assert.Equal("bc", lst[1]);
        Assert.Equal("def", lst[2]);
    }

    [Fact]
    public void Symbol()
    {
        /// inner join table_b as b on ...
        var relationSql = ". .. ...";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal(".", lst[0]);
        Assert.Equal("..", lst[1]);
        Assert.Equal("...", lst[2]);
    }

    [Fact]
    public void LineComment()
    {
        /// inner join table_b as b on ...
        var relationSql = "start-- comment\r\nend";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("-- comment", lst[1]);
        Assert.Equal("end", lst[2]);
    }

    [Fact]
    public void BlockComment()
    {
        /// inner join table_b as b on ...
        var relationSql = "start/* comment */end";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("/* comment */", lst[1]);
        Assert.Equal("end", lst[2]);
    }

    [Fact]
    public void BlockCommentNest()
    {
        /// inner join table_b as b on ...
        var relationSql = "start/* comment /* nest */ */end";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("/* comment /* nest */ */", lst[1]);
        Assert.Equal("end", lst[2]);
    }

    [Fact]
    public void LineCommentInBlockComment()
    {
        /// inner join table_b as b on ...
        var relationSql = "start--/* comment */blockend\r\nlineend";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("--/* comment */blockend", lst[1]);
        Assert.Equal("lineend", lst[2]);
    }
}
