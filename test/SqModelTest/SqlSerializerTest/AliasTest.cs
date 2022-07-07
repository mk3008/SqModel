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
        using var p = new Parser(",");
        var q = new SelectQuery();
        var col = new SelectColumn();
        p.ParseAlias(q, col);
        Assert.Equal("", col.AliasName);
    }

    [Fact]
    public void Nothing_from()
    {
        using var p = new Parser(" from");
        var q = new SelectQuery();
        var col = new SelectColumn();
        p.ParseAlias(q, col);
        Assert.Equal("", col.AliasName);
    }

    [Fact]
    public void as_commna()
    {
        using var p = new Parser(" as column1 ,");
        var q = new SelectQuery();
        var col = new SelectColumn();
        p.ParseAlias(q, col);
        Assert.Equal("column1", col.AliasName);
    }

    [Fact]
    public void as_from()
    {
        using var p = new Parser(" as column1 from");
        var q = new SelectQuery();
        var col = new SelectColumn();
        p.ParseAlias(q, col);
        Assert.Equal("column1", col.AliasName);
    }

    [Fact]
    public void omit_comma()
    {
        using var p = new Parser(" column1,");
        var q = new SelectQuery();
        var col = new SelectColumn();
        p.ParseAlias(q, col);
        Assert.Equal("column1", col.AliasName);
    }
    [Fact]
    public void omit_from()
    {
        using var p = new Parser(" column1 from");
        var q = new SelectQuery();
        var col = new SelectColumn();
        p.ParseAlias(q, col);
        Assert.Equal("column1", col.AliasName);
    }
}
