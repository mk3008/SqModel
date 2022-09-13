using Microsoft.VisualStudio.TestPlatform.Utilities;
using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseValueClauseTest
{
    public ParseValueClauseTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        var text = "t.column";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseValueClause();
        var sql = val.ToQuery().CommandText;

        Assert.Equal(text, sql);
    }

    [Fact]
    public void Value()
    {
        var text = "1";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseValueClause();
        var sql = val.ToQuery().CommandText;

        Assert.Equal(text, sql);
    }

    [Fact]
    public void Condition()
    {
        var text = "1 = 1";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseValueClause();
        var sql = val.ToQuery().CommandText;

        Assert.Equal("1", sql);
    }

    [Fact]
    public void Condition2()
    {
        var text = "1 + 1 = 2";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseValueClause();
        var sql = val.ToQuery().CommandText;

        Assert.Equal("1 + 1", sql);
    }

    [Fact]
    public void Expression()
    {
        var text = "(1 + 2) * 3.4";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseValueClause();
        var sql = val.ToQuery().CommandText;

        Assert.Equal(text, sql);
    }

    [Fact]
    public void CaseWhenExpression()
    {
        var text = "case when 1 = 1 then 1 else 2 end";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseValueClause();
        var sql = val.ToQuery().CommandText;

        Assert.Equal(text, sql);
    }

    [Fact]
    public void InlineQuery()
    {
        var text = "(select t.colmn from table as t)";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseValueClause();
        var sql = val.ToQuery().CommandText;

        Assert.Equal(text, sql);
    }
}
