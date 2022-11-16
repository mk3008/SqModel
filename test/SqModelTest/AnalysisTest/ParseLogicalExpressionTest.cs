using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseLogicalExpressionTest
{
    public ParseLogicalExpressionTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        /// inner join table_b as b on ...
        var relationSql = "a.id1 = b.id2";
        var clause = LogicalExpressionParser.Parse(relationSql);

        Assert.Equal(relationSql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void Expression()
    {
        /// inner join table_b as b on ...
        var relationSql = "a.id1 = (1+2) * 3";
        var clause = LogicalExpressionParser.Parse(relationSql);

        Assert.Equal(relationSql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void SubQuery()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 = (select x.id from table_x as x)";
        var clause = LogicalExpressionParser.Parse(relationSql);
        var text = clause.ToQuery().CommandText;

        Assert.Equal(relationSql, text);
    }

    [Fact]
    public void Sign()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 >= 10";
        var clause = LogicalExpressionParser.Parse(relationSql);
        var text = clause.ToQuery().CommandText;

        Assert.Equal(relationSql, text);
    }

    [Fact]
    public void IsNull()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 is null";
        var clause = LogicalExpressionParser.Parse(relationSql);
        var text = clause.ToQuery().CommandText;

        Assert.Equal(relationSql, text);
    }

    [Fact]
    public void IsNotNull()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 is not null";
        var clause = LogicalExpressionParser.Parse(relationSql);
        var text = clause.ToQuery().CommandText;

        Assert.Equal(relationSql, text);
    }

    [Fact]
    public void IsNullAnd()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 is null and a.id1 is not null";
        var clause = LogicalExpressionParser.Parse(relationSql);
        var text = clause.ToQuery().CommandText;

        Assert.Equal("a.id1 is null and a.id1 is not null", text);
    }

    [Fact]
    public void IsNotNullAnd()
    {
        /// inner join table_b as b on ...
        var relationSql = @"a.id1 is not null and a.id1 is not null";
        var clause = LogicalExpressionParser.Parse(relationSql);
        var text = clause.ToQuery().CommandText;

        Assert.Equal("a.id1 is not null and a.id1 is not null", text);
    }
}
