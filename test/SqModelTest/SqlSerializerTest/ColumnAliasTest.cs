using SqModel;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class ColumnAliasTest
{
    [Fact]
    public void Nothing_comma()
    {
        using var p = new Parser(" ,");       
        var alias = p.ParseColumnAliasOrDefault();

        Assert.Equal("", alias);
    }

    [Fact]
    public void Nothing_from()
    {
        using var p = new Parser(" from");
        var alias = p.ParseColumnAliasOrDefault();

        Assert.Null(alias);

        var c = p.Peek();
        Assert.Equal('f', c); //roll back
    }

    [Fact]
    public void as_alias_space()
    {
        using var p = new Parser(" as column1 ");
        var alias = p.ParseColumnAliasOrDefault();

        Assert.Equal("column1", alias);
    }

    [Fact]
    public void as_alias_comma()
    {
        using var p = new Parser(" as column1,");
        var alias = p.ParseColumnAliasOrDefault();

        Assert.Equal("column1", alias);
    }

    [Fact]
    public void alias_comma()
    {
        using var p = new Parser(" column1,");
        var alias = p.ParseColumnAliasOrDefault();

        Assert.Equal("column1", alias);
    }

    [Fact]
    public void alias_space()
    {
        using var p = new Parser(" column1 ");
        var alias = p.ParseColumnAliasOrDefault();

        Assert.Equal("column1", alias);
    }

    [Fact]
    public void prefix_match_as()
    {
        using var p = new Parser(" ascolumn1 ");
        var alias = p.ParseColumnAliasOrDefault();

        Assert.Equal("ascolumn1", alias);
    }

    [Fact]
    public void prefix_match_from()
    {
        using var p = new Parser(" fromcolumn1 ");
        var alias = p.ParseColumnAliasOrDefault();

        Assert.Equal("fromcolumn1", alias);
    }
}
