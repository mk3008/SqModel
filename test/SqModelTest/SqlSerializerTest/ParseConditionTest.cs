using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ParseConditionTest
{
    public ParseConditionTest(ITestOutputHelper output)
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
        var clause = p.ParseCondition();

        Assert.Equal(relationSql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void Expression()
    {
        /// inner join table_b as b on ...
        var relationSql = "a.id1 = (1+2) * 3";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseCondition();

        Assert.Equal(relationSql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void SubQuery()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 = (select x.id from table_x as x)";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseCondition();
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
        var clause = p.ParseCondition();
        var text = clause.ToQuery().CommandText;

        Assert.Equal(relationSql, text);
    }

    [Fact]
    public void IsNull()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 is null";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseCondition();
        var text = clause.ToQuery().CommandText;

        Assert.Equal(relationSql, text);
    }

    [Fact]
    public void IsNotNull()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 is not null";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseCondition();
        var text = clause.ToQuery().CommandText;

        Assert.Equal(relationSql, text);
    }

    [Fact]
    public void IsNullAnd()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 is null and a.id1 is not null";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseCondition();
        var text = clause.ToQuery().CommandText;

        Assert.Equal("a.id1 is null", text);
    }

    [Fact]
    public void IsNotNullAnd()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 is not null and a.id1 is not null";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseCondition();
        var text = clause.ToQuery().CommandText;

        Assert.Equal("a.id1 is not null", text);
    }
}
