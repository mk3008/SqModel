using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ParseRelationTest
{
    public ParseRelationTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        /// inner join table_b as b on ...
        var relationSql = "a.id1 = b.id2";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseRelation();

        Assert.Equal(relationSql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void Expression()
    {
        /// inner join table_b as b on ...
        var relationSql = "a.id1 = (1+2) * 3";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseRelation();

        Assert.Equal(relationSql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void SubQuery()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 = (select x.id from table_x as x)";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseRelation();
        var text = clause.ToQuery().CommandText;

        Assert.Equal(relationSql, text);
    }

    [Fact]
    public void Sign()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 >= 10";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseRelation();
        var text = clause.ToQuery().CommandText;

        Assert.Equal(relationSql, text);
    }
}
