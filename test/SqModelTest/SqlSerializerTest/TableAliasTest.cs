using SqModel;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class TableAliasTest
{
    [Fact]
    public void Nothing_where()
    {
        using var p = new Parser(" where");
        var alias = p.ParseTableAliasOrDefault();

        Assert.Null(alias);

        var c = p.Peek();
        Assert.Equal('w', c); //roll back
    }

    [Fact]
    public void Nothing_orderby()
    {
        using var p = new Parser(" order by");
        var alias = p.ParseTableAliasOrDefault();

        Assert.Null(alias);

        var c = p.Peek();
        Assert.Equal('o', c); //roll back
    }

    [Fact]
    public void as_alias_space()
    {
        using var p = new Parser(" as table1 ");
        var alias = p.ParseTableAliasOrDefault();

        Assert.Equal("table1", alias);
    }

    [Fact]
    public void alias_space()
    {
        using var p = new Parser(" table1 ");
        var alias = p.ParseTableAliasOrDefault();

        Assert.Equal("table1", alias);
    }
}
