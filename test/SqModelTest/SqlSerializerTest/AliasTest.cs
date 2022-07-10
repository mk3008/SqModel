using SqModel;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class AliasTest
{
    [Fact]
    public void Nothing_comma()
    {
        using var p = new Parser(" ,");
        
        var alias = string.Empty;
        Action<string> setter = (x) => alias = x;
        p.ParseColumnAlias(setter);

        Assert.Equal("", alias);
    }

    [Fact]
    public void Nothing_from()
    {
        using var p = new Parser(" from");

        var alias = string.Empty;
        Action<string> setter = (x) => alias = x;
        p.ParseColumnAlias(setter);

        Assert.Equal("", alias);

        var c = p.Peek();
        Assert.Equal('f', c); //roll back
    }

    [Fact]
    public void as_alias_commna()
    {
        using var p = new Parser(" as column1 ,");

        var alias = string.Empty;
        Action<string> setter = (x) => alias = x;
        p.ParseColumnAlias(setter);

        Assert.Equal("column1", alias);
    }

    [Fact]
    public void as_alias_from()
    {
        using var p = new Parser(" as column1 from");

        var alias = string.Empty;
        Action<string> setter = (x) => alias = x;
        p.ParseColumnAlias(setter);

        Assert.Equal("column1", alias);
    }

    [Fact]
    public void alias_comma()
    {
        using var p = new Parser(" column1 ,");

        var alias = string.Empty;
        Action<string> setter = (x) => alias = x;
        p.ParseColumnAlias(setter);

        Assert.Equal("column1", alias);
    }

    [Fact]
    public void omit_comma_nospace()
    {
        using var p = new Parser(" column1,");

        var alias = string.Empty;
        Action<string> setter = (x) => alias = x;
        p.ParseColumnAlias(setter);

        Assert.Equal("column1", alias);
    }

    [Fact]
    public void alias_from()
    {
        using var p = new Parser(" column1 from");

        var alias = string.Empty;
        Action<string> setter = (x) => alias = x;
        p.ParseColumnAlias(setter);

        Assert.Equal("column1", alias);
    }

    [Fact]
    public void prefix_match_as()
    {
        using var p = new Parser(" ascolumn1 from");

        var alias = string.Empty;
        Action<string> setter = (x) => alias = x;
        p.ParseColumnAlias(setter);

        Assert.Equal("ascolumn1", alias);
    }

    [Fact]
    public void prefix_match_from()
    {
        using var p = new Parser(" fromcolumn1 from");

        var alias = string.Empty;
        Action<string> setter = (x) => alias = x;
        p.ParseColumnAlias(setter);

        Assert.Equal("fromcolumn1", alias);
    }
}
